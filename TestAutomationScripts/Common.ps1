$ScriptDirectory = Split-Path $MyInvocation.MyCommand.Path -Parent
. (Join-Path $ScriptDirectory SetPaths.ps1)

function PrepareDataForAutomaticTests([string]$SourceFolder, [string]$DestinationFolder){
    
    if (!(Test-Path -LiteralPath $DestinationFolder)) { New-Item -Path $DestinationFolder -Force -ItemType Directory }
    if (!(Test-Path -LiteralPath "$DestinationFolder\import-api-rdc")) { New-Item -Path "$DestinationFolder\import-api-rdc" -Force -ItemType Directory }

    $DestinationFolder = "$DestinationFolder\import-api-rdc"

    # Remove old files
    if(Test-Path -LiteralPath "$DestinationFolder\Source") { Remove-Item -LiteralPath "$DestinationFolder\Source" -Recurse -Force }
    if(Test-Path -LiteralPath "$DestinationFolder\packages") { Remove-Item -LiteralPath "$DestinationFolder\packages" -Recurse -Force }

    # Sources and tests dll files
    $items = Get-ChildItem -LiteralPath "$SourceFolder\Source" -Directory -Depth 0
  
    foreach($item in $items){ 
        if(Test-Path -LiteralPath "$SourceFolder\Source\$item\bin") {
            New-Item -Path "$DestinationFolder\Source\$item" -ItemType Directory -Force
            Copy-Item -LiteralPath "$SourceFolder\Source\$item\bin" -Destination "$DestinationFolder\Source\$item" -Force -Recurse -PassThru
        }
    }

    # Test data, NUnit console, ...
    Copy-Item -LiteralPath "$SourceFolder\packages" -Destination "$DestinationFolder" -Force -Recurse -PassThru

    # Test Automation Scripts
    Copy-Item -LiteralPath "$SourceFolder\TestAutomationScripts" -Destination "$DestinationFolder" -Force -Recurse -PassThru
}


function CreateBatForTestExecution([ValidateSet(“Api-Export”,”Api-Import”,”Api-Shared”,"UITests","LoadTests")]$TestAssemblyName, [string]$TestClassName="", [string]$TestNameWithParameters="", [bool]$ToBeExecutedOnTestVM=$true){

    [string]$ParentFolderPath = ""
    [string]$NUnitConsoleRunnerPath = ""

    if ($ToBeExecutedOnTestVM) { 
        $ParentFolderPath = $RootFolderOnTestVM 
        $NUnitConsoleRunnerPath = $NUnitConsoleRunnerTestVM
    } else { 
        $ParentFolderPath = $SourcesDirectory 
        $NUnitConsoleRunnerPath = $NUnitConsoleRunner
    }

    [string]$outputBatFilePath = "$ParentFolderPath\$TestAssemblyName.bat"

    [string]$assemblyPath = ""
    [string]$folderName   = $IntegrationTestsReportDirName

     switch ($TestAssemblyName ) {

        “Api-Export” { $assemblyPath = "$ParentFolderPath\Source\Relativity.DataExchange.Export.NUnit.Integration\bin\Relativity.DataExchange.Export.NUnit.Integration.dll"}
        “Api-Import” { $assemblyPath = "$ParentFolderPath\Source\Relativity.DataExchange.Import.NUnit.Integration\bin\Relativity.DataExchange.Import.NUnit.Integration.dll"}
        “Api-Shared” { $assemblyPath = "$ParentFolderPath\Source\Relativity.DataExchange.NUnit.Integration\bin\Relativity.DataExchange.NUnit.Integration.dll"}
        “LoadTests”  { $assemblyPath = "$ParentFolderPath\Source\Relativity.DataExchange.Import.NUnit.LoadTests\bin\Relativity.DataExchange.Import.NUnit.LoadTests.dll"}
        “UITests”    { 
            $assemblyPath = "$ParentFolderPath\Source\Relativity.Desktop.Client.Legacy.Tests.UI\bin\Release\Relativity.Desktop.Client.Legacy.Tests.UI.dll"
            $folderName   = $UIAutomationTestsReportDirName
        }
    }

    [string]$BatFileContent = ""

    if($TestClassName -eq "") {
    
        # pathToNUnitConssole pathToAssembly /out:pathToOutputXml /result:pathToResultXml
        $BatFileContent = """$NUnitConsoleRunnerPath"" ""$assemblyPath"" /out:""$ParentFolderPath\$TestResultsFolderName\$folderName\$TestAssemblyName-Output.txt"" /result:""$ParentFolderPath\$TestResultsFolderName\$folderName\$TestAssemblyName-Result.xml"""

    } else {
        [string]$Namespace = [System.IO.Path]::GetFileNameWithoutExtension($assemblyPath)

        # pathToNUnitConssole --test=namespace.class.test(params) pathToAssembly /out:pathToOutputXml /result:pathToResultXml
        $BatFileContent = """$NUnitConsoleRunnerPath"" --test=$Namespace.$TestClassName.$TestNameWithParameters ""$assemblyPath"" /out:""$ParentFolderPath\$TestResultsFolderName\$folderName\$TestAssemblyName-Output.txt"" /result:""$ParentFolderPath\$TestResultsFolderName\$folderName\$TestAssemblyName-Result.xml"""
    }

    Set-Content -LiteralPath "$ParentFolderPath\Run_$TestAssemblyName.bat" -Value "$BatFileContent`npause" -PassThru -Force

}

