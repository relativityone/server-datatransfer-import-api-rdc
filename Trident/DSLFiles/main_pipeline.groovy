folder('TheGoodTheBadTheUgly') {
  description('folder for the team : The Good The Bad and the Ugly')
}

folder('TheGoodTheBadTheUgly/main_pipeline') {
  description('builds import API and RDC')
    properties {
        folderLibraries {
            libraries {
                libraryConfiguration {
                    name('Import API and RDC')
                        defaultVersion('v1')
                        allowVersionOverride(true)
                        includeInChangesets(false)
                        implicit(true)
                        retriever {
                            modernSCM {
                                scm {
                                    git {
                                        remote('ssh://git@git.kcura.com:7999/dtx/import-api-rdc.git')
                                        credentialsId('bitbucket-repo-key')
                                    }
                                }
                            }
                        }
                    }
            }
        }
    } 
}

multibranchPipelineJob('TheGoodTheBadTheUgly/main_pipeline/main_build') {
  displayName('Main build')
  factory {
        workflowBranchProjectFactory {
            scriptPath('jenkinsfile')
        }
  }
  branchSources {
    git {
      remote("ssh://git@git.kcura.com:7999/dtx/import-api-rdc.git")
      credentialsId('bitbucket-repo-key')
    }
  }
}
