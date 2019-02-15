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
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$Id,

    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$OldVersion,

    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$NewVersion
)

$manager = [PackageUpgradeValidator]::new()
$manager.Validate($Id, $OldVersion, $NewVersion)

class PackageUpgradeValidator {
    [bool] $verbose = $false

    hidden [void]
    Validate([string] $id, [string] $oldVersion, [string] $newVersion) {
        $this.DownloadNuGet()
        $oldMap = $this.GetPackageHashTable($id, $oldVersion)
        $newMap = $this.GetPackageHashTable($id, $newVersion)
        $totalGreenPackages = 0
        $skippedPackages = 0
        $width = (Get-Host).UI.RawUI.MaxWindowSize.Width
        $separator = "=" * $width
        $this.LogInfo("")
        $this.LogInfo("Package Upgrade Summary")
        $this.LogInfo($separator)
        foreach ($item in $newMap.GetEnumerator() | Sort-Object Name) {
            $id = $item.Key
            if ($oldMap.ContainsKey($id)) {

                #TODO: PS6 introduces a semantic version object that can handle labels within versions (IE DEV, RC)
                #      This should never be an issue because GOLD versions should never include labels.
                #      For now, these are skipped with a warning to manually inspwct the versions.
                $oldVersionString = $oldMap[$id].Version
                $newVersionString = $newMap[$id].Version
                [System.Version]$oldVersion = $null
                $oldVersionParse = [System.Version]::TryParse($oldVersionString, [ref]$oldVersion)
                [System.Version]$newVersion = $null
                $newVersionParse = [System.Version]::TryParse($newVersionString, [ref]$newVersion)
                if ($oldVersionParse -eq $false -or $newVersionParse -eq $false) {
                    $skippedPackages++
                    $versionInError = if (![string]::IsNullOrEmpty($oldVersionString)) { $oldVersionString } else { $newVersionString }
                    $this.DisplayManuallyInspectStatus("${id}: $versionInError version parse failure. Is this a semantic version?")
                }
                else {
                    if ($newVersion -eq $oldVersion) {
                        $totalGreenPackages++
                        $this.DisplayPassStatus("${id}: $newVersion = $oldVersion")
                    }
                    elseif ($newVersion -gt $oldVersion) {
                        $totalGreenPackages++
                        $this.DisplayPassStatus("${id}: $newVersion > $oldVersion")
                    }
                    else {
                        $this.DisplayFailStatus("${id}: $newVersion < $oldVersion")
                    }
                }
            }
            else {
                $skippedPackages++
                $this.DisplaySkippedStatus("$id doesn't exist in the old package")
            }
        }

        $exitCode = 1
        $totalPackages = $newMap.Count - $skippedPackages
        if ($totalGreenPackages -gt 0 -and $totalGreenPackages -eq $totalPackages) {
            $exitCode = 0
        }

        $this.LogInfo($separator)
        $this.LogInfo("")
        $this.LogInfo("Validated $totalGreenPackages of $totalPackages packages.")
        if ($skippedPackages -gt 0) {
            $this.LogInfo("Skipped $skippedPackages packages.")
        }

        $this.LogInfo("Exit code: $exitCode")
        exit $exitCode
    }

