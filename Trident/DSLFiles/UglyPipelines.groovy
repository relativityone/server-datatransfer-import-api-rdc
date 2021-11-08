folder('Import-API') {
	description('Folder containing all Import-API non-CD pipelines')
}

// Create your job inside the folder you created above
// IMPORTANT!!!
// IMPORTANT!!! Choose a job name that's meaningful and relevant to your use case. Ensure you place it in the folder you created above
// IMPORTANT!!!
multibranchPipelineJob('Import-API/Compatibility-pipeline') {
	displayName('Compatibility pipeline')
	description('Compatibility tests against all supported Relativity versions.')
	branchSources {
		branchSource {
			source {
				bitbucket {
					// IMPORTANT!!!
					// IMPORTANT!!! Bitbucket project identifier and repository name
					// IMPORTANT!!!
					repoOwner('DTX')
					repository('Import-API-RDC') // name of your repository
					credentialsId('JenkinsKcuraBBSVC')
					serverUrl('https://git.kcura.com')
					// IMPORTANT!!!
					// IMPORTANT!!! Make sure that each branch source has a constant and unique identifier.  Create one with a tool like : https://www.uuidgenerator.net/
					// IMPORTANT!!!
					id('d915795c-b651-4342-a2a8-cb632860e31c') // id must be unique
				}
			}
		}
	}

	// do some additional stuff

	configure { project ->
	(project / factory / scriptPath).value = "Trident/Jobs/nightly.groovy" // path to your job file
	}

	configure {
		def traits = it / sources / data / 'jenkins.branch.BranchSource' / source / traits
		// IMPORTANT!!!
		// IMPORTANT!!! Set up your branch includes/excludes filter here. Jobs will not be created for branches that aren't included
		// IMPORTANT!!!
		traits << 'jenkins.scm.impl.trait.WildcardSCMHeadFilterTrait' {
			includes('Trident develop release-* pipeline-test-*') // branches that you want to include in your job
			excludes('') // branches you want to exclude from your job
		}
		// you shouldn't need to modify anything under here unless you have some extraordinary use case
		traits << 'com.cloudbees.jenkins.plugins.bitbucket.BranchDiscoveryTrait' {
			strategyId('3')
		}
		traits << 'jenkins.plugins.git.traits.CleanAfterCheckoutTrait' {
			extension(class:'hudson.plugins.git.extensions.impl.CleanCheckout')
		}
		traits << 'com.cloudbees.jenkins.plugins.bitbucket.SSHCheckoutTrait' {
			credentialsId('bitbucket-repo-key')
		}

		def namedBranchStrategies = it / sources / data / 'jenkins.branch.BranchSource' / buildStrategies / 'jenkins.branch.buildstrategies.basic.AllBranchBuildStrategyImpl' / 'strategies'
		namedBranchStrategies / 'jenkins.branch.buildstrategies.basic.SkipInitialBuildOnFirstBranchIndexing'
		def filters = namedBranchStrategies / 'jenkins.branch.buildstrategies.basic.NamedBranchBuildStrategyImpl' / 'filters'
		filters << 'jenkins.branch.buildstrategies.basic.NamedBranchBuildStrategyImpl_-WildcardsNameFilter' {
			'includes' '*'
			'excludes' '*'
			'caseSensitive' false
		}
	}
}

multibranchPipelineJob('Import-API/loadtest-pipeline') {
	displayName('Loadtests pipeline')
	description('Execute loadtests against hoppers.')
	branchSources {
		branchSource {
			source {
				bitbucket {
					// IMPORTANT!!!
					// IMPORTANT!!! Bitbucket project identifier and repository name
					// IMPORTANT!!!
					repoOwner('DTX')
					repository('Import-API-RDC') // name of your repository
					credentialsId('JenkinsKcuraBBSVC')
					serverUrl('https://git.kcura.com')
					// IMPORTANT!!!
					// IMPORTANT!!! Make sure that each branch source has a constant and unique identifier.  Create one with a tool like : https://www.uuidgenerator.net/
					// IMPORTANT!!!
					id('d915795c-b651-4342-a2a8-ad123860e31c') // id must be unique
				}
			}
		}
	}

	// do some additional stuff

	configure { project ->
		(project / factory / scriptPath).value = "Trident/Jobs/loadtest.groovy" // path to your job file
	}

	configure {
		def traits = it / sources / data / 'jenkins.branch.BranchSource' / source / traits
		// IMPORTANT!!!
		// IMPORTANT!!! Set up your branch includes/excludes filter here. Jobs will not be created for branches that aren't included
		// IMPORTANT!!!
		traits << 'jenkins.scm.impl.trait.WildcardSCMHeadFilterTrait' {
			includes('Performance-pipeline develop pipeline-test-*') // branches that you want to include in your job
			excludes('') // branches you want to exclude from your job
		}
		// you shouldn't need to modify anything under here unless you have some extraordinary use case
		traits << 'com.cloudbees.jenkins.plugins.bitbucket.BranchDiscoveryTrait' {
			strategyId('3')
		}
		traits << 'jenkins.plugins.git.traits.CleanAfterCheckoutTrait' {
			extension(class:'hudson.plugins.git.extensions.impl.CleanCheckout')
		}
		traits << 'com.cloudbees.jenkins.plugins.bitbucket.SSHCheckoutTrait' {
			credentialsId('bitbucket-repo-key')
		}

		def namedBranchStrategies = it / sources / data / 'jenkins.branch.BranchSource' / buildStrategies / 'jenkins.branch.buildstrategies.basic.AllBranchBuildStrategyImpl' / 'strategies'
		namedBranchStrategies / 'jenkins.branch.buildstrategies.basic.SkipInitialBuildOnFirstBranchIndexing'
		def filters = namedBranchStrategies / 'jenkins.branch.buildstrategies.basic.NamedBranchBuildStrategyImpl' / 'filters'
		filters << 'jenkins.branch.buildstrategies.basic.NamedBranchBuildStrategyImpl_-WildcardsNameFilter' {
			'includes' '*'
			'excludes' '*'
			'caseSensitive' false
		}
	}
}

