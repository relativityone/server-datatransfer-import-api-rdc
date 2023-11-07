import groovy.json.JsonOutput 
library 'ProjectMayhem@v1'
library 'SlackHelpers@5.2.0-Trident'

properties([
    buildDiscarder(logRotator(artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30')),
    parameters([
        choice(defaultValue: 'Release', choices: ["Release","Debug"], description: 'Build config', name: 'buildConfig'),
        choice(defaultValue: 'normal', choices: ["quiet", "minimal", "normal", "detailed", "diagnostic"], description: 'Build verbosity', name: 'buildVerbosity'),
        string(defaultValue: '', description: 'Environment to test against templates, example: reg-b.r1.kcura.com,reg-prod-update.r1.kcura.com,regression-a.r1.kcura.com,reg-prod-previous.r1.kcura.com,reg-zero.r1.kcura.com', name: 'environmentToTest'),
        string(defaultValue: 'RelativityOne Quick Start Template', description: 'Workspace template, example: RelativityOne Quick Start Template, zTemplate DLA Collation Primary [DO NOT DELETE]', name: 'workspaceTemplate')
    ])
])

testResultsPassed = 0
testResultsFailed = 0
testResultsSkipped = 0
testResultsFailedForsutTemplate = 0

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
				Slack = load 'Trident/Tools/Slack.groovy'
			}

			stage('Build binaries')
			{
				echo "Building the binaries"
				output = powershell ".\\build.ps1 UpdateAssemblyInfo,Build -Configuration '${params.buildConfig}' -Verbosity '${params.buildVerbosity}' -ILMerge -Sign -Branch '${env.BRANCH_NAME}'"
				echo output
			}
			
			try
			{  	
				stage("Replacing test variables")
				{
					echo "Replacing variables for ${environmentToTest}"
					replaceTestVariables(params.environmentToTest, "https://${environmentToTest}", params.workspaceTemplate)
				}
				
				stage("Import integration tests")
				{
					echo "Running Import integration tests for ${environmentToTest}"
					RunSelectedIntegrationTests(environmentToTest, 3)
				}
				
				stage("Export integration tests")
				{
					echo "Running Export integration tests for ${environmentToTest}"
					RunSelectedIntegrationTests(environmentToTest, 2)
				}
			}				
			finally
			{ 
				stage("Creating test report")
				{
					echo "Publishing the build logs"
					archiveArtifacts artifacts: 'Logs/**/*.*'
					
					echo "Test results report for ${environmentToTest}"
					createTestReport(environmentToTest)
						
					echo "Publishing the integration tests report"
					archiveArtifacts artifacts: "TestReports/${environmentToTest}/**/*.*"	
					
					echo "Get test results"
					GetTestResults(environmentToTest)
					
					def int numberOfFailedTests = testResultsFailedForsutTemplate
					if (numberOfFailedTests > 0)
					{
						echo "Failed tests count for '${environmentToTest}' bigger than 0"
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
			inCompatibleEnvironments = inCompatibleEnvironments + environmentToTest + " "
		}			
		finally{
			stage('Send slack and bitbucket notification')
			{
				def int numberOfFailedTests = testResultsFailed
				if(numberOfErrors > 0 || numberOfFailedTests > 0)
				{
					message = "Something went wrong"
					currentBuild.result = 'FAILED'
				}
				else
				{
					if(testResultsPassed == 0 && testResultsFailed == 0 && testResultsSkipped == 0)
					{
						message = "No test was executed"
						currentBuild.result = 'FAILED'
					}
					else 
					{
						message = "All tests passed"
						currentBuild.result = 'SUCCESS'
					}
				}
				
				notifyBitbucket()
				Slack.SendSlackNotification("Relativity on '${params.environmentToTest}'", "Regression tests", '${env.BRANCH_NAME}', params.buildType, "regression", testResultsFailed, testResultsPassed, testResultsSkipped, message)
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
    output = powershell ".\\build.ps1 IntegrationTestsRegression -ILMerge -TestTimeoutInMS 900000 -TestReportFolderName '${sutTemplate}' -TestParametersFile '${pathToJsonFile}' -Branch '${env.BRANCH_NAME}'"
    echo output 								
}

def RunSelectedIntegrationTests(String sutTemplate, int testMode)
{
	// 1: Api-Shared tests
	// 2: Api-Export tests
	// 3: Api-Import tests
	
	String pathToJsonFile = getPathToTestParametersFile(sutTemplate)
    echo "Running the integration tests"
	
	timeout(time: 3, unit: 'HOURS') 
	{				
		if(testMode == 1)
		{
			output = powershell ".\\build.ps1 IntegrationTestsRegressionShared -ILMerge -TestTimeoutInMS 900000 -TestReportFolderName '${sutTemplate}' -TestParametersFile '${pathToJsonFile}' -Branch '${env.BRANCH_NAME}'"
			echo output 
		}
		
		if(testMode == 2)
		{
			output = powershell ".\\build.ps1 IntegrationTestsRegressionExport -ILMerge -TestTimeoutInMS 900000 -TestReportFolderName '${sutTemplate}' -TestParametersFile '${pathToJsonFile}' -Branch '${env.BRANCH_NAME}'"
			echo output 
		}
		
		if(testMode == 3)
		{
			output = powershell ".\\build.ps1 IntegrationTestsRegressionImport -ILMerge -TestTimeoutInMS 900000 -TestReportFolderName '${sutTemplate}' -TestParametersFile '${pathToJsonFile}' -Branch '${env.BRANCH_NAME}'"
			echo output 
		}
	}
}

def GetTestResults(String sutTemplate)
{
	echo "Retrieving results of integration tests : $sutTemplate"
	def testResultOutputString = tools.runCommandWithOutput(".\\build.ps1 IntegrationTestResults -TestReportFolderName '${sutTemplate}' ")

	// Search for specific tokens within the response.
	echo "Extracting the $sutTemplate-test result parameters"
	def int passed = tools.extractValue("testResultsPassed", testResultOutputString).toInteger()
	def int failed = tools.extractValue("testResultsFailed", testResultOutputString).toInteger()
	def int skipped = tools.extractValue("testResultsSkipped", testResultOutputString).toInteger()
	echo "Extracted the $sutTemplate-test result parameters"

	// Dump the individual test results
	echo "$sutTemplate-test passed: $passed"
	echo "$sutTemplate-test failed: $failed"
	echo "$sutTemplate-test skipped: $skipped"
	
	// Now add to the final test results
	testResultsPassed += passed
	testResultsFailed += failed
	testResultsSkipped += skipped	
	
    testResultsFailedForsutTemplate = failed
	
}

def replaceTestVariables(String sutTemplate, String url, String workspaceTemplate)
{
    String pathToJsonFile = getPathToTestParametersFile(sutTemplate)
    powershell ".\\build.ps1 CreateTemplateTestParametersFileForRegressionTests -TestParametersFile '${pathToJsonFile}'"
	
	echo "replacing test variables in ${sutTemplate}"
    output = powershell ".\\build.ps1 ReplaceTestVariablesRegression -TestTarget '${url}' -WorkspaceTemplate '${workspaceTemplate}' -TestParametersFile '${pathToJsonFile}'"
    echo output
	
	output = powershell ".\\build.ps1 ReplaceTestVariables -TestTarget '${url}' -TestParametersFile '${pathToJsonFile}'"
    echo output
}

def createTestReport(String sutTemplate)
{
    echo "Generating test report"
    powershell ".\\build.ps1 TestReports -TestReportFolderName '${sutTemplate}' -Branch '${env.BRANCH_NAME}'"
}
