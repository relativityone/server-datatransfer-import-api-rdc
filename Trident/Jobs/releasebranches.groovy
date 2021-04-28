import groovy.json.JsonOutput 
library 'ProjectMayhem@v1'
library 'SlackHelpers@5.2.0-Trident'

// Execute tests on 'release-*' branches automatically every Monday, Wednesday and Friday at 1 AM
def currentBranchName = env.BRANCH_NAME
def cronString = currentBranchName.startsWith('release-') ? "0 1 * * 1,3,5" : ""

properties([
    buildDiscarder(logRotator(artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30')),
    parameters([
        choice(defaultValue: 'Release', choices: ["Release","Debug"], description: 'Build config', name: 'buildConfig'),
        choice(defaultValue: 'normal', choices: ["quiet", "minimal", "normal", "detailed", "diagnostic"], description: 'Build verbosity', name: 'buildVerbosity'),
    ]),
	pipelineTriggers([cron(cronString)])
])

testResultsPassed = 0
testResultsFailed = 0
testResultsSkipped = 0
testResultsFailedForsutTemplate = 0
numberOfLeaseRenewals = 1

def globalVmInfo = null
numberOfErrors = 0
tools = null
Slack = null

summaryMessage = ""
testedVersions = ""
inCompatibleEnvironments = ""	

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
			
			def hopperTemplate = GetHopperTemplate(env.BRANCH_NAME)
			def relativityBranchesForTest = GetRelativityBranches(env.BRANCH_NAME)
			
			String[] templates = relativityBranchesForTest.tokenize(',')

			for(sutTemplate in templates)
			{
				timeout(time: 3, unit: 'HOURS')
				{
					try
					{
						stage("Prepare hopper ${hopperTemplate}")
						{
							echo "Getting hopper for ${sutTemplate}"
							globalVmInfo = tools.createHopperInstance(hopperTemplate, sutTemplate)
                            tools.renewHopperInstanceLease(globalVmInfo, numberOfLeaseRenewals)
						}
						
						stage("Run tests against Relativity ${sutTemplate}")
						{
							testedVersions = testedVersions == "" ? "'${sutTemplate}'" : "${testedVersions} and '${sutTemplate}'"
						
							echo "Replacing variables for ${sutTemplate}"
							replaceTestVariables(sutTemplate, globalVmInfo.Url)
							
							try
							{                        
								echo "Running integration tests for ${sutTemplate}"
								runTests(sutTemplate, "IntegrationTests")
							}
							finally
							{ 
								try
								{
									echo "Running unit tests for ${sutTemplate}"
									runTests(sutTemplate, "UnitTests")
								}
								finally
								{
									testResultsFailedForsutTemplate = 0
									
									echo "Get test results"
									GetTestResults(sutTemplate, "UnitTestResults")
									GetTestResults(sutTemplate, "IntegrationTestResults")
									
									echo "Test results report for ${sutTemplate}"
									createTestReport(sutTemplate)
									
									echo "Publishing the build logs"
									archiveArtifacts artifacts: 'Logs/**/*.*'
										
									echo "Publishing the tests report"
									archiveArtifacts artifacts: "TestReports/${sutTemplate}/**/*.*"		

									def int numberOfFailedTests = testResultsFailedForsutTemplate
									if (numberOfFailedTests > 0)
									{
										echo "Failed tests count for '${sutTemplate}' bigger than 0"
										currentBuild.result = 'FAILED'
										throw new Exception("One or more tests failed")
									}
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
						inCompatibleEnvironments = inCompatibleEnvironments + sutTemplate + " "
                        tools.transferHopper(globalVmInfo)
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
                currentBuild.result = prepareSummaryMessage(numberOfErrors, testResultsPassed, testResultsFailed, testResultsSkipped)
                notifyBitbucket()
                
                Slack.SendSlackNotification("Tested against Relativity from ${testedVersions}", "Test Relativity release branches", env.BRANCH_NAME, params.buildConfig, "release-branches", testResultsFailed, testResultsPassed, testResultsSkipped, "${env.BUILD_TAG} - ${summaryMessage}")
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
			summaryMessage = inCompatibleEnvironments == "" ? "Something went wrong" : "Something went wrong with the following versions: ${inCompatibleEnvironments}"
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

def getPathToTestParametersFile(String sutTemplate)
{
    return ".\\Source\\Relativity.DataExchange.TestFramework\\Resources\\${sutTemplate}.json"
}

def runTests(String sutTemplate, String task)
{
    String pathToJsonFile = getPathToTestParametersFile(sutTemplate)
    echo "Running the ${task}"
    output = powershell ".\\build.ps1 ${task} -ILMerge -TestTimeoutInMS 900000 -TestReportFolderName '${sutTemplate}' -TestParametersFile '${pathToJsonFile}'"
    echo output 								
}

def GetTestResults(String sutTemplate, String task)
{
	echo "Retrieving results of ${task} : $sutTemplate"
	def testResultOutputString = tools.runCommandWithOutput(".\\build.ps1 ${task} -TestReportFolderName '${sutTemplate}' ")

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
    
    testResultsFailedForsutTemplate += failed
	
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

def GetReleaseVersionName(String branchName)
{
	// To enable test branch 'TestReleaseBranch' working
	def usedBranchName = branchName == "TestReleaseBranch" ? "release-1.10-lanceleaf" : branchName
	
	def versionName = usedBranchName.split('-')
	echo "Branch name: '${branchName}'"
	echo "Version name: '${versionName[2]}'"
	
	return versionName[2]
}

def GetHopperTemplate(String branchName)
{
	def hopperImage = ''
	def versionName = GetReleaseVersionName(branchName)
	
	// This section should be updated when new release appears
	// Hopper images should be reviewed and updated to latest available
	switch(versionName) {
		case "juniper":
			hopperImage = "aio-juniper-2"
			break
		case "lanceleaf":
			hopperImage = "aio-lanceleaf-0"
			break
		case "mayapple":
			hopperImage = "aio-mayapple-0"
			break
		case "ninebark":
			hopperImage = "aio-ninebark-ea"
			break
		default:
			echo "Define the hopper image which should be used in tests on branch '${branchName}'"
			currentBuild.result = 'FAILED' 
			throw new Exception("Build failed, because hopper image is not defined in script")
			break
	}
	
	echo "Hopper template for branch '${branchName}' is '${hopperImage}'"
	
	return hopperImage
}

def GetRelativityBranches(String branchName)
{
	echo "Get Relativity branches for tests"

	def relativityBranchName = GetReleaseVersionName(branchName)
	def outputString = tools.runCommandWithOutput(".\\build.ps1 GetRelativityBranchesForTests -ReleasedVersionName '${relativityBranchName}' ")
	def branchesNames = tools.extractValue("relativityBranchesForTests", outputString)
	
	echo "Relativity branches folders for tests: '${branchesNames}'"
	
	return branchesNames
}