multibranchPipelineJob('Import-API/regression-pipeline') {
	displayName('Regression pipeline')
	description('Regression tests against regression environments.')
	branchSources {
		branchSource {
			source {
				bitbucket {
					// IMPORTANT!!!
					// IMPORTANT!!! Bitbucket project identifier and repository name
					// IMPORTANT!!!
					repoOwner('DTX')
					repository('Import-API-RDC') // name of your repository
					credentialsId('JenkinsKcuraBBSVC')
					serverUrl('https://git.kcura.com')
					// IMPORTANT!!!
					// IMPORTANT!!! Make sure that each branch source has a constant and unique identifier.  Create one with a tool like : https://www.uuidgenerator.net/
					// IMPORTANT!!!
					id('d915795c-b651-4342-a3a8-ad123460e31c') // id must be unique
				}
			}
		}
	}

	configure { project ->
		(project / factory / scriptPath).value = "Trident/Jobs/regression.groovy" // path to your job file
	}

	configure {
		def traits = it / sources / data / 'jenkins.branch.BranchSource' / source / traits
		// IMPORTANT!!!
		// IMPORTANT!!! Set up your branch includes/excludes filter here. Jobs will not be created for branches that aren't included
		// IMPORTANT!!!
		traits << 'jenkins.scm.impl.trait.WildcardSCMHeadFilterTrait' {
			includes('develop release-* pipeline-test-*') // branches that you want to include in your job
			excludes('') // branches you want to exclude from your job
		}
		// you shouldn't need to modify anything under here unless you have some extraordinary use case
		traits << 'com.cloudbees.jenkins.plugins.bitbucket.BranchDiscoveryTrait' {
			strategyId('3')
		}
		traits << 'jenkins.plugins.git.traits.CleanAfterCheckoutTrait' {
			extension(class:'hudson.plugins.git.extensions.impl.CleanCheckout')
		}
		traits << 'com.cloudbees.jenkins.plugins.bitbucket.SSHCheckoutTrait' {
			credentialsId('bitbucket-repo-key')
		}

		def namedBranchStrategies = it / sources / data / 'jenkins.branch.BranchSource' / buildStrategies / 'jenkins.branch.buildstrategies.basic.AllBranchBuildStrategyImpl' / 'strategies'
		namedBranchStrategies / 'jenkins.branch.buildstrategies.basic.SkipInitialBuildOnFirstBranchIndexing'
		def filters = namedBranchStrategies / 'jenkins.branch.buildstrategies.basic.NamedBranchBuildStrategyImpl' / 'filters'
		filters << 'jenkins.branch.buildstrategies.basic.NamedBranchBuildStrategyImpl_-WildcardsNameFilter' {
			'includes' '*'
			'excludes' '*'
			'caseSensitive' false
		}
	}
}
  
