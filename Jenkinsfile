#!groovy

library 'PipelineTools@RMT-9.3.2'
library 'SCVMMHelpers@3.3.0'
library 'SlackHelpers@3.0.0'

def buildTypeCoicesStr = 'DEV\nGOLD'
def isNewBuild = (env.BRANCH_NAME.contains('roland-pipeline-test')) 

properties([
    [$class: 'BuildDiscarderProperty', strategy: [$class: 'LogRotator', artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30']],
    parameters([
        choice(defaultValue: 'DEV',choices: buildTypeCoicesStr, description: 'The type of build to execute, you can choose "GOLD" if you intent to push this package for release', name: 'buildType'),
        choice(defaultValue: 'Release', choices: ["Release","Debug"], description: 'Build config', name: 'buildConfig'),
        choice(defaultValue: 'normal', choices: ["quiet", "minimal", "normal", "detailed", "diagnostic"], description: 'Build verbosity', name: 'buildVerbosity'),
        string(defaultValue: '#import-api-rdc-build', description: 'Slack Channel title where to report the pipeline results', name: 'slackChannel'),
        booleanParam(defaultValue: true, description: "Enable or disable running unit tests", name: 'runUnitTests'),
        booleanParam(defaultValue: true, description: "Enable or disable running integration tests", name: 'runIntegrationTests'),
        booleanParam(defaultValue: true, description: "Enable or disable creating a code coverage report", name: 'createCodeCoverageReport'),
        choice(defaultValue: 'hyperv', choices: ["hyperv"], description: 'The test environment used for integration tests and code coverage', name: 'testEnvironment'),
        booleanParam(defaultValue: true, description: "Enable or disable publishing NuGet packages", name: 'publishPackages'),
        booleanParam(defaultValue: false, description: "Enable or disable force publishing an RDC MSI NuGet package", name: 'forcePublishRdcPackage')
    ])
])

// Do not modify.
def profile = createProfile(true, false, false, false)
def knife = 'C:\\Python27\\Lib\\site-packages\\jeeves\\knife.rb'
def sessionID = System.currentTimeMillis().toString()
def eventHash = java.security.MessageDigest.getInstance("MD5").digest(env.JOB_NAME.bytes).encodeHex().toString()
def buildVersion = ""
def packageVersion = ""
def int testResultsPassed = 0
def int testResultsFailed = 0
def int testResultsSkipped = 0
build = params.build

timestamps
{
    timeout(time: 3, unit: 'HOURS')
    {
        node("PolandBuild")
        {
            try
            {
                stage('Checkout')
                {
                    checkout scm
                    notifyBitbucket()
                }

                try
                {
                    stage('Clean')
                    {
						steps{
							output = powershell ".\\build.ps1 Clean -Verbosity '${params.buildVerbosity}' -Branch '${env.BRANCH_NAME}'"
                            echo output
						}
                    }

                    stage('Retrieve semantic versions')
                    { 
						echo "Retrieving the semantic versions"
					    if(isNewBuild)
						{
							echo "Using new build strategy"
							def outputString = runCommandWithOutput(".\\Get-ReleaseVersion.ps1 '${env.BRANCH_NAME}'")
							currentBuild.displayName = "$outputString"
							buildVersion = outputString
						}
						else
						{
							def outputString = runCommandWithOutput(".\\build.ps1 SemanticVersions -BuildUrl ${BUILD_URL} -Verbosity '${params.buildVerbosity}' -Branch '${env.BRANCH_NAME}'")
							echo "Retrieved the semantic versions"
							buildVersion = extractValue("buildVersion", outputString)
							packageVersion = extractValue("packageVersion", outputString)
							echo "Build URL: ${BUILD_URL}"
							echo "Build version: $buildVersion"
							echo "Package version: $packageVersion"
							currentBuild.displayName = buildVersion
						}
                    }

                    stage('Build binaries')
                    {
                        echo "Building the binaries for version $buildVersion"
                        output = powershell ".\\build.ps1 UpdateAssemblyInfo,Build -Configuration '${params.buildConfig}' -Verbosity '${params.buildVerbosity}' -ILMerge -Sign -Branch '${env.BRANCH_NAME}'"
                        echo output
                    }

                    stage('Build install packages')
                    {
                        echo "Building the installers for version $buildVersion"
                        output = powershell ".\\build.ps1 BuildInstallPackages -Configuration '${params.buildConfig}' -Verbosity '${params.buildVerbosity}' -Sign -Branch '${env.BRANCH_NAME}'"
                        echo output
                    }

                    //stage('Extended code analysis')
                    //{
                    //    echo "Extending code analysis"
                    //    output = powershell ".\\build.ps1 ExtendedCodeAnalysis -Verbosity '${params.buildVerbosity}' -Branch '${env.BRANCH_NAME}'"
                   //     echo output
                   // }

                    try
                    {
                        if (params.runUnitTests)
                        {
                            stage('Run unit tests')
                            {
                                echo "Running the unit tests"
                                output = powershell ".\\build.ps1 UnitTests -ILMerge -Branch '${env.BRANCH_NAME}'"
                                echo output
                            }
                        }

                        //if (params.runIntegrationTests)
                       // {
                        //    stage('Run integration tests')
                       //     {
                       //         echo "Running the integration tests"
                       //         output = powershell ".\\build.ps1 IntegrationTests -ILMerge -TestEnvironment $params.testEnvironment -Branch '${env.BRANCH_NAME}'"
                        //        echo output
                        //    }
                       // }
                    }
                    finally
                    {
                        if (params.runUnitTests || params.runIntegrationTests)
                        {
                            stage('Test results report')
                            {
                                echo "Generating test report"
                                powershell ".\\build.ps1 TestReports -Branch '${env.BRANCH_NAME}'"
                            }
                        }

                        if (params.runUnitTests || params.runIntegrationTests)
                        {
                            stage('Retrieve test results')
                            {
                                def taskCandidates = []
                                if (params.runUnitTests)
                                {
                                    taskCandidates.add("UnitTestResults")
                                }

                                if (params.runIntegrationTests)
                                {
                                   // taskCandidates.add("IntegrationTestResults")
                                }

                                taskCandidates.eachWithIndex { task, index ->
                                    def testDescription = ""
                                    switch (index)
                                    {
                                        case 0:
                                            testDescription = "unit"
                                            break

                                        case 1:
                                            testDescription = "integration"
                                            break

                                        default:
                                            throw new Exception("The test result type $index is not mapped.")
                                    }

                                    // Let the build script retrieve the unit test result values.
                                    echo "Retrieving the $testDescription-test results"
                                    def testResultOutputString = runCommandWithOutput(".\\build.ps1 ${task} -Verbosity '${params.buildVerbosity}' -Branch '${env.BRANCH_NAME}'")
                                    echo "Retrieved the $testDescription-test results"

                                    // Search for specific tokens within the response.
                                    echo "Extracting the $testDescription-test result parameters"
                                    def int passed = extractValue("testResultsPassed", testResultOutputString)
                                    def int failed = extractValue("testResultsFailed", testResultOutputString)
                                    def int skipped = extractValue("testResultsSkipped", testResultOutputString)
                                    echo "Extracted the $testDescription-test result parameters"

                                    // Dump the individual test results
                                    echo "$testDescription-test passed: $passed"
                                    echo "$testDescription-test failed: $failed"
                                    echo "$testDescription-test skipped: $skipped"
                                    
                                    // Now add to the final test results
                                    testResultsPassed += passed
                                    testResultsFailed += failed
                                    testResultsSkipped += skipped
                                }

                                // Dump the final test results
                                echo "Total passed: $testResultsPassed"
                                echo "Total failed: $testResultsFailed"
                                echo "Total skipped: $testResultsSkipped"
                            }
                        }
                    }

                    if (params.createCodeCoverageReport)
                    {
                        stage('Code coverage report')
                        {
                            echo "Creating a code coverage report"
                            output = powershell ".\\build.ps1 CodeCoverageReport -TestEnvironment $params.testEnvironment -Branch '${env.BRANCH_NAME}'"
                            echo output
                        }
                    }
                    else
                    {
                        echo "Skip creating code coverage report due to configuration parameter."
                    }

                    stage ('Create NuGet packages')
                    {
                        echo "Build number: ${currentBuild.number}"
                        echo "Building all SDK and RDC packages"
                        powershell ".\\build.ps1 BuildPackages -Branch '${env.BRANCH_NAME}' -BuildNumber '${currentBuild.number}'"
                    }

                    if (params.publishPackages)
                    {
                        stage ('Publish packages to proget')
                        {
                            if (env.BRANCH_NAME == 'master' || params.forcePublishRdcPackage)
                            {
                                echo "Publishing the SDK and RDC package(s)"
                                powershell ".\\build.ps1 PublishPackages -Branch '${env.BRANCH_NAME}'"
                            }
                            else
                            {
                                echo "Publishing only the SDK package(s)"
                                powershell ".\\build.ps1 PublishPackages -SkipPublishRdcPackage -Branch '${env.BRANCH_NAME}'"
                            }
                        }
                    }
                    else
                    {
                        echo "Skip publishing package(s) due to configuration parameter."
                    }

                    stage('Publish build artifacts')
                    {
                        echo "Publishing build artifacts"
                        output = powershell ".\\build.ps1 PublishBuildArtifacts -Version '$buildVersion' -Branch '${env.BRANCH_NAME}'"
                        echo output
                    }

                    currentBuild.result = 'SUCCESS'
                }
                finally
                {
                    echo "Publishing the build logs"
                    archiveArtifacts artifacts: 'Logs/**/*.*'
                    if (params.runUnitTests)
                    {
                        echo "Publishing the unit tests report"
                        archiveArtifacts artifacts: 'TestReports/unit-tests/**/*.*'
                    }

                    if (params.runIntegrationTests)
                    {
                        echo "Publishing the integration tests report"
                        archiveArtifacts artifacts: 'TestReports/integration-tests/**/*.*'
                    }

                    if (params.createCodeCoverageReport)
                    {
                        echo "Publishing the code coverage report"
                        archiveArtifacts artifacts: 'TestReports/code-coverage/**/*.*'
                    }
                } 
            }
            catch(err)
            {
                echo err.toString()
                currentBuild.result = 'FAILED' 
                if (env.BRANCH_NAME == 'develop' || env.BRANCH_NAME == 'master')
                {
                    sendEmailAboutFailureToTeam()
                }
                else
                {
                    sendEmailAboutFailureToAuthor() 
                }
            }
            finally
            {
                try
                {
                    stage('Reporting and Cleanup')
                    {
                        parallel(
                            SlackNotification: 
                            {
                                def script = this
                                def String serverUnderTestName = ""
                                def String version = buildVersion
                                def String branch = env.BRANCH_NAME
                                def String buildType = params.buildType
                                def String slackChannel = params.slackChannel
                                def String email = "slack_svc@relativity.com"
                                def int numberOfFailedTests = testResultsFailed
                                def int numberOfPassedTests = testResultsPassed
                                def int numberOfSkippedTests = testResultsSkipped
                                def String message = env.BUILD_TAG
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
                                sendCDSlackNotification(script, serverUnderTestName, version, branch, buildType, slackChannel, email, numberOfFailedTests, numberOfPassedTests, numberOfSkippedTests, message)
                            },
                            // StashNotifier second call, passes currentBuild.result to BitBucket as build status 
                            BitBucketNotification:
                            { 
                                notifyBitbucket()
                            }
                        )
                    }
                }
                catch (err)
                {
                    // Just catch everything here, if reporting/cleanup is the only thing that failed, let's not fail out the pipeline.
                    echo err.toString()
                }
            }
        }
    }
}

def sendEmailAboutFailureToAuthor()
{
    def commiterDetails = bat (
        script: 'git --no-pager show -s --format=%ae', 
        returnStdout: true
    )

    def recipients = extractCommiterEmail(commiterDetails)
    sendEmailAboutFailure(recipients)
}

def sendEmailAboutFailureToTeam()
{
   def recipients = 'thegoodthebadandtheugly@relativity.com'
   sendEmailAboutFailure(recipients)
}

def sendEmailAboutFailure(String recipients)
{
    echo "Sending ${env.BRANCH_NAME} build failure to $recipients"
    def subject = "${env.JOB_NAME} - Build ${env.BUILD_DISPLAY_NAME} - Failed! On branch ${env.BRANCH_NAME}"
    def body = """${env.JOB_NAME} - Build - Failed:

Check console output at ${env.BUILD_URL} to view the results."""
    sendEmail(body, subject, recipients)
}

def sendEmail(String body, String subject, String recipients)
{
    emailext attachLog: true, attachmentsPattern: 'TestResults/**/*.*', body: body, subject: subject, to: recipients
}

def extractCommiterEmail(details) {
    
    def arr = details.tokenize('\n')
    def email = arr[2].trim()
    return email
}

def extractValue(String value, String output)
{
    if (value == null || value.isEmpty())
    {
        return ""
    }

    def matcher = output =~ "$value=(.*)"
    $result =  matcher[0][0].split("=")[1]
    matcher = null
    return $result
}

def runCommandWithOutput(String command)
{
    def outputString = powershell(returnStdout: true, script: command).trim()
    if (!outputString) 
    {
        error("$command command returned empty output!")
    }

    return outputString
}