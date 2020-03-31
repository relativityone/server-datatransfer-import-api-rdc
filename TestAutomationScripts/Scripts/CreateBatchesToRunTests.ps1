# Include paths file
$ScriptDirectory = Split-Path (Split-Path $MyInvocation.MyCommand.Path -Parent) -Parent
. (Join-Path $ScriptDirectory SetPaths.ps1)
. (Join-Path $ScriptDirectory Common.ps1)

Write-Host
Write-Host "Create *.bat files to run tests" -ForegroundColor Green

CreateFolderStructureForTestResult -ToBeExecutedOnTestVM $IsExecutedOnTestVM

# Api-Export Integration Tests
CreateBatForTestExecution -TestAssemblyName “Api-Export” -ToBeExecutedOnTestVM $IsExecutedOnTestVM

# Api-Import Integration Tests
CreateBatForTestExecution -TestAssemblyName “Api-Import” -ToBeExecutedOnTestVM $IsExecutedOnTestVM

# Api-Shared Integration Tests
CreateBatForTestExecution -TestAssemblyName “Api-Shared” -ToBeExecutedOnTestVM $IsExecutedOnTestVM

# UIAutomation Tests
CreateBatForTestExecution -TestAssemblyName “UITests” -ToBeExecutedOnTestVM $IsExecutedOnTestVM

# Create bat to convert results from .xml to .html
CreateBatToConvertResultsToHtml -ToBeExecutedOnTestVM $IsExecutedOnTestVM