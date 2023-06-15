# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.9.9] - 06-14-2023

### Changed

- Updated tagGitCommit logic to get server git tags [REL-850174] (https://jira.kcura.com/browse/REL-850174)

## [1.17.1] - 06-08-2023

### Changed

- Aligned 'Import-API-RDC' versioning to trident standards [REL-850174] (https://jira.kcura.com/browse/REL-850174)

## [1.0.4] - 06-12-2023

### Changed

- Updated help link to have Server version of RDC
- Bumped up the RDC version to 12.3.15

## [1.0.3] - 05-23-2023
### Changed

- Updated version in 'version.txt'.
- Made code change in 'Tridentfile'.
- Made code changes in 'Slack.groovy'. Changes are as below:
	- Created new slack-app : 'cicd-Import-API-RDC'
	- The new slack-app created url : https://kcura-pd.slack.com/apps/A057YAY6LK1-cicd-import-api-rdc?tab=more_info
	- Respective webhook url page: https://api.slack.com/apps/A057YAY6LK1/install-on-team?success=1
	- Used following Webhook url : https://hooks.slack.com/services/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
	  in Slack.groovy file.

## [1.0.2] - 05-17-2023

### Added

- Added a new changelog.md file
- Added CODEOWNERS File

### Changed

- Archived the existing changelog.md file
- Removed ##maintainers section in README.md
- The code owners details has been updated to show all server delta team members
- Bumped the minor version and zeroed out the patch number in version.wxi file