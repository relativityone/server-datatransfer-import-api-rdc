#!groovy

library 'PipelineTools@RMT-9.3.2'
library 'SCVMMHelpers@3.3.0'
library 'SlackHelpers@3.0.0'

properties([
    [$class: 'BuildDiscarderProperty', strategy: [$class: 'LogRotator', artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30']],
    parameters([
        choice(choices: 'DEV\nGOLD', description: 'The type of Relativity build to install', name: 'type'),
        string(defaultValue: '', description: 'The Relativity build to install', name: 'build'),
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

                stage('Compile')
                {
                    version = powershell(returnStdout:true, script: "(.\\Version\\Increment-ProductVersion.ps1 -Version (Get-Content .\\Version\\version.txt) -Force).ToString()")
                    version = version.trim()
                    echo "Building version $version"

                    powershell ".\\build.ps1 -AssemblyVersion '$version'"
                }

                stage ('Unit Tests')
                {
                    powershell ".\\build.ps1 -SkipBuild -UnitTests"
                }

                stage ('Code Analysis')
                {
                    powershell ".\\build.ps1 -ExtendedCodeAnalysis"
                }

                stage ('Publish to bld-pkgs')
                {
                    $productName = "IAPI";
                    
                    withCredentials([usernamePassword(credentialsId: 'jenkins_packages_svc', passwordVariable: 'BLDPKGSPASSWORD', usernameVariable: 'BLDPKGSUSERNAME')])
                    {
                        powershell "echo $env:BLDPKGSUSERNAME"
                        //def sourcePath = "${env.WORKSPACE}\\publishPackage.zip";
                        //def destinationPath = "\\\\BLD-PKGS.kcura.corp\\Packages\\DataTransfer\\${ProductName}\\${env.BRANCH_NAME}\\${buildNumber}"
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
                def(passedTests, failedTests, ignoredTests) = getTestCounts(this)
                node(sessionID)
                {
                    sendCDSlackNotification(this, "", build, env.BRANCH_NAME, params.type, params.slackChannel, "", failedTests, passedTests, ignoredTests, env.BUILD_TAG)
                }
            }
            catch (err)  // Just catch everything here, if reporting/cleanup is the only thing that failed, let's not fail out the pipeline.
            {
                echo err.toString()
            }
        }
    }
}