multibranchPipelineJob('Import-API/MassImportVerification-pipeline') {
	displayName('Mass Import Verification pipeline')
	description('Integration tests for different settings against newest Relativity version from develop')
	branchSources {
		branchSource {
			source {
				bitbucket {
					// IMPORTANT!!!
					// IMPORTANT!!! Bitbucket project identifier and repository name
					// IMPORTANT!!!
					repoOwner('DTX')
					repository('Import-API-RDC') // name of your repository
					credentialsId('JenkinsKcuraBBSVC')
					serverUrl('https://git.kcura.com')
					// IMPORTANT!!!
					// IMPORTANT!!! Make sure that each branch source has a constant and unique identifier.  Create one with a tool like : https://www.uuidgenerator.net/
					// IMPORTANT!!!
					id('d915795c-b651-4342-a5a8-ad123460e32d') // id must be unique
						
				}
			}
		}
	}

	configure { project ->
		(project / factory / scriptPath).value = "Trident/Jobs/massimportverification.groovy" // path to your job file
	}

	configure {
		def traits = it / sources / data / 'jenkins.branch.BranchSource' / source / traits
		// IMPORTANT!!!
		// IMPORTANT!!! Set up your branch includes/excludes filter here. Jobs will not be created for branches that aren't included
		// IMPORTANT!!!
		traits << 'jenkins.scm.impl.trait.WildcardSCMHeadFilterTrait' {
			includes('develop pipeline-test-*') // branches that you want to include in your job
			excludes('') // branches you want to exclude from your job
		}
		// you shouldn't need to modify anything under here unless you have some extraordinary use case
		traits << 'com.cloudbees.jenkins.plugins.bitbucket.BranchDiscoveryTrait' {
			strategyId('3')
		}
		traits << 'jenkins.plugins.git.traits.CleanAfterCheckoutTrait' {
			extension(class:'hudson.plugins.git.extensions.impl.CleanCheckout')
		}
		traits << 'com.cloudbees.jenkins.plugins.bitbucket.SSHCheckoutTrait' {
			credentialsId('bitbucket-repo-key')
		}

		def namedBranchStrategies = it / sources / data / 'jenkins.branch.BranchSource' / buildStrategies / 'jenkins.branch.buildstrategies.basic.AllBranchBuildStrategyImpl' / 'strategies'
		namedBranchStrategies / 'jenkins.branch.buildstrategies.basic.SkipInitialBuildOnFirstBranchIndexing'
		def filters = namedBranchStrategies / 'jenkins.branch.buildstrategies.basic.NamedBranchBuildStrategyImpl' / 'filters'
		filters << 'jenkins.branch.buildstrategies.basic.NamedBranchBuildStrategyImpl_-WildcardsNameFilter' {
			'includes' '*'
			'excludes' '*'
			'caseSensitive' false
		}
	}
}

multibranchPipelineJob('Import-API/ReleaseBranches-pipeline') {
	displayName('Release branches pipeline')
	description('Unit and Integration tests on release branches against Relativity versions that represent current and upcoming production releases')
	branchSources {
		branchSource {
			source {
				bitbucket {
					// IMPORTANT!!!
					// IMPORTANT!!! Bitbucket project identifier and repository name
					// IMPORTANT!!!
					repoOwner('DTX')
					repository('Import-API-RDC') // name of your repository
					credentialsId('JenkinsKcuraBBSVC')
					serverUrl('https://git.kcura.com')
					// IMPORTANT!!!
					// IMPORTANT!!! Make sure that each branch source has a constant and unique identifier.  Create one with a tool like : https://www.uuidgenerator.net/
					// IMPORTANT!!!
					id('d915795c-b651-4342-a5a8-ad123460e33e') // id must be unique
						
				}
			}
		}
	}

	configure { project ->
		(project / factory / scriptPath).value = "Trident/Jobs/releasebranches.groovy" // path to your job file
	}

	configure {
		def traits = it / sources / data / 'jenkins.branch.BranchSource' / source / traits
		// IMPORTANT!!!
		// IMPORTANT!!! Set up your branch includes/excludes filter here. Jobs will not be created for branches that aren't included
		// IMPORTANT!!!
		traits << 'jenkins.scm.impl.trait.WildcardSCMHeadFilterTrait' {
			includes('release-* pipeline-test-*') // branches that you want to include in your job
			excludes('') // branches you want to exclude from your job
		}
		// you shouldn't need to modify anything under here unless you have some extraordinary use case
		traits << 'com.cloudbees.jenkins.plugins.bitbucket.BranchDiscoveryTrait' {
			strategyId('3')
		}
		traits << 'jenkins.plugins.git.traits.CleanAfterCheckoutTrait' {
			extension(class:'hudson.plugins.git.extensions.impl.CleanCheckout')
		}
		traits << 'com.cloudbees.jenkins.plugins.bitbucket.SSHCheckoutTrait' {
			credentialsId('bitbucket-repo-key')
		}

		def namedBranchStrategies = it / sources / data / 'jenkins.branch.BranchSource' / buildStrategies / 'jenkins.branch.buildstrategies.basic.AllBranchBuildStrategyImpl' / 'strategies'
		namedBranchStrategies / 'jenkins.branch.buildstrategies.basic.SkipInitialBuildOnFirstBranchIndexing'
		def filters = namedBranchStrategies / 'jenkins.branch.buildstrategies.basic.NamedBranchBuildStrategyImpl' / 'filters'
		filters << 'jenkins.branch.buildstrategies.basic.NamedBranchBuildStrategyImpl_-WildcardsNameFilter' {
			'includes' '*'
			'excludes' '*'
			'caseSensitive' false
		}
	}
}