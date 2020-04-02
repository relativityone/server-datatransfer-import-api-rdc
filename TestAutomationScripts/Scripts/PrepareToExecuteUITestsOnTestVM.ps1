# Include paths file
$ScriptDirectory = Split-Path (Split-Path $MyInvocation.MyCommand.Path -Parent) -Parent
. (Join-Path $ScriptDirectory SetPaths.ps1)
. (Join-Path $ScriptDirectory Common.ps1)

Write-Host 
Write-Host "Prepare nunit tests dll to execute tests remotely" -ForegroundColor Green

# Prepare app.config files

# Build solution
& "$SourcesDirectory\build.ps1" Build, BuildUIAutomation

# Copy files to remote machine
PrepareDataForAutomaticTests -SourceFolder $SourcesDirectory -DestinationFolder "\\$RemoteHostName\$SharedFolderOnRemoteHost"