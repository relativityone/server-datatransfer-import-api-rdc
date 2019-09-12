param(
[string]$NewVersion
)
cd .\Version

Get-ChildItem -Include AssemblySharedInfo.cs, AssemblySharedInfo.vb -Recurse | 
    ForEach-Object {
        $_.IsReadOnly = $false
        (Get-Content -Path $_) -replace '(?<=Assembly(?:(File|Informational))?Version\(")[^"]*(?="\))', $NewVersion |
            Set-Content -Path $_
    }
cd .. 