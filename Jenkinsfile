#!groovy

library 'PipelineTools@RMT-9.3.2'
library 'SCVMMHelpers@3.3.0'
library 'SlackHelpers@3.0.0'

properties([
    [$class: 'BuildDiscarderProperty', strategy: [$class: 'LogRotator', artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30']],
    parameters([
        choice(defaultValue: 'Release', choices: ["Release","Debug"], description: 'Build config', name: 'buildConfig'),
        string(defaultValue: '#import-api-rdc-build', description: 'Slack Channel title where to report the pipeline results', name: 'slackChannel')
    ])
])

// Do not modify.
def profile = createProfile(true, false, false, false)
def knife = 'C:\\Python27\\Lib\\site-packages\\jeeves\\knife.rb'
def sessionID = System.currentTimeMillis().toString()
def eventHash = java.security.MessageDigest.getInstance("MD5").digest(env.JOB_NAME.bytes).encodeHex().toString()
def NUnit = nunit()
build = params.build

timestamps
{
    timeout(time: 3, unit: 'HOURS')
    {
        try
        {
            node("PolandBuild")
            {
                stage ('Checkout')
                {
                    checkout scm
                }

                stage('Clean')
                {
                    output = powershell ".\\build.ps1 -Target 'Clean' -Verbosity 'normal'"
                    echo output
                }

                stage('Compile')
                {
                    version = powershell(returnStdout:true, script: "(.\\Version\\Increment-ProductVersion.ps1 -Version (Get-Content .\\Version\\version.txt) -Force).ToString()")
                    version = version.trim()
                    echo "Building version $version"
                    output = powershell ".\\build.ps1 -AssemblyVersion '$version' -Configuration '${params.buildConfig} -ExtendedCodeAnalysis -ForceDeleteTools -ForceDeletePackages -Verbosity 'normal'"
                    echo output
                }

                stage ('Unit Tests')
                {
                    output = powershell ".\\build.ps1 -SkipBuild -UnitTests"
                    echo output
                }

                stage ('Publish to bld-pkgs')
                {
                    $productName = "IAPI";                    
                    withCredentials([usernamePassword(credentialsId: 'jenkins_packages_svc', passwordVariable: 'BLDPKGSPASSWORD', usernameVariable: 'BLDPKGSUSERNAME')])
                    {
                        powershell "echo $env:BLDPKGSUSERNAME"
                        //def sourcePath = "${env.WORKSPACE}\\publishPackage.zip";
                        //def destinationPath = "\\\\BLD-PKGS.kcura.corp\\Packages\\DataTransfer\\Import-API-RDC\\${env.BRANCH_NAME}\\${buildNumber}"
                        //
                        //powershell """
                        //    net use \\\\bld-pkgs.kcura.corp\\Packages\\DataTransfer\\$ProductName "$BLDPKGSPASSWORD" /user:kcura\\$BLDPKGSUSERNAME 
                        //        mkdir ${destinationPath} -Force
                        //        Copy-Item $sourcePath ${destinationPath} -recurse -force
                        //    net use \\\\bld-pkgs.kcura.corp\\Packages\\DataTransfer\\$ProductName /DELETE /Y
                        //"""
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

def sendEmailAboutFailure()
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
   // TODO: Add attachments via 'attachmentsPattern'
   emailext attachLog: true, body: body, subject: subject, to: recipients
}

def extractCommiterEmail(details) {
    
   def arr = details.tokenize('\n')
   def email = arr[2].trim()
   return email
}