import groovy.json.JsonOutput 
library 'ProjectMayhem@v1'
library 'SlackHelpers@5.2.0-Trident'

properties([
    buildDiscarder(logRotator(artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30')),
    parameters([
        choice(defaultValue: 'Release', choices: ["Release","Debug"], description: 'Build config', name: 'buildConfig'),
        choice(defaultValue: 'normal', choices: ["quiet", "minimal", "normal", "detailed", "diagnostic"], description: 'Build verbosity', name: 'buildVerbosity'),
        string(defaultValue: '#ugly_test', description: 'Slack Channel title where to report the pipeline results', name: 'slackChannel'),
        string(defaultValue: 'aio-goatsbeard-3,aio-blazingstar-3,aio-larkspur-3,aio-foxglove-3,aio-juniper-0', description: 'Comma separated list of SUT templates', name: 'temlatesStr')
    ]),
    pipelineTriggers([cron("H 22 * * *")])
])

testResultsPassed = 0
testResultsFailed = 0
testResultsSkipped = 0

String[] templates = params.temlatesStr.tokenize(',')

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

			for(sutTemplate in templates)
			{
				timeout(time: 3, unit: 'HOURS')
				{
					stage("Run integration tests against ${sutTemplate}")
					{
						try
						{
							echo "Getting hopper for ${sutTemplate}"
							globalVmInfo = tools.createHopperInstance(sutTemplate, null)
							
							echo "Replacing variables for ${sutTemplate}"
							replaceTestVariables(sutTemplate, globalVmInfo.Url)
							
							try
							{                        
								echo "Running integration tests for ${sutTemplate}"
								runIntegrationTests(sutTemplate)
							}
							finally
							{ 
								echo "Test results report for ${sutTemplate}"
								createTestReport(sutTemplate)
								
								echo "Publishing the build logs"
								archiveArtifacts artifacts: 'Logs/**/*.*'
									
								echo "Publishing the integration tests report"
								archiveArtifacts artifacts: "TestReports/${sutTemplate}/**/*.*"							
							}
						}
						catch(err)
						{
							echo err.toString()
							numberOfErrors++
							echo "Number of errors: ${numberOfErrors}"
							currentBuild.result = 'FAILED'
							inCompatibleEnvironments = inCompatibleEnvironments + sutTemplate + " "
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
		}
		finally{
			stage("Send slack and bitbucket notification")
			{
				def script = this
				def String serverUnderTestName = temlatesStr
				def String version = "Trident nightly"
				def String branch = "Trident"
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
				sendCDSlackNotification(script, serverUnderTestName, version, branch, buildType, slackChannel, email, ['tests': ['passed': numberOfPassedTests, 'failed': numberOfFailedTests, 'skipped': numberOfSkippedTests]], message, "CD" )
			}
		}
	}
}

//################################ functions ###################################################

def getPathToTestParametersFile(String sutTemplate)
{
    return ".\\Source\\Relativity.DataExchange.TestFramework\\Resources\\${sutTemplate}.json"
}

def runIntegrationTests(String sutTemplate)
{
    String pathToJsonFile = getPathToTestParametersFile(sutTemplate)
    echo "Running the integration tests"
    output = powershell ".\\build.ps1 IntegrationTestsNightly -ILMerge -TestTimeoutInMS 900000 -TestReportFolderName '${sutTemplate}' -TestParametersFile '${pathToJsonFile}' -Branch 'Trident'"
    echo output 								
	
	echo "Retrieving results of integration tests : $sutTemplate"
	def testResultOutputString = tools.runCommandWithOutput(".\\build.ps1 IntegrationTestResults -TestReportFolderName '${sutTemplate}' ")

	// Search for specific tokens within the response.
	echo "Extracting the $sutTemplate-test result parameters"
	def int passed = tools.extractValue("testResultsPassed", testResultOutputString)
	def int failed = tools.extractValue("testResultsFailed", testResultOutputString)
	def int skipped = tools.extractValue("testResultsSkipped", testResultOutputString)
	echo "Extracted the $sutTemplate-test result parameters"

	// Dump the individual test results
	echo "$sutTemplate-test passed: $passed"
	echo "$sutTemplate-test failed: $failed"
	echo "$sutTemplate-test skipped: $skipped"
	
	// Now add to the final test results
	testResultsPassed += passed
	testResultsFailed += failed
	testResultsSkipped += skipped	
}

def replaceTestVariables(String sutTemplate, String vmUrl)
{
    String pathToJsonFile = getPathToTestParametersFile(sutTemplate)
    powershell ".\\build.ps1 CreateTemplateTestParametersFile -TestParametersFile '${pathToJsonFile}'"
	echo "replacing test variables in ${sutTemplate}"
    output = powershell ".\\build.ps1 ReplaceTestVariables -TestTarget '${(new URI(vmUrl)).getHost()}' -TestParametersFile '${pathToJsonFile}'"
    echo output
}

def createTestReport(String sutTemplate)
{
    echo "Generating test report"
    powershell ".\\build.ps1 TestReports -TestReportFolderName '${sutTemplate}' -Branch '${env.BRANCH_NAME}'"
}