# Include paths file
$ScriptDirectory = Split-Path (Split-Path $MyInvocation.MyCommand.Path -Parent) -Parent
. (Join-Path $ScriptDirectory SetPaths.ps1)

Write-Host 
Write-Host "Prepare nunit tests dll to execute tests remotely" -ForegroundColor Green

& "$SourcesDirectory\build.ps1" Build, BuildUIAutomation

Write-Host 
Write-Host "Copy files to Test VM '$RemoteHostName'" -ForegroundColor Green

# Sources and tests dll files
if (Test-Path -LiteralPath "\\$RemoteHostName\$SharedFolderOnRemoteHost\Source") { Remove-Item -LiteralPath "\\$RemoteHostName\$SharedFolderOnRemoteHost\Source" -Recurse -Force}

$items = Get-ChildItem -LiteralPath "$SourcesDirectory\Source" -Directory -Depth 0

foreach($item in $items){ 
    if(Test-Path -LiteralPath "$SourcesDirectory\Source\$item\bin") {
        New-Item -Path "\\$RemoteHostName\$SharedFolderOnRemoteHost\Source\$item" -ItemType Directory -Force
        Copy-Item -LiteralPath "$SourcesDirectory\Source\$item\bin" -Destination "\\$RemoteHostName\$SharedFolderOnRemoteHost\Source\$item" -Force -Recurse -PassThru
    }
}

# Test data, NUnit console, ...
if (Test-Path -LiteralPath "\\$RemoteHostName\$SharedFolderOnRemoteHost\packages") { Remove-Item -LiteralPath "\\$RemoteHostName\$SharedFolderOnRemoteHost\packages" -Recurse -Force }
Copy-Item -LiteralPath "$SourcesDirectory\packages" -Destination "\\$RemoteHostName\$SharedFolderOnRemoteHost" -Force -Recurse -PassThru

# Test Automation Scripts
Copy-Item -LiteralPath "$SourcesDirectory\TestAutomationScripts" -Destination "\\$RemoteHostName\$SharedFolderOnRemoteHost" -Force -Recurse -PassThru