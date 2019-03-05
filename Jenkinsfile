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
sut = null

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

                stage('Install RAID')
                {
                    Scvmm = scvmm(this, sessionID)
                    sut = Scvmm.getServerFromPool()

                    parallel (
                        Deploy:
                        {
                            build = getBuildArtifactsPath(this, "Relativity", "develop", build, params.type, sessionID)
                            currentBuild.description = "Relativity: " + "develop" + " " + build

                            uploadEnvironmentFile(this, sut.name, build, "develop", params.type, "N/A", "N/A", getCookbooks(), 'fluidOn:1,cdonprem:1', knife, "N/A", "N/A", sessionID, true, false, false)
                            addRunlist(this, sessionID, sut.name, sut.domain, sut.ip, createRunList(true, false, false, false), knife, profile, eventHash, "", "")
                        },
                        ProvisionNodes:
                        {
                            Scvmm.createNodes(1, 60, '2', 'IL1-Tintri002-CD-Test-Node_r2.2', 'cd_node_svc', 'Jenkins_sa', true)
                            bootstrapPythonPackages(this, sessionID, [], [[packages: ['jeeves' : '5.3.3']]])
                        }
                    )
                }

                stage('Integration Tests')
                {
                    if(sut?.name)
                    {
                        withEnv([
                            "IAPI_INTEGRATION_RELATIVITYURL=https://${sut.name}.kcura.corp",
                            "IAPI_INTEGRATION_RELATIVITYRESTURL=https://${sut.name}.kcura.corp/relativity.rest/api",
                            "IAPI_INTEGRATION_RELATIVITYSERVICEURL=https://${sut.name}.kcura.corp/relativity.services",
                            "IAPI_INTEGRATION_RELATIVITYWEBAPIURL=https://${sut.name}.kcura.corp/relativitywebapi",
                            "IAPI_INTEGRATION_RELATIVITYUSERNAME=relativity.admin@kcura.com",
                            "IAPI_INTEGRATION_RELATIVITYPASSWORD=Test1234!",
                            "IAPI_INTEGRATION_SQLINSTANCENAME=${sut.name}.kcura.corp\\EDDSINSTANCE001",
                            "IAPI_INTEGRATION_SQLADMINUSERNAME=sa",
                            "IAPI_INTEGRATION_SQLADMINPASSWORD=P@ssw0rd@1"
                        ]) 
                        {
                            powershell ".\\build.ps1 -SkipBuild -IntegrationTests"
                        }
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
                    parallel([
                        SlackNotification: { sendCDSlackNotification(this, (sut?.name ?: ""), build, env.BRANCH_NAME, params.type, params.slackChannel, "", failedTests, passedTests, ignoredTests, env.BUILD_TAG) },
                        ChefCleanup: {
                            if(sut?.name)
                            {
                                bat "python -m jeeves.chef_functions -f delete_chef_artifacts -n ${sut.name} -r '$knife'"
                            }
                        }
                    ])
                }
            }
            catch (err)  // Just catch everything here, if reporting/cleanup is the only thing that failed, let's not fail out the pipeline.
            {
                echo err.toString()
            }
            finally
            {
                if(sut?.name)
                {
                    SaveVMs = false
                    // If we don't have a result, we didn't get to a test because somthing failed out earlier.
                    // If the result is FAILURE, a test failed.
                    if (!currentBuild.result || currentBuild.result == "FAILURE")
                    {
                        try
                        {
                            timeout(time: 30, unit: 'MINUTES')
                            {
                                user = input(message: 'Save the VMs?', ok: 'Save', submitter: 'JNK-Basic', submitterParameter: 'submitter')
                            }
                            SaveVMs = true
                            Scvmm.saveVMs(user)
                        }
                        // This throws an error if you click abort or let it time out /shrug
                        catch(err)
                        {
                            echo err.toString()
                        }
                    }
            
                    if (!SaveVMs)
                    {
                        Scvmm.deleteVMs()
                    }
                    deleteNodes(this, sessionID)
                }
            }
        }
    }
}