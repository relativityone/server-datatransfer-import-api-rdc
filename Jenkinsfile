#!groovy

library 'PipelineTools@RMT-9.3.2'
library 'SCVMMHelpers@3.3.0'
library 'SlackHelpers@3.0.0'

properties([
    [$class: 'BuildDiscarderProperty', strategy: [$class: 'LogRotator', artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30']],
    parameters([
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
def NUnit = nunit()
def buildVersion = ""
def packageVersion = ""
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

                stage('Clean')
                {
                    output = powershell ".\\build.ps1 Clean -ForceDeleteTools -ForceDeletePackages -ForceDeleteArtifacts -Verbosity '${params.buildVerbosity}'"
                    echo output
                }

                stage('Retrieve semantic versions')
                { 
                    echo "Retrieving the semantic versions"
                    def outputString = runCommandWithOutput(".\\build.ps1 SemanticVersions -BuildUrl ${BUILD_URL} -Verbosity '${params.buildVerbosity}'"
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
                    try
                    {
                        // Wrapped in a try/finally to ensure the logs are archived.
                        echo "Building version $buildVersion"
                        output = powershell ".\\build.ps1 Build -Configuration '${params.buildConfig}' -Version '$buildVersion' -Verbosity '${params.buildVerbosity}'"
                        echo output
                    }
                    finally
                    {
                        archiveArtifacts artifacts: 'Logs/**/*.*'
                    }
                }

                stage('Extended code analysis')
                {
                    try
                    {
                        // Wrapped in a try/finally to ensure the logs are archived.
                        echo "Extending code analysis"
                        output = powershell ".\\build.ps1 ExtendedCodeAnalysis -Verbosity '${params.buildVerbosity}'"
                        echo output
                    }
                    finally
                    {
                        archiveArtifacts artifacts: 'Logs/**/*.*'
                    }
                }

                stage('Run unit tests')
                {
                    try
                    {
                        // Wrapped in a try/finally to ensure the results are archived.
                        output = powershell ".\\build.ps1 UnitTests"
                        echo output
                    }
                    finally
                    {
                        powershell ".\\build.ps1 GenerateTestReport"
                        archiveArtifacts artifacts: 'TestResults/**/*.*'
                    }
                }

                stage('Digitally sign binaries')
                {
                    output = powershell ".\\build.ps1 DigitallySign -Verbosity '${params.buildVerbosity}'"
                    echo output
                }

                stage('Publish build artifacts')
                {
                    try
                    {
                        // Wrapped in a try/finally to ensure the logs are archived.
                        output = powershell ".\\build.ps1 PublishBuildArtifacts -Version '$buildVersion' -Branch '${env.BRANCH_NAME}'"
                        echo output
                    }
                    finally
                    {
                        archiveArtifacts artifacts: 'Logs/**/*.*'
                    }
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
        finally
        {
            try
            {
                NUnit.publish(this, sessionID)
            }
            catch (err)
            {
                echo err.toString()
            }
        }

        stage('Reporting and Cleanup')
        {
            try
            {
                //def(passedTests, failedTests, ignoredTests) = getTestCounts(this)
                //node(sessionID)
                //{
                //    sendCDSlackNotification(this, "", build, env.BRANCH_NAME, params.type, params.slackChannel, "", failedTests, passedTests, ignoredTests, env.BUILD_TAG)
                //}
            }
            catch (err)  // Just catch everything here, if reporting/cleanup is the only thing that failed, let's not fail out the pipeline.
            {
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