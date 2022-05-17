import groovy.json.JsonOutput 
library 'ProjectMayhem@v1'
library 'SlackHelpers@5.2.0-Trident'

// Execute tests on 'develop' branch automatically every Monday and Thursday at 1 AM
def cronString = env.BRANCH_NAME == 'develop' ? "0 1 * * 1,4" : ""

properties([
	buildDiscarder(logRotator(artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30')),
	parameters([
		choice(defaultValue: 'Release', choices: ["Release","Debug"], description: 'Build config', name: 'buildConfig'),
		choice(defaultValue: 'normal', choices: ["quiet", "minimal", "normal", "detailed", "diagnostic"], description: 'Build verbosity', name: 'buildVerbosity'),
		string(defaultValue: 'aio-whitesedge-eau', description: 'The template used to prepare hopper instance', name: 'hopperTemplate'),
		string(defaultValue: 'develop', description: 'Name of folder in bld-pkgs Packages Relativity', name: 'relativityInstallerSource'),
	]),
	pipelineTriggers([cron(cronString)])
])

testResultsPassed = 0
testResultsFailed = 0
testResultsSkipped = 0
sqlComparerTestsResultFailed = 0
numberOfLeaseRenewals = 3

def globalVmInfo = null
numberOfErrors = 0	
tools = null
Slack = null

summaryMessage = ""

timestamps
{
	node('role-build-agent||buildAgent')
	{
		try
		{
		
			stage ('Clean')
			{
				deleteDir()
			}	
			
			stage('Checkout')
			{
				checkout([
					$class: 'GitSCM',
					branches: scm.branches,
					doGenerateSubmoduleConfigurations: scm.doGenerateSubmoduleConfigurations,
					extensions: [[$class: 'CloneOption', noTags: false, shallow: false, reference: '']] + [[$class: 'CleanCheckout']],
					userRemoteConfigs: scm.userRemoteConfigs,
					])
				notifyBitbucket()
				tools = load 'Trident/Tools/Tools.groovy'
				Slack = load 'Trident/Tools/Slack.groovy'
			}

			stage('Build binaries')
			{
				echo "Building the binaries"
				output = powershell ".\\build.ps1 UpdateAssemblyInfo,Build,BuildSQLDataComparer -Configuration '${params.buildConfig}' -Verbosity '${params.buildVerbosity}' -ILMerge -Sign -Branch '${env.BRANCH_NAME}'"
				echo output
			}

			timeout(time: 11, unit: 'HOURS')
			{
				stage("Prepare hopper")
				{
					try
					{
						echo "Getting hopper for ${hopperTemplate}"
						globalVmInfo = tools.createHopperInstance(hopperTemplate, relativityInstallerSource)
                        tools.renewHopperInstanceLease(globalVmInfo, numberOfLeaseRenewals)
						
						try
						{   
							try
							{
								stage("Run tests for SqlComparer tool")
								{
									echo "Replacing variables"
									replaceTestVariables(globalVmInfo.Url, true)
									
									try
									{
										echo "Run tests for SqlComparer tool" 
										runTestsForSqlComparerTool(globalVmInfo.Url)
									}
									finally
									{
										echo "Get SqlComparer results"
										checkSqlComparerToolResults()
										
										if(sqlComparerTestsResultFailed > 0)
										{
											echo "SqlComparer detected one or more non identical databases"
											currentBuild.result = 'FAILED'
											summaryMessage = "Compare databases using SQL Comparer Tool finished with errors"
											throw new Exception("Compare databases using SQL Comparer Tool finished with errors")
										}
									}
								}
							}
							finally
							{	
								try
								{
									stage("Run load tests for MassImportImprovementsToggle On")
									{
										echo "Replacing variables"
										replaceTestVariables(globalVmInfo.Url, false)
								
										runLoadTests(globalVmInfo.Url, true)
									}
								}
								finally
								{
									stage("Run load tests for MassImportImprovementsToggle Off")
									{
										runLoadTests(globalVmInfo.Url, false)
									}
								}
							}
						}
						finally
						{ 
							stage("Retrieve test results")
							echo "Get test results"
							GetTestResults()
				
							echo "Test results report"
							createTestReport()
							
							echo "Publishing the build logs"
							archiveArtifacts artifacts: 'Logs/**/*.*'
								
							echo "Publishing the load tests report"
							archiveArtifacts artifacts: "TestReports/load-tests/**/*.*"	

							echo "Publishing deadlocks details"
							archiveArtifacts artifacts: "TestReports/SqlProfiling/**/*.*"	
							
							echo "Publishing sql comparer details"
							archiveArtifacts artifacts: "TestReports/SqlComparer/**/*.*"	
							
							echo "Publishing performance results"
							archiveArtifacts artifacts: "TestReports/PerformanceSummary.csv"	
							
							def int numberOfFailedTests = testResultsFailed
							if (numberOfFailedTests > 0)
							{
								echo "Failed tests count bigger than 0"
								currentBuild.result = 'FAILED'
								throw new Exception("One or more tests failed")
							}
						}
					}
					catch(err)
					{
						echo err.toString()
						numberOfErrors++
						echo "Number of errors: ${numberOfErrors}"
						currentBuild.result = 'FAILED'
						tools.transferHopper(globalVmInfo)
					}
				}
			}
		}
		finally
		{
			try
			{
				stage("Send slack and bitbucket notification")
				{
					def buildResult = prepareSummaryMessage(numberOfErrors, testResultsPassed, testResultsFailed, testResultsSkipped)
					currentBuild.result = buildResult

					notifyBitbucket()

					Slack.SendSlackNotification("Newest Relativity from '${relativityInstallerSource}'", "Trident loadtests", env.BRANCH_NAME, params.buildConfig, "load-tests", testResultsFailed, testResultsPassed, testResultsSkipped, summaryMessage)
				}
			}
            catch(err)
            {
                echo err.toString()
            }
		}
	}
}

//################################ functions ###################################################

def prepareSummaryMessage(int numberOfErrors, int testResultsPassed, int testResultsFailed, int testResultsSkipped)
{
	def buildResult = 'FAILED'
	
	if(numberOfErrors > 0)
	{
		if(summaryMessage == "")
		{
			summaryMessage = "Something went wrong"
		}
	}
	else
	{
		if(testResultsFailed > 0)
		{
			summaryMessage = "One or more tests finished with errors"
		}
		else 
		{
			if(testResultsPassed == 0 && testResultsFailed == 0 && testResultsSkipped == 0)
			{
				summaryMessage = "No test was executed"
			}
			else 
			{
				summaryMessage = "All tests passed"
				buildResult = 'SUCCESS'
			}
		}
	}
	
	return buildResult
}

def getPathToTestParametersFile()
{
	return ".\\Source\\Relativity.DataExchange.TestFramework\\Resources\\loadtests.json"
}

def runLoadTests(String vmUrl, Boolean massImportImprovementsToggleOn)
{
	echo "Running the load tests"
	
	String pathToJsonFile = getPathToTestParametersFile()
	def massImportImprovementsToggle = massImportImprovementsToggleOn ? "-MassImportImprovementsToggle" : ""
	def timeout = massImportImprovementsToggleOn  ? 2700000 : 4800000
	
	output = powershell ".\\build.ps1 LoadTests ${massImportImprovementsToggle} -ILMerge -TestTimeoutInMS ${timeout} -TestTarget '${(new URI(vmUrl)).getHost()}' -TestParametersFile '${pathToJsonFile}' -Branch '${env.BRANCH_NAME}'"
	echo output 								
}

def runTestsForSqlComparerTool(String vmUrl)
{
	echo "Run LoadTests for both MassImportImprovementsToggle values and run SqlComparer after each test"
	
	String pathToJsonFile = getPathToTestParametersFile()
	
	output = powershell ".\\build.ps1 LoadTestsForSqlComparer -ILMerge -TestTimeoutInMS 4800000 -TestTarget '${(new URI(vmUrl)).getHost()}' -TestParametersFile '${pathToJsonFile}' -Branch '${env.BRANCH_NAME}'"
	echo output
}

def checkSqlComparerToolResults()
{
	echo "Check SqlComparer tool results"
	def testResultOutputString = tools.runCommandWithOutput(".\\build.ps1 CheckSqlComparerToolResults")

	// Search for specific tokens within the response.
	echo "Extracting the loadtests result parameters for SqlComparer"
	def int sqlComparerTestsPassed = tools.extractValue("sqlComparerTestsResultPassed", testResultOutputString)
	def int sqlComparerTestsFailed = tools.extractValue("sqlComparerTestsResultFailed", testResultOutputString)
	
	echo "SqlComparer test passed: $sqlComparerTestsPassed"
	echo "SqlComparer test failed: $sqlComparerTestsFailed"
	
	sqlComparerTestsResultFailed += sqlComparerTestsFailed
}

def GetTestResults()
{
	echo "Retrieving results of load tests"
	def testResultOutputString = tools.runCommandWithOutput(".\\build.ps1 LoadTestResults")

	// Search for specific tokens within the response.
	echo "Extracting the loadtests result parameters"
	def int passed = tools.extractValue("testResultsPassed", testResultOutputString)
	def int failed = tools.extractValue("testResultsFailed", testResultOutputString)
	def int skipped = tools.extractValue("testResultsSkipped", testResultOutputString)
	echo "Extracted the loadtests result parameters"

	// Dump the individual test results
	echo "Test passed: $passed"
	echo "Test failed: $failed"
	echo "Test skipped: $skipped"
	
	// Now add to the final test results
	testResultsPassed += passed
	testResultsFailed += failed
	testResultsSkipped += skipped	
}

def replaceTestVariables(String vmUrl, Boolean sqlComparerEnabled)
{
	String pathToJsonFile = getPathToTestParametersFile()
	powershell ".\\build.ps1 CreateTemplateTestParametersFileForLoadTests -TestParametersFile '${pathToJsonFile}'"
	
	echo "replacing test variables"
	def sqlComparerEnabledInTests = sqlComparerEnabled ? "-SqlDataComparer" : ""
	output = powershell ".\\build.ps1 ReplaceTestVariables -TestTarget '${(new URI(vmUrl)).getHost()}' -TestParametersFile '${pathToJsonFile}' -SqlProfiling ${sqlComparerEnabledInTests}"
	echo output
}

def createTestReport()
{
	echo "Generating test report"
	powershell ".\\build.ps1 TestReports -Branch '${env.BRANCH_NAME}'"
}