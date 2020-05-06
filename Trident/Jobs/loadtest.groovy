import groovy.json.JsonOutput 
library 'ProjectMayhem@v1'
library 'SlackHelpers@5.2.0-Trident'

properties([
    buildDiscarder(logRotator(artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30')),
    parameters([
        choice(defaultValue: 'Release', choices: ["Release","Debug"], description: 'Build config', name: 'buildConfig'),
        choice(defaultValue: 'normal', choices: ["quiet", "minimal", "normal", "detailed", "diagnostic"], description: 'Build verbosity', name: 'buildVerbosity'),
        string(defaultValue: '#ugly_test', description: 'Slack Channel title where to report the pipeline results', name: 'slackChannel'),
        choice(defaultValue: 'release-11.1-juniper-1', choices: ["release-11.1-juniper-1"], description: 'The test environment used for integration tests and code coverage', name: 'testEnvironment'),
    ]),
    pipelineTriggers([cron("H 22 * * *")])
])

testResultsPassed = 0
testResultsFailed = 0
testResultsSkipped = 0

String[] templates = params.testEnvironment.tokenize(',')

def globalVmInfo = null
numberOfErrors = 0
def String inCompatibleEnvironments = ""	
tools = null

timestamps
{
	node('role-build-agent||buildAgent')
	{
		try{
		
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
			}

			stage('Build binaries')
			{
				echo "Building the binaries"
				output = powershell ".\\build.ps1 UpdateAssemblyInfo,Build -Configuration '${params.buildConfig}' -Verbosity '${params.buildVerbosity}' -ILMerge -Sign -Branch '${env.BRANCH_NAME}'"
				echo output
			}

			timeout(time: 8, unit: 'HOURS')
			{
				stage("Prepare hopper")
				{
					try
					{
						echo "Getting hopper for ${testEnvironment}"
						globalVmInfo = tools.createHopperInstance(testEnvironment, "release-11.1-juniper-1")
						
						echo "Replacing variables for ${testEnvironment}"
						replaceTestVariables(testEnvironment, globalVmInfo.Url)
						
						stage("Run tests against ${testEnvironment}")
						{
							try
							{                        
								echo "Running tests for ${testEnvironment}"
								runLoadTests(testEnvironment)
							}
							finally
							{ 
								echo "Get test results for ${testEnvironment}"
								GetTestResults(testEnvironment)
					
								echo "Test results report for ${testEnvironment}"
								createTestReport(testEnvironment)
								
								echo "Publishing the build logs"
								archiveArtifacts artifacts: 'Logs/**/*.*'
									
								echo "Publishing the load tests report"
								archiveArtifacts artifacts: "TestReports/${testEnvironment}/**/*.*"	

								echo "Publishing deadlocks details"
								archiveArtifacts artifacts: "TestReports/SqlProfiling/**/*.*"	
								
								def int numberOfFailedTests = testResultsFailed
								if (numberOfFailedTests > 0)
								{
									echo "Failed tests count bigger than 0"
									currentBuild.result = 'FAILED'
									throw new Exception("One or more tests failed")
								}
							}
						}
					}
					catch(err)
					{
						echo err.toString()
						numberOfErrors++
						echo "Number of errors: ${numberOfErrors}"
						currentBuild.result = 'FAILED'
						inCompatibleEnvironments = inCompatibleEnvironments + testEnvironment + " "
					}
					finally
					{
						if(globalVmInfo != null) 
						{
							tools.deleteHopperInstance(globalVmInfo.Id)
						}	
					}
				}
			}
		}
		finally{
			stage("Send slack and bitbucket notification")
			{
				def script = this
				def String serverUnderTestName = testEnvironment
				def String version = "Trident loadtests"
				def String branch = env.BRANCH_NAME
				def String buildType = params.buildConfig
				def String slackChannel = params.slackChannel
				def String email = "slack_svc@relativity.com"
				def int numberOfFailedTests = testResultsFailed
				def int numberOfPassedTests = testResultsPassed
				def int numberOfSkippedTests = testResultsSkipped
				def String message = ""
				if(numberOfErrors > 0 || numberOfFailedTests > 0)
				{
					message = "Something went wrong with the following environments : "
					message = message + inCompatibleEnvironments
					currentBuild.result = 'FAILED'
				}
				else
				{
                    currentBuild.result = 'SUCCESS'
				}
                notifyBitbucket()
				echo "*************************************************" +
					"\n" +
					"\n" + "sendCDSlackNotification Parameters: " +
					"\n" +
					"\n" + "script: " + script +
					"\n" + "serverUnderTestName: " + serverUnderTestName +
					"\n" + "version: " + version +
					"\n" + "branch: " + branch +
					"\n" + "buildType: " + buildType +
					"\n" + "slackChannel: " + slackChannel +
					"\n" + "email: " + email +
					"\n" + "numberOfFailedTests: " + numberOfFailedTests +
					"\n" + "numberOfPassedTests: " + numberOfPassedTests +
					"\n" + "numberOfSkippedTests: " + numberOfSkippedTests +
					"\n" + "message: " + message +
					"\n" +
					"\n*************************************************"
				try
				{
					sendCDSlackNotification(script, serverUnderTestName, version, branch, buildType, slackChannel, email, ['tests': ['passed': numberOfPassedTests, 'failed': numberOfFailedTests, 'skipped': numberOfSkippedTests]], message, "CD" )
				}
				catch(err)
				{
					echo "Send slack notification failed"
					echo err.toString()
				}
			}
		}
	}
}

//################################ functions ###################################################

def getPathToTestParametersFile(String testEnvironment)
{
    return ".\\Source\\Relativity.DataExchange.TestFramework\\Resources\\${testEnvironment}.json"
}

def runLoadTests(String testEnvironment)
{
    String pathToJsonFile = getPathToTestParametersFile(testEnvironment)
    echo "Running the load tests"
    output = powershell ".\\build.ps1 LoadTests -ILMerge -TestTimeoutInMS 900000 -TestReportFolderName '${testEnvironment}' -TestParametersFile '${pathToJsonFile}' -Branch '${env.BRANCH_NAME}'"
    echo output 								
}

def GetTestResults(String testEnvironment)
{
	echo "Retrieving results of load tests : $testEnvironment"
	def testResultOutputString = tools.runCommandWithOutput(".\\build.ps1 LoadTestResults -TestReportFolderName '${testEnvironment}' ")

	// Search for specific tokens within the response.
	echo "Extracting the $testEnvironment-test result parameters"
	def int passed = tools.extractValue("testResultsPassed", testResultOutputString)
	def int failed = tools.extractValue("testResultsFailed", testResultOutputString)
	def int skipped = tools.extractValue("testResultsSkipped", testResultOutputString)
	echo "Extracted the $testEnvironment-test result parameters"

	// Dump the individual test results
	echo "$testEnvironment-test passed: $passed"
	echo "$testEnvironment-test failed: $failed"
	echo "$testEnvironment-test skipped: $skipped"
	
	// Now add to the final test results
	testResultsPassed += passed
	testResultsFailed += failed
	testResultsSkipped += skipped	
}

def replaceTestVariables(String testEnvironment, String vmUrl)
{
    String pathToJsonFile = getPathToTestParametersFile(testEnvironment)
    powershell ".\\build.ps1 CreateTemplateTestParametersFileForLoadTests -TestParametersFile '${pathToJsonFile}'"
	echo "replacing test variables in ${testEnvironment}"
    output = powershell ".\\build.ps1 ReplaceTestVariables -TestTarget '${(new URI(vmUrl)).getHost()}' -TestParametersFile '${pathToJsonFile}'"
    echo output
	
	echo "set values for sql profiling"
	output = powershell ".\\build.ps1 SetVariablesForSqlProfiling -TestTarget '${(new URI(vmUrl)).getHost()}' -TestParametersFile '${pathToJsonFile}'"
    echo output
}

def createTestReport(String testEnvironment)
{
    echo "Generating test report"
    powershell ".\\build.ps1 TestReports -TestReportFolderName '${testEnvironment}' -Branch '${env.BRANCH_NAME}'"
}