function CreateFolderStructureForTestResult([bool]$ToBeExecutedOnTestVM=$true){
    if ($ToBeExecutedOnTestVM) { 
        $ParentFolderPath      = $RootFolderOnTestVM 
    } else { 
        $ParentFolderPath = $SourcesDirectory 
    }
    [string]$TestResultsFolder = "$ParentFolderPath\$TestResultsFolderName"
    if (Test-Path -LiteralPath "$TestResultsFolder") { Remove-Item -LiteralPath $TestResultsFolder -Recurse -Force }
    New-Item -Path $TestResultsFolder -ItemType Directory -Force
    New-Item -Path "$TestResultsFolder\$IntegrationTestsReportDirName" -ItemType Directory -Force
    New-Item -Path "$TestResultsFolder\$UnitTestsReportDirName" -ItemType Directory -Force
    New-Item -Path "$TestResultsFolder\$UIAutomationTestsReportDirName" -ItemType Directory -Force
    New-Item -Path "$TestResultsFolder\$LoadTestsReportDirName" -ItemType Directory -Force
   
}

function CreateBatToConvertResultsToHtml([bool]$ToBeExecutedOnTestVM=$true){
    
    [string]$ParentFolderPath      = ""
    [string]$ReportNUnitPath       = ""

    if ($ToBeExecutedOnTestVM) { 
        $ParentFolderPath      = $RootFolderOnTestVM 
        $ReportNUnitPath       = $ReportNUnitTestVM
    } else { 
        $ParentFolderPath = $SourcesDirectory 
        $ReportNUnitPath  = $ReportNUnit
    }

    [string]$batContent = """$ReportNUnitPath"" ""$ParentFolderPath\$TestResultsFolderName\$IntegrationTestsReportDirName"" ""$ParentFolderPath\$TestResultsFolderName\$IntegrationTestsReportDirName"""
    $batContent        += "`n""$ReportNUnitPath"" ""$ParentFolderPath\$TestResultsFolderName\$UnitTestsReportDirName"" ""$ParentFolderPath\$TestResultsFolderName\$UnitTestsReportDirName"""
    $batContent        += "`n""$ReportNUnitPath"" ""$ParentFolderPath\$TestResultsFolderName\$UIAutomationTestsReportDirName"" ""$ParentFolderPath\$TestResultsFolderName\$UIAutomationTestsReportDirName"""
    $batContent        += "`n""$ReportNUnitPath"" ""$ParentFolderPath\$TestResultsFolderName\$LoadTestsReportDirName"" ""$ParentFolderPath\$TestResultsFolderName\$LoadTestsReportDirName"""

    Set-Content -LiteralPath "$ParentFolderPath\Run_ConvertTestReportsToHtml.bat" -value $batContent -PassThru -Force
}