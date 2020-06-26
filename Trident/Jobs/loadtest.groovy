import groovy.json.JsonOutput 
library 'ProjectMayhem@v1'
library 'SlackHelpers@5.2.0-Trident'

properties([
    buildDiscarder(logRotator(artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30')),
    parameters([
        choice(defaultValue: 'Release', choices: ["Release","Debug"], description: 'Build config', name: 'buildConfig'),
        choice(defaultValue: 'normal', choices: ["quiet", "minimal", "normal", "detailed", "diagnostic"], description: 'Build verbosity', name: 'buildVerbosity'),
        choice(defaultValue: 'aio-lanceleaf-ea', choices: ["aio-lanceleaf-ea"], description: 'The template used to prepare the hopper instance', name: 'hopperTemplate'),
		string(defaultValue: 'release-11.2-lanceleaf', description: 'Name of folder in bld-pkgs Packages Relativity', name: 'relativityInstallerSource'),
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
						
						echo "Replacing variables"
						replaceTestVariables(globalVmInfo.Url)
						
						try
						{        
							try{
								stage("Run load tests for MassImportImprovementsToggle On")
								runLoadTests(globalVmInfo.Url, true)
							}
							finally{
								
								stage("Run load tests for MassImportImprovementsToggle Off")
								runLoadTests(globalVmInfo.Url, false)
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
                    currentBuild.result = 'SUCCESS'
                }
                
                notifyBitbucket()
                
                Slack.SendSlackNotification("Newest Relativity from '${relativityInstallerSource}'", "Trident loadtests", env.BRANCH_NAME, params.buildConfig, "load-tests", testResultsFailed, testResultsPassed, testResultsSkipped, message)
            }
		}
	}
}

//################################ functions ###################################################

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

def replaceTestVariables(String vmUrl)
{
    String pathToJsonFile = getPathToTestParametersFile()
    powershell ".\\build.ps1 CreateTemplateTestParametersFileForLoadTests -TestParametersFile '${pathToJsonFile}'"
	echo "replacing test variables"
    output = powershell ".\\build.ps1 ReplaceTestVariables -TestTarget '${(new URI(vmUrl)).getHost()}' -TestParametersFile '${pathToJsonFile}'"
    echo output
	
	echo "set values for sql profiling"
	output = powershell ".\\build.ps1 SetVariablesForSqlProfiling -TestTarget '${(new URI(vmUrl)).getHost()}' -TestParametersFile '${pathToJsonFile}'"
    echo output
}

def createTestReport()
{
    echo "Generating test report"
    powershell ".\\build.ps1 TestReports -Branch '${env.BRANCH_NAME}'"
}