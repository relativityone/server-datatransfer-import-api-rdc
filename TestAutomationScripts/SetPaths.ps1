# Test VM settings
[string]$RemoteHostName              = "CHANGE_ME_TO_TEST_VM_NAME"
[string]$SharedFolderOnRemoteHost    = "CHANGE_ME_TO_SHARED_FOLDER_NAME_ON_TEST_VM"

# Paths on Test VM
[string]$NUnitConsoleRunner          = "C:\$SharedFolderOnRemoteHost\packages\NUnit.ConsoleRunner\tools\nunit3-console.exe"
[string]$ReportNUnit                 = "C:\$SharedFolderOnRemoteHost\reportunit.1.2.1\tools\ReportUnit.exe" 

# Local path
[string]$SourcesDirectory            = Split-Path -Path (Split-Path -Path $MyInvocation.MyCommand.Definition -Parent) -Parent
