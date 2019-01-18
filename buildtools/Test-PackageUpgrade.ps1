<#
.SYNOPSIS
Performs test so ensure that upgrading from the old to the new package is validated.

.DESCRIPTION
The purpose of this script is to compare package dependencies for both the old and new package and ensure
that all dependencies are greater than or equal to the old package. The validation check is designed to 
prevent the scenario where consumers of the specified package don't experience a downgrade with any of the
associated files and cause Microsft Installer (MSI) to uninstall components.

.PARAMETER Id
The package identifier.

.PARAMETER OldVersion
The old package version.

.PARAMETER NewVersion
The new package version.

.EXAMPLE
.\Test-PackageUpgrade.ps1 -Id "Relativity.ImportExport" -OldVersion "9.7.209.7" -NewVersion "10.2.9.12"
#>
[CmdletBinding()]
param (
  [Parameter(Mandatory=$true)]
  [ValidateNotNullOrEmpty()]
    [string]$Id,

  [Parameter(Mandatory=$true)]
  [ValidateNotNullOrEmpty()]
    [string]$OldVersion,

  [Parameter(Mandatory=$true)]
  [ValidateNotNullOrEmpty()]
    [string]$NewVersion
)

$manager = [PackageUpgradeValidator]::new()
$manager.Validate($Id, $OldVersion, $NewVersion)

class PackageUpgradeValidator {

  hidden [void]
  Validate([string] $id, [string] $oldVersion, [string] $newVersion) {
    $this.DownloadNuGet()
    $oldMap = $this.GetPackageHashTable($id, $oldVersion)
    $newMap = $this.GetPackageHashTable($id, $newVersion)
    $totalGreenPackages = 0
    $skippedPackages = 0
    foreach ($idKey in $newMap.Keys) 
    {
      if ($oldMap.ContainsKey($idKey)) {

        #TODO: PS6 introduces a semantic version object that can handle labels within versions (IE DEV, RC)
        #      This should never be an issue because GOLD versions should never include labels.
        #      For now, these are skipped with a warning to manually inspwct the versions.
        $oldVersionString = $oldMap[$idKey].Version
        $newVersionString = $newMap[$idKey].Version
        [System.Version]$oldVersion = $null
        $oldVersionParse = [System.Version]::TryParse($oldVersionString, [ref]$oldVersion)
        [System.Version]$newVersion = $null
        $newVersionParse = [System.Version]::TryParse($newVersionString, [ref]$newVersion)
        if ($oldVersionParse -eq $false -or $newVersionParse -eq $false)
        {
          $skippedPackages++
          $this.DisplayManuallyInspectStatus("Package dependency $idKey-$oldVersionString (old) or $idKey-$newVersionString (new) version parse failure. Is this a semantic version?")
        }
        else {
          if ($newVersion -eq $oldVersion)
          {
            $totalGreenPackages++
            $this.DisplayPassStatus("Package dependency $idKey-$oldVersion did not change")
          }
          elseif ($newVersion -gt $oldVersion)
          {
            $totalGreenPackages++
            $this.DisplayPassStatus("Package dependency $idKey-$oldVersion is greater than $newVersion [Pass]")
          }
          else {
            $this.DisplayFailStatus("Package dependency $idKey-$oldVersion is less than $newVersion")
          }
        }
      }
      else {
        $skippedPackages++
        $this.DisplaySkippedStatus("Package dependency $idKey does not exist in the old package")
      }
    }

    $exitCode = 1
    $totalPackages = $newMap.Count - $skippedPackages
    if ($totalGreenPackages -gt 0 -and $totalGreenPackages -eq $totalPackages)
    {
      $exitCode = 0
    }

    Write-Host "Validated $totalGreenPackages of $totalPackages packages."
    if ($skippedPackages -gt 0){
      Write-Host "Skipped $skippedPackages packages."
    }

    Write-Host "Exit code: $exitCode"
    exit $exitCode
  }

  hidden [void]
  DisplayManuallyInspectStatus($message) {
    Write-Host " [Mannually Inspect]" -ForegroundColor Yellow
  }

  hidden [void]
  DisplayPassStatus($message) {
    Write-Host $message -nonewline
    Write-Host " [Pass]" -ForegroundColor Green
  }

  hidden [void]
  DisplayFailStatus($message) {
    Write-Host $message -nonewline
    Write-Host " [Fail]" -ForegroundColor Red
  }

  hidden [void]
  DisplaySkippedStatus($message) {
    Write-Host $message -nonewline
    Write-Host " [Skipped]" -ForegroundColor Magenta
  }

  hidden [hashtable]
  GetPackageHashTable([string] $id, [string] $version)
  {
    $tempDir = $this.CreatePackageTempDir()
    try {
      $this.InstallPackage($id, $version, $tempDir)
      $nupkgFiles = Get-Childitem -Path $tempDir -Recurse -Filter "*.nupkg"
      if ($nupkgFiles.Count -eq 0)
      {
        throw [System.InvalidOperationException] "The NuGet package $id.$version was installed but didn't contain a .nupkg file."
      }
      
      $map = @{}
      foreach ($nupkgFile in $nupkgFiles) {
        $package = $this.GetPackageDetails($nupkgFile.FullName)
        if ($package -ne $id)
        {
          $map[$package.Id] = $package
        }
      }

      return $map
    }
    finally
    {
      $this.DeleteDir($tempDir)
    }
  }

