$NewVersion = '10.1.2.3'
cd .\Version

Get-ChildItem -Include AssemblySharedInfo.cs, AssemblySharedInfo.vb -Recurse | 
    ForEach-Object {
        $_.IsReadOnly = $false
        (Get-Content -Path $_) -replace '(?<=Assembly(?:(File|Informational))?Version\(")[^"]*(?="\))', $NewVersion |
            Set-Content -Path $_
    }
cd .. 