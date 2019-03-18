#!groovy

library 'PipelineTools@RMT-9.3.2'
library 'SCVMMHelpers@3.3.0'
library 'SlackHelpers@3.0.0'

def buildTypeCoicesStr = (env.BRANCH_NAME in ["master"]) ? 'GOLD\nDEV' : 'DEV\nGOLD'

properties([
    [$class: 'BuildDiscarderProperty', strategy: [$class: 'LogRotator', artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30']],
    parameters([
        choice(choices: buildTypeCoicesStr, description: 'The type of build to execute', name: 'buildType'),
        choice(defaultValue: 'Release', choices: ["Release","Debug"], description: 'Build config', name: 'buildConfig'),
        choice(defaultValue: 'normal', choices: ["quiet", "minimal", "normal", "detailed", "diagnostic"], description: 'Build verbosity', name: 'buildVerbosity'),
        string(defaultValue: '#import-api-rdc-build', description: 'Slack Channel title where to report the pipeline results', name: 'slackChannel')
    ])
])

// Do not modify.
def profile = createProfile(true, false, false, false)
def knife = 'C:\\Python27\\Lib\\site-packages\\jeeves\\knife.rb'
def sessionID = System.currentTimeMillis().toString()
def eventHash = java.security.MessageDigest.getInstance("MD5").digest(env.JOB_NAME.bytes).encodeHex().toString()
def buildVersion = ""
def packageVersion = ""
def testResultsPassed = 0
def testResultsFailed = 0
def testResultsSkipped = 0
build = params.build

timestamps
{
    timeout(time: 3, unit: 'HOURS')
    {
        try
        {
            node("PolandBuild")
            {
                stage('Checkout')
                {
                    checkout scm
                }

                try
                {
                    stage('Clean')
                    {
                        output = powershell ".\\build.ps1 Clean -ForceDeleteTools -ForceDeletePackages -ForceDeleteArtifacts -Verbosity '${params.buildVerbosity}'"
                        echo output
                    }

                    stage('Retrieve semantic versions')
                    { 
                        echo "Retrieving the semantic versions"
                        def outputString = runCommandWithOutput(".\\build.ps1 SemanticVersions -BuildUrl ${BUILD_URL} -Verbosity '${params.buildVerbosity}'")
                        echo "Retrieved the semantic versions"
                        buildVersion = extractValue("buildVersion", outputString)
                        packageVersion = extractValue("packageVersion", outputString)
                        echo "Build URL: ${BUILD_URL}"
                        echo "Build version: $buildVersion"
                        echo "Package version: $packageVersion"
                        currentBuild.displayName = buildVersion
                    }

                    stage('Build')
                    {
                        echo "Building version $buildVersion"
                        output = powershell ".\\build.ps1 Build -Configuration '${params.buildConfig}' -Version '$buildVersion' -Verbosity '${params.buildVerbosity}'"
                        echo output
                    }

                    stage('Extended code analysis')
                    {
                        echo "Extending code analysis"
                        output = powershell ".\\build.ps1 ExtendedCodeAnalysis -Verbosity '${params.buildVerbosity}'"
                        echo output
                    }

                    stage('Run unit tests')
                    {
                        try
                        {
                            // Wrapped in a try/finally to ensure the test results are generated.
                            echo "Running the unit tests"
                            output = powershell ".\\build.ps1 UnitTests"
                            echo output
                        }
                        finally
                        {
                            echo "Generating unit test results"
                            powershell ".\\build.ps1 GenerateTestReport"
                        }
                    }

                    stage('Retrieve unit test results')
                    {
                        // Let the build script retrieve the values.
                        echo "Retrieving the unit test results"
                        def outputString = runCommandWithOutput(".\\build.ps1 UnitTestResults -Verbosity '${params.buildVerbosity}'")
                        echo "Retrieved the unit test results"

                        // Search for specific tokens within the response.
                        echo "Extracting the unit test result parameters"
                        testResultsPassed = extractValue("testResultsPassed", outputString)
                        testResultsFailed = extractValue("testResultsFailed", outputString)
                        testResultsSkipped = extractValue("testResultsSkipped", outputString)
                        echo "Extracted the unit test result parameters"

                        // Dump the test results
                        echo "Total passed: $testResultsPassed"
                        echo "Total failed: $testResultsFailed"
                        echo "Total skipped: $testResultsSkipped"
                    }

                    stage('Digitally sign binaries')
                    {
                        output = powershell ".\\build.ps1 DigitallySign -Verbosity '${params.buildVerbosity}'"
                        echo output
                    }

                    stage ('Publish packages to proget')
                    {
                        powershell ".\\build.ps1 PublishPackages -PackageVersion '$packageVersion' -Branch '${env.BRANCH_NAME}'"
                    }

                    stage('Publish build artifacts')
                    {
                        output = powershell ".\\build.ps1 PublishBuildArtifacts -Version '$buildVersion' -Branch '${env.BRANCH_NAME}'"
                        echo output
                    }
                }
                finally
                {
                    archiveArtifacts artifacts: 'Logs/**/*.*'
                    archiveArtifacts artifacts: 'TestResults/**/*.*'
                }                    
            }
        }
        catch(err)
        {
            echo err.toString()
            currentBuild.result = "FAILURE"
            if (env.BRANCH_NAME == 'develop' || env.BRANCH_NAME == 'master')
            {
               sendEmailAboutFailureToTeam()
            }
            else
            {
               sendEmailAboutFailureToAuthor() 
            }
        }

        stage('Reporting and Cleanup')
        {
            try
            {
                sendCDSlackNotification(this, "", build, env.BRANCH_NAME, params.buildType, params.slackChannel, "", testResultsFailed, testResultsPassed, testResultsSkipped, env.BUILD_TAG)
            }
            catch (err)
            {
                // Just catch everything here, if reporting/cleanup is the only thing that failed, let's not fail out the pipeline.
                echo err.toString()
            }
        }
    }
}

def sendEmailAboutFailureToAuthor()
{
    def commiterDetails = bat (
        script: 'git --no-pager show -s --format=%%ae', 
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