  hidden [void]
	InstallPackage ([string] $id, [string] $version, [string] $extractDir) {
		$this.LogInfo("Installing the [$id] package to the [$extractDir] directory...")
		$commandLineArgs = "install $id -Version $version -OutputDirectory ""$extractDir"""
		$this.ExecNugetProcess("package installation/extraction", $commandLineArgs)
		$this.LogInfo("Extracted the [$id] package to the [$extractDir] directory.")
	}

  hidden [NugetPackage]
  GetPackageDetails([string] $file) {
    # Anothe temp directory is used to avoid path too long exceptions.
    $tempDir = $this.CreatePackageTempDir()

    try {
      # The rename is required because Expand-Archive rejects any file extension that isn't .zip 
      $fileDir = [System.IO.Path]::GetDirectoryName($file)
      $fileNameNoExt = [System.IO.Path]::GetFileNameWithoutExtension($file)
      $targetZipFile = [System.IO.Path]::Combine($fileDir, "$fileNameNoExt.zip")
      $this.RenameFile($file, $targetZipFile)
                  
      Expand-Archive $targetZipFile -DestinationPath $tempDir
      $files = Get-Childitem -Path $tempDir -Recurse -Filter "*.nuspec"
      if ($files.Count -eq 0)
      {
        throw [System.InvalidOperationException] "The NuGet package $file was extracted but didn't contain a .nupec file."
      }

      $nuspecFile = $files[0].FullName
      $package = [NugetPackage]::new()
      $package.Id = $this.SelectPackageMetadata($nuspecFile, "id")
      $package.Version = $this.SelectPackageMetadata($nuspecFile, "version")
      return $package
    }
    finally {
      $this.DeleteDir($tempDir)
    }
  }

  hidden [string]
  SelectPackageMetadata($file, $metadata) {
    $xpath = "//*[local-name()='$metadata']/text()"
    $result = Select-Xml -Path $file -XPath $xpath      
    if ($null -ne $result)
    {
      return $result.Node.Value
    }

    throw [System.InvalidOperationException] "The xpath query failed to retrieve the $file .nuspec $metadata metadata value." 
  }

  hidden [string]
	CreatePackageTempDir() {
		$folder = "RelativityPackageTmpDir_" + [System.DateTime]::Now.Ticks + "_" + [System.Guid]::NewGuid()
		$tempPath = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), $folder)
		$this.CreateDir($tempPath)
		return $tempPath
  }

  hidden [void]
	CreateDir([string] $dir) {
		if (-Not (Test-Path -Path $dir)) {
			$this.LogInfo("Creating directory [$dir]...")
			New-Item -ItemType directory -Path $dir
			$this.LogInfo("Created directory [$dir].")
		}
  }

  hidden [void]
	DeleteDir ([string] $dir) {
		if (Test-Path -Path $dir) {
			$this.LogInfo("Deleting directory [$dir]...")
			Remove-Item "$dir" -Force -Recurse
			$this.LogInfo("Deleted directory [$dir].")
		}
  }

  hidden [void]
	CopyFile([string] $sourceFile, [string] $targetDir) {
    $this.LogInfo("Copying source file [$sourceFile] to [$targetDir]...")
		Copy-Item $sourceFile -Destination $targetDir
    $this.LogInfo("Copied source file [$sourceFile] to [$targetDir].")
  }
  
  hidden [void]
	RenameFile ([string] $file, [string] $newFileName) {
		$this.LogInfo("Renaming file [$file] with new filename [$newFileName]...")
		Rename-Item -Path $file -NewName $newFileName
		$this.LogInfo("Renamed file [$file] with new filename [$newFileName].")
  }
  
  hidden [void]
  DownloadNuGet()
  {
    $sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
    $targetNugetExe = Join-Path $PSScriptRoot "nuget.exe"
    if (-Not (Test-Path -Path $targetNugetExe)) {
      Invoke-WebRequest $sourceNugetExe -OutFile $targetNugetExe
    }
  }

  hidden [void]
	ExecNugetProcess([string] $id, [string] $commandLineArgs) {
		$this.ExecNugetProcess($id, $commandLineArgs, $null)
	}

	hidden [void]
	ExecNugetProcess([string] $commandDescription, [string] $commandLineArgs, [string] $workingDirectory) {
		$exitCode = 0
		$exePath = Join-Path $PSScriptRoot "nuget.exe"
		if (-Not ([string]::IsNullOrEmpty($($workingDirectory)))) {
			$process = Start-Process -FilePath "$exePath" -ArgumentList "$commandLineArgs" -NoNewWindow -PassThru -Wait -WorkingDirectory $workingDirectory
			$exitCode = $process.ExitCode
		}
		else {
			$process = Start-Process -FilePath "$exePath" -ArgumentList "$commandLineArgs" -NoNewWindow -PassThru -Wait
			$exitCode = $process.ExitCode
		}

		if ($exitCode -eq 0)
		{
			return
		}

		throw [System.InvalidOperationException] "The Nuget package command [$commandDescription] with arguments [$commandLineArgs] failed with the $exitCode exit code."
	}

  hidden [void]
	LogInfo([string] $message) {
		Write-Host $message
	}
}

class NugetPackage {
	[string] $Id
	[string] $Version
}