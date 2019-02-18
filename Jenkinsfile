#!groovy

library 'PipelineTools@RMT-9.3.2'
library 'SCVMMHelpers@3.3.0'
library 'SlackHelpers@3.0.0'

properties([
    [$class: 'BuildDiscarderProperty', strategy: [$class: 'LogRotator', artifactDaysToKeepStr: '7', artifactNumToKeepStr: '30', daysToKeepStr: '7', numToKeepStr: '30']],
    parameters([
        choice(choices: 'DEV\nGOLD', description: 'The type of Relativity build to install', name: 'type'),
        string(defaultValue: '', description: 'The Relativity build to install', name: 'build'),
        string(defaultValue: '#slackchannelnamehere', description: 'Slack Channel title where to report the pipeline results', name: 'slackChannel')
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
                stage ('Checkout and Version')
                {
                    //checkout source code here

                    //call versioning script here
                }

                stage ('Compile Solutions')
                {
                    // call the build script here
                }

                stage ('Build package')
                {
                    // call the script to build the package here
                }

                stage('Install RAID')
                {
                    Scvmm = scvmm(this, sessionID)
                    sut = Scvmm.getServerFromPool()

                    parallel (
                        Deploy:
                        {
                            build = getBuildArtifactsPath(this, "Relativity", env.BRANCH_NAME, build, params.type, sessionID)
                            currentBuild.description = "Relativity: " + env.BRANCH_NAME + " " + build

                            uploadEnvironmentFile(this, sut.name, build, env.BRANCH_NAME, params.type, "N/A", "N/A", getCookbooks(), 'fluidOn:1,cdonprem:1', knife, "N/A", "N/A", sessionID, true, false, false)
                            addRunlist(this, sessionID, sut.name, sut.domain, sut.ip, createRunList(true, false, false, false), knife, profile, eventHash, "", "")
                        },
                        ProvisionNodes:
                        {
                            Scvmm.createNodes(1, 60, '2', 'IL1-Tintri002-CD-Test-Node_r2.2', 'cd_node_svc', 'Jenkins_sa', true)
                            bootstrapPythonPackages(this, sessionID, [], [[packages: ['jeeves' : '5.3.3']]])
                        }
                    )
                }

                stage('Tests')
                {
                    // call your tests here
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
                        },
                        ReportToSQL: {
                            withCredentials([usernamePassword(credentialsId: 'BuildMetrics', passwordVariable: 'SQLPassword', usernameVariable: 'SQLUsername')])
                            {
                                //Send the pipeline results and some metadata to the reporting database
                                checkout scm
                                output = powershell(returnStdout: true, script: ".\\utilities\\sql_publish.ps1 -SQLUsername $SQLUsername -SQLPassword $SQLPassword -RMTJobID $env.BUILD_NUMBER -PipelineName $env.JOB_NAME -RelativityBranch $env.BRANCH_NAME -Status $currentBuild.result -BuildURL $env.BUILD_URL -BuildNumber ${build} -StartTime $currentBuild.startTimeInMillis")
                                echo output
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