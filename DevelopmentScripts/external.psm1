$global:SignToolPath = "${Env:ProgramFiles(x86)}\Windows Kits\10\bin\10.0.17763.0\x86\signtool.exe"

Function Invoke-SignDirectoryFiles {
    param(
        [String[]] $DirectoryCandidates
    )

    foreach ($directory in $DirectoryCandidates) {
        if (-Not (Test-Path $directory -PathType Container)) {
            Throw "The '$directory' can't be digitally signed because the directory doesn't exist. Verify the build script and project files are in agreement."
        }

        $filesToSign = Get-ChildItem -Path $directory -Recurse -Include *.dll, *.exe, *.msi | Where-Object { $_.Name -Match ".*Relativity.*|.*kCura.*" }
        $totalFilesToSign = $filesToSign.Length
        if ($totalFilesToSign -eq 0) {
            Throw "The '$directory' can't be digitally signed because there aren't any candidate files within the directory. Verify the build script and project files are in agreement."
        }

        Write-Output "Signing $totalFilesToSign total files in $directory"
        foreach ($FileToSign in $filesToSign) {
            Invoke-SignFile $FileToSign.FullName
        }
    }
}

Function Invoke-SignFile {
    param(
        [String] $File
    )

    if (!$File) {
        Throw "You must specify a non-null path to digitally sign a file. Check to make sure the path value or variable passed to this method is valid."
    }
    
    $RetryAttempts = 3
    $SignSites = @(
        "http://timestamp.comodoca.com/authenticode",
        "http://timestamp.verisign.com/scripts/timstamp.dll",
        "http://tsa.starfieldtech.com"
    )

    $Signed = $false
    & $SigntoolPath verify /pa /q $File
    $Signed = $?
    if (-not $Signed) {
        For ($i = 0; $i -lt $RetryAttempts; $i++) {
            ForEach ($Site in $SignSites) {
                Write-Host "Attempting to sign" $File "using" $Site "..."
                & $SigntoolPath sign /a /t $Site /d "Relativity" /du "http://www.kcura.com" $File
                $Signed = $?
                if ($Signed) {
                    Write-Host "Signed $File Successfully!"
                    break
                }
            }  
                    
            if ($Signed) {
                break
            }
        }
        
        if (-not $Signed) {
            Throw "Failed to sign $File. See the error above."
        }
    }
    else {
        Write-Host $File "is already signed."
    }
}

Export-ModuleMember -Variable SignToolPath
Export-ModuleMember -Function Invoke-SignDirectoryFiles