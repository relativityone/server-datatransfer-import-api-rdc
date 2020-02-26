# Include paths file
$ScriptDirectory = Split-Path (Split-Path $MyInvocation.MyCommand.Path -Parent) -Parent
. (Join-Path $ScriptDirectory SetPaths.ps1)

Write-Host
Write-Host "Create *.bat files to run tests" -ForegroundColor Green

# Prepare folders for results
[string]$TestResultsFolder           = "C:\$SharedFolderOnRemoteHost\TestResults"

if (Test-Path -LiteralPath "$TestResultsFolder") { Remove-Item -LiteralPath $TestResultsFolder -Recurse -Force }
New-Item -Path $TestResultsFolder -ItemType Directory -Force
New-Item -Path "$TestResultsFolder\Result" -ItemType Directory -Force
New-Item -Path "$TestResultsFolder\Output" -ItemType Directory -Force
New-Item -Path "$TestResultsFolder\Html" -ItemType Directory -Force

# Api-Export Integration Tests
[string]$RunTestBat      = "C:\$SharedFolderOnRemoteHost\RunIntegrationExportTests.bat"
[string]$TestDll         = "C:\$SharedFolderOnRemoteHost\Source\Relativity.DataExchange.Export.NUnit.Integration\bin\Relativity.DataExchange.Export.NUnit.Integration.dll"
[string]$OutFilePath     = "$TestResultsFolder\Output\api_export_output.xml"
[string]$ResultFilePath  = "$TestResultsFolder\Result\api_export_result.xml"

Set-Content -LiteralPath $RunTestBat -Value """$NUnitConsoleRunner"" ""$TestDll""  /out:""$OutFilePath"" /result:""$ResultFilePath""" -PassThru -Force

# Api-Import Integration Tests
[string]$RunTestBat      = "C:\$SharedFolderOnRemoteHost\RunIntegrationImportTests.bat"
[string]$TestDll         = "C:\$SharedFolderOnRemoteHost\Source\Relativity.DataExchange.Import.NUnit.Integration\bin\Relativity.DataExchange.Import.NUnit.Integration.dll"
[string]$OutFilePath     = "$TestResultsFolder\Output\api_import_output.xml"
[string]$ResultFilePath  = "$TestResultsFolder\Result\api_import_result.xml"

Set-Content -LiteralPath $RunTestBat -Value """$NUnitConsoleRunner"" ""$TestDll""  /out:""$OutFilePath"" /result:""$ResultFilePath""" -PassThru -Force

# Api-Shared Integration Tests
[string]$RunTestBat      = "C:\$SharedFolderOnRemoteHost\RunIntegrationSharedTests.bat"
[string]$TestDll         = "C:\$SharedFolderOnRemoteHost\Source\Relativity.DataExchange.NUnit.Integration\bin\Relativity.DataExchange.NUnit.Integration.dll"
[string]$OutFilePath     = "$TestResultsFolder\Output\api_shared_output.xml"
[string]$ResultFilePath  = "$TestResultsFolder\Result\api_shared_result.xml"

Set-Content -LiteralPath $RunTestBat -Value """$NUnitConsoleRunner"" ""$TestDll""  /out:""$OutFilePath"" /result:""$ResultFilePath""" -PassThru -Force

# UIAutomation Tests
[string]$RunTestBat      = "C:\$SharedFolderOnRemoteHost\RunUIAutomationTests.bat"
[string]$TestDll         = "C:\$SharedFolderOnRemoteHost\Source\Relativity.Desktop.Client.Legacy.Tests.UI\bin\Release\Relativity.Desktop.Client.Legacy.Tests.UI.dll"
[string]$OutFilePath     = "$TestResultsFolder\Output\UIAutomation_output.xml"
[string]$ResultFilePath  = "$TestResultsFolder\Result\UIAutomation_result.xml"

Set-Content -LiteralPath $RunTestBat -Value """$NUnitConsoleRunner"" ""$TestDll""  /out:""$OutFilePath"" /result:""$ResultFilePath""" -PassThru -Force


# Create bat to convert results from .xml to .html
Set-Content -LiteralPath "C:\$SharedFolderOnRemoteHost\ConvertTestReportsToHtml.bat" -value """$ReportNUnit"" ""$TestResultsFolder\Result"" ""$TestResultsFolder\Html""" -PassThru -Force