    hidden [void]
    DisplayManuallyInspectStatus($message) {
        Write-Host $message -nonewline
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
    GetPackageHashTable([string] $id, [string] $version) {
        $this.LogInfo("Retrieving the $id $version package details...")
        $tempDir = $this.CreatePackageTempDir()
        try {
            $this.InstallPackage($id, $version, $tempDir)
            $nupkgFiles = Get-Childitem -Path $tempDir -Recurse -Filter "*.nupkg"
            if ($nupkgFiles.Count -eq 0) {
                throw [System.InvalidOperationException] "The NuGet package $id.$version was installed but didn't contain a .nupkg file."
            }

            $map = @{}
            foreach ($nupkgFile in $nupkgFiles) {
                $package = $this.GetPackageDetails($nupkgFile.FullName)
                if ($package -ne $id) {
                    $map[$package.Id] = $package
                }
            }

            return $map
        }
        finally {
            $this.DeleteDir($tempDir)
        }
    }

    hidden [void]
    InstallPackage ([string] $id, [string] $version, [string] $extractDir) {
        $this.LogVerbose("Installing the [$id] package to the [$extractDir] directory...")
        $commandLineArgs = "install $id -Version $version -OutputDirectory ""$extractDir"""
        $this.ExecNugetProcess("package installation/extraction", $commandLineArgs)
        $this.LogVerbose("Extracted the [$id] package to the [$extractDir] directory.")
    }

    hidden [NugetPackage]
    GetPackageDetails([string] $file) {
        # Another temp directory is used to avoid path too long exceptions.
        $tempDir = $this.CreatePackageTempDir()

        try {
            # The rename is required because Expand-Archive rejects any file extension that isn't .zip
            $fileDir = [System.IO.Path]::GetDirectoryName($file)
            $fileNameNoExt = [System.IO.Path]::GetFileNameWithoutExtension($file)
            $targetZipFile = [System.IO.Path]::Combine($fileDir, "$fileNameNoExt.zip")
            $this.RenameFile($file, $targetZipFile)
            Expand-Archive $targetZipFile -DestinationPath $tempDir
            $files = Get-Childitem -Path $tempDir -Recurse -Filter "*.nuspec"
            if ($files.Count -eq 0) {
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
        if ($null -ne $result) {
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
            $this.LogVerbose("Creating directory [$dir]...")
            New-Item -ItemType directory -Path $dir
            $this.LogVerbose("Created directory [$dir].")
        }
    }

    hidden [void]
    DeleteDir ([string] $dir) {
        if (Test-Path -Path $dir) {
            $this.LogVerbose("Deleting directory [$dir]...")
            Remove-Item "$dir" -Force -Recurse
            $this.LogVerbose("Deleted directory [$dir].")
        }
    }

    hidden [void]
    CopyFile([string] $sourceFile, [string] $targetDir) {
        $this.LogVerbose("Copying source file [$sourceFile] to [$targetDir]...")
        Copy-Item $sourceFile -Destination $targetDir
        $this.LogVerbose("Copied source file [$sourceFile] to [$targetDir].")
    }

    hidden [void]
    RenameFile ([string] $file, [string] $newFileName) {
        $this.LogVerbose("Renaming file [$file] with new filename [$newFileName]...")
        Rename-Item -Path $file -NewName $newFileName
        $this.LogVerbose("Renamed file [$file] with new filename [$newFileName].")
    }

    hidden [void]
    DownloadNuGet() {
        $sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
        $targetNugetExe = Join-Path $PSScriptRoot "nuget.exe"
        if (-Not (Test-Path -Path $targetNugetExe)) {
            Invoke-WebRequest $sourceNugetExe -OutFile $targetNugetExe
        }
    }

    hidden [void]
    ExecNugetProcess([string] $commandDescription, [string] $commandLineArgs) {
        $exePath = Join-Path $PSScriptRoot "nuget.exe"
        $pinfo = New-Object System.Diagnostics.ProcessStartInfo
        $pinfo.FileName = $exePath
        $pinfo.RedirectStandardError = $true
        $pinfo.RedirectStandardOutput = $true
        $pinfo.UseShellExecute = $false
        $pinfo.Arguments = $commandLineArgs
        $p = New-Object System.Diagnostics.Process
        $p.StartInfo = $pinfo
        $p.Start() | Out-Null
        $stdout = $p.StandardOutput.ReadToEnd()
        $stderr = $p.StandardError.ReadToEnd()
        $p.WaitForExit()
        $exitCode = $p.ExitCode
        if ($exitCode -eq 0) {
            return
        }

        if (![string]::IsNullOrEmpty($stdout)) {
            $this.LogError($stdout)
        }

        if (![string]::IsNullOrEmpty($stderr)) {
            $this.LogError($stderr)
        }

        throw [System.InvalidOperationException] "The Nuget package command [$commandDescription] with arguments [$commandLineArgs] failed with the $exitCode exit code."
    }

    hidden [void]
    LogError([string] $message) {
        Write-Host $message -ForegroundColor Red
    }

    hidden [void]
    LogInfo([string] $message) {
        Write-Host $message
    }

    hidden [void]
    LogVerbose([string] $message) {
        if ($this.logVerbose -eq $true) {
            Write-Host $message
        }
    }
}

class NugetPackage {
    [string] $Id
    [string] $Version
}