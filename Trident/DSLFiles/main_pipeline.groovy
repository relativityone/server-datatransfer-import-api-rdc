folder('DataTransfer') {
    description('Folder for data transfer')
}

multibranchPipelineJob('DataTransfer/import-api-rdc') {
  branchSources {
      branchSource {
        source {
            git {
                remote('ssh://git@git.kcura.com:7999/dtx/import-api-rdc.git') // link to your repository
                credentialsId('bitbucket-repo-key')
                id('ed1c1e04-add7-48dd-8bf8-655dd1337f62') // id will be a required field for multibranch pipelines
            }
        }
        strategy {
            defaultBranchPropertyStrategy {
                props {
                    noTriggerBranchProperty()
                }
            }
        }
    }
  }

  triggers {
      periodic(2) 
  }

    configure { project ->
    (project / factory / scriptPath).value = "Trident/DSLFiles/main_pipeline.groovy" 
  }

    configure {
    def traits = it / sources / data / 'jenkins.branch.BranchSource' / source / traits
    traits << 'jenkins.plugins.git.traits.BranchDiscoveryTrait'()
    traits << 'jenkins.scm.impl.trait.WildcardSCMHeadFilterTrait' {
           includes('*') // branches to include
        }
    traits << 'jenkins.plugins.git.traits.CleanAfterCheckoutTrait' {
        extension(class:'hudson.plugins.git.extensions.impl.CleanCheckout')
    }
  }
}