# This module contains functions for folder management

Function Initialize-Folder {
    param(
        [Parameter(Mandatory = $true, Position = 0)]
        [String] $Path,
        [Parameter()]
        [switch] $Safe
    )

    if (!$Path) {
        Throw "You must specify a non-null path to initialize a folder. Check to make sure the path value or variable passed to this method is valid."
    }

    if ((Test-Path $Path) -and $Safe) {
        Write-Host "The directory '$Path' already exists."
        Return
    }

    if (Test-Path $Path) {
        Remove-Item -Recurse -Force $Path -ErrorAction Stop
        Write-Host "Deleted the '$Path' directory."
    }

    New-Item -Type Directory $Path -Force -ErrorAction Stop | Out-Null
    Write-Host "Created the '$Path' directory."
}

Function Copy-Folder {
    param(
        [String] $SourceDir,
        [String] $TargetDir
    )

    $robocopy = "robocopy.exe"
    Write-Output "Copying the build artifacts from $SourceDir to $TargetDir"
    & $robocopy "$SourceDir" "$TargetDir" /MIR /is /R:6 /W:10 /FP /MT

    # https://ss64.com/nt/robocopy-exit.html
    if ($LASTEXITCODE -ge 8) {
        Throw "An error occured while copying the build artifacts from $SourceDir to $TargetDir. Robocopy exit code = $LASTEXITCODE"
    }
}