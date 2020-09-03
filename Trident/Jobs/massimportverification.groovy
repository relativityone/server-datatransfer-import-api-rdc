import groovy.json.JsonOutput 
library 'ProjectMayhem@v1'
library 'SlackHelpers@5.2.0-Trident'

properties([
	buildDiscarder(logRotator(artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30')),
	parameters([
		choice(defaultValue: 'Release', choices: ["Release","Debug"], description: 'Build config', name: 'buildConfig'),
		choice(defaultValue: 'normal', choices: ["quiet", "minimal", "normal", "detailed", "diagnostic"], description: 'Build verbosity', name: 'buildVerbosity'),
		choice(defaultValue: 'lanceleafAA1', choices: ["lanceleafAA1"], description: 'The template used to prepare hopper instance', name: 'hopperTemplate'),
		string(defaultValue: 'develop', description: 'Name of folder in bld-pkgs Packages Relativity', name: 'relativityInstallerSource'),
	])
])

testResultsPassed = 0
testResultsFailed = 0
testResultsSkipped = 0

def globalVmInfo = null
numberOfErrors = 0	
tools = null
Slack = null

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

			timeout(time: 11, unit: 'HOURS')
			{
				stage("Prepare hopper")
				{
					try
					{
						echo "Getting hopper for ${hopperTemplate}"
						globalVmInfo = tools.createHopperInstance(hopperTemplate, relativityInstallerSource)
						
						try
						{        
							try
							{
								stage('Workspace with non default collation MassImportImprovementsToggle On and DataGrid disabled')
								{
									runIntegrationTests(globalVmInfo.Url, true, false, true)
								}
							}
							finally
							{
								try
								{
									stage('MassImportImprovementsToggle On and DataGrid enabled')
									{
										runIntegrationTests(globalVmInfo.Url, true, true, false)
									}
								}
								finally
								{
									try
									{
										stage('MassImportImprovementsToggle On and DataGrid disabled')
										{
											runIntegrationTests(globalVmInfo.Url, true, false, false)
										}
									}
									finally
									{
										try 
										{
											stage('MassImportImprovementsToggle Off and DataGrid enabled')
											{
												runIntegrationTests(globalVmInfo.Url, false, true, false)
											}
										}
										finally
										{
											stage('MassImportImprovementsToggle Off and DataGrid disabled')
											{
												runIntegrationTests(globalVmInfo.Url, false, false, false)
											}
										}
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
							archiveArtifacts artifacts: "TestReports/integration-tests/**/*.*"	
							
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

				Slack.SendSlackNotification("Newest Relativity from '${relativityInstallerSource}'", "Complex cases tests", env.BRANCH_NAME, params.buildConfig, "mass-import-verification", testResultsFailed, testResultsPassed, testResultsSkipped, message)
			}
		}
	}
}

//################################ functions ###################################################

def getPathToTestParametersFile()
{
	echo "Get path to json file"
	return ".\\Source\\Relativity.DataExchange.TestFramework\\Resources\\develop.json"
}

def runIntegrationTests(String vmUrl, Boolean massImportImprovementsToggleOn, Boolean enableDataGrid, Boolean workspaceWithNonDefaultCollation)
{
	String pathToJsonFile = getPathToTestParametersFile()

	echo "Running tests for:"
	echo "'MassImportImprovementsToggle' = ${massImportImprovementsToggleOn}"
	echo "'DataGrid' enabled = ${enableDataGrid}"

	def massImportImprovementsToggle = massImportImprovementsToggleOn ? "-MassImportImprovementsToggle" : ""
	def dataGridEnabled = enableDataGrid ? "-EnableDataGrid" : ""
	def testWorkspaceWithNonDefaultCollation = workspaceWithNonDefaultCollation ? "-TestOnWorkspaceWithNonDefaultCollation" : ""
	
	echo "Replace test variables"
	replaceTestVariables(vmUrl, enableDataGrid, workspaceWithNonDefaultCollation)

	echo "Run tests"
	output = powershell ".\\build.ps1 IntegrationTestsForMassImportImprovementsToggle ${massImportImprovementsToggle} ${dataGridEnabled} ${testWorkspaceWithNonDefaultCollation} -ILMerge -TestTarget '${(new URI(vmUrl)).getHost()}' -TestParametersFile '${pathToJsonFile}' -Branch '${env.BRANCH_NAME}'"
	echo output 								
}

def GetTestResults()
{
	echo "Retrieving results"
	def testResultOutputString = tools.runCommandWithOutput(".\\build.ps1 IntegrationTestResults")

	// Search for specific tokens within the response.
	echo "Extracting the tests result parameters"
	def int passed = tools.extractValue("testResultsPassed", testResultOutputString)
	def int failed = tools.extractValue("testResultsFailed", testResultOutputString)
	def int skipped = tools.extractValue("testResultsSkipped", testResultOutputString)
	echo "Extracted the tests result parameters"

	// Dump the individual test results
	echo "Test passed: $passed"
	echo "Test failed: $failed"
	echo "Test skipped: $skipped"

	// Now add to the final test results
	testResultsPassed += passed
	testResultsFailed += failed
	testResultsSkipped += skipped	
}

def replaceTestVariables(String vmUrl, Boolean enableDataGrid, Boolean workspaceWithNonDefaultCollation)
{
	String pathToJsonFile = getPathToTestParametersFile()
	powershell ".\\build.ps1 CreateTemplateTestParametersFile -TestParametersFile '${pathToJsonFile}'"
	
	echo "Replacing test variables"
	def dataGridEnabled = enableDataGrid ? "-EnableDataGrid" : ""
	def testWorkspaceWithNonDefaultCollation = workspaceWithNonDefaultCollation ? "-TestOnWorkspaceWithNonDefaultCollation" : ""

	output = powershell ".\\build.ps1 ReplaceTestVariables ${dataGridEnabled} ${testWorkspaceWithNonDefaultCollation} -TestTarget '${(new URI(vmUrl)).getHost()}' -TestParametersFile '${pathToJsonFile}'"
    echo output
}

def createTestReport()
{
	echo "Generating test report"
	powershell ".\\build.ps1 TestReports -Branch '${env.BRANCH_NAME}'"
}