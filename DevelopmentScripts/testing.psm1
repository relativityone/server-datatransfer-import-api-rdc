# This module contains functions and variables related to setting test environment, executing tests and retrieving test results

Function Invoke-SetTestParameters {
    param(
        [bool] $SkipIntegrationTests,
        [String] $TestParametersFile,
        [String] $TestEnvironment
    )

    [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SKIPINTEGRATIONTESTS", $SkipIntegrationTests, "Process")
    if ($TestParametersFile) {
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_TEST_JSON_FILE", $TestParametersFile , "Process")
    }

    if ($TestEnvironment) {
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_TEST_ENV", $TestEnvironment , "Process")
    }
}

Function Invoke-IntegrationTests {
    param(
        [String] $TestCategoryFilter
    )

    $SolutionFile = $MasterSolution
    if ($ILMerge) {
        $SolutionFile = $MasterILMergeSolution
    }

    Invoke-SetTestParameters -SkipIntegrationTests $false -TestParametersFile $TestParametersFile -TestEnvironment $TestEnvironment
    exec { & $NunitExe $SolutionFile `
            "--labels=All" `
            "--agents=$NumberOfProcessors" `
            "--skipnontestassemblies" `
            "--timeout=$TestTimeoutInMS" `
            "--result=$IntegrationTestsResultXmlFile" `
            "--out=$IntegrationTestsOutputFile" `
            $testCategoryFilter `
    } -errorMessage "There was an error running the integration tests."
}

Function Remove-EmptyLogFile {
    param(
        [String] $LogFile
    )

    if (!$LogFile) {
        Throw "You must specify a non-null path to remove the empty logfile. Check to make sure the path value or variable passed to this method is valid."
    }

    # Remove the error log when none exist.
    if (Test-Path $LogFile -PathType Leaf) {
        if ((Get-Item $LogFile).length -eq 0) {
            Remove-Item $LogFile
        }
    }
}

Function Write-TestResultsOutput {
    param(
        [string] $FolderWithTestResults
    )

    if (!$FolderWithTestResults) { 
        Throw "You must specify a non-null path to retrieve the test results XML file. Check to make sure the path value or variable passed to this method is valid."
    }

    $TestResultFiles = Get-ChildItem -Path $FolderWithTestResults -Include "*.xml" -Recurse -Force

    [int]$passed,$failed,$skipped = 0

    foreach($TestResultsXmlFile in $TestResultFiles) {

        if (-Not (Test-Path $TestResultsXmlFile.FullName -PathType Leaf)) {
            Throw "The test results cannot be retrieved because the Xml tests file '" + $TestResultsXmlFile.FullName + "' doesn't exist."
        }

        $xml = [xml] (Get-Content $TestResultsXmlFile)
        $passed   += [convert]::ToInt32($xml.'test-run'.passed)
        $failed   += [convert]::ToInt32($xml.'test-run'.failed)
        $skipped  += [convert]::ToInt32($xml.'test-run'.skipped)
    }

    # So Jenkins can get the results
    Write-Output "testResultsPassed=$passed"
    Write-Output "testResultsFailed=$failed"
    Write-Output "testResultsSkipped=$skipped"
}