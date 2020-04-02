# Test VM settings
[string]$RemoteHostName              = "CHANGE_ME_TO_TEST_VM_NAME"
[string]$SharedFolderOnRemoteHost    = "CHANGE_ME_TO_SHARED_FOLDER_NAME_ON_TEST_VM"

[bool]$IsExecutedOnTestVM                = $env:computername.Contains("P-DV-VM")

# Paths on Test VM
[string]$RootFolderOnTestVM              = "C:\$SharedFolderOnRemoteHost\import-api-rdc"
[string]$NUnitConsoleRunnerTestVM        = "$RootFolderOnTestVM\packages\NUnit.ConsoleRunner\tools\nunit3-console.exe"
[string]$ReportNUnitTestVM               = "$RootFolderOnTestVM\packages\ReportUnit\tools\ReportUnit.exe"

# Folder for tests results
[string]$TestResultsFolderName           = "TestReports"
[string]$IntegrationTestsReportDirName   = "integration-tests"
[string]$UIAutomationTestsReportDirName  = "ui-automation-tests"
[string]$UnitTestsReportDirName          = "unit-tests"

# Local path
[string]$SourcesDirectory                = Split-Path -Path (Split-Path -Path $MyInvocation.MyCommand.Definition -Parent) -Parent
[string]$NUnitConsoleRunner              = "$SourcesDirectory\packages\NUnit.ConsoleRunner\tools\nunit3-console.exe"
[string]$ReportNUnit                     = "$SourcesDirectory\packages\ReportUnit\tools\ReportUnit.exe"


