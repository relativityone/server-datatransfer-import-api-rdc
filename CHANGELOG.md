# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.9.2001] - 06-Feb-2024
 
### Changed
 
- [REL-911798(https://jira.kcura.com/browse/REL-911798) - Minor version and compatible changes for IAPI

## [2.9.1005] - 11-28-2023
 
### Changed
 
- [REL-866934](https://jira.kcura.com/browse/REL-866934) - RDC Installer Should default to a server configuration

## [2.9.1004] - 11-28-2023

### Fixed

- [REL-891563](https://jira.kcura.com/browse/REL-891563) - Issues with Saved search Export using Production Precedence

- Updated version.txt and version.wxi file

## [2.9.1003] - 11-14-2023

### Fixed

- [REL-873329](https://jira.kcura.com/browse/REL-873329) - Ensure Import API and RDC is able to start logging from the time it's start
- Updated version.txt and version.wxi file

## [2.9.1002] - 10-18-2023

### Fixed

- [REL-873329](https://jira.kcura.com/browse/REL-873329) - Ensure Import API and RDC is able to start logging from the time it's start

## [2.9.1001] - 10-06-2023

### Fixed

- [REL-867254](https://jira.kcura.com/browse/REL-867254) - Verification of Issue with the BCP share and the fallback mode by updating latest Relativity.Transfer.Client

## [2.9.2] - 08-28-2023

### Fixed

- [REL-871666](https://jira.kcura.com/browse/REL-871666)

## [2.9.1] - 08-14-2023

### Fixed

- [REL-857917](https://jira.kcura.com/browse/REL-857917) - Backported from [REL-669697](https://jira.kcura.com/browse/REL-669697) ticket from Server 2022 release.
- [REL-864302](https://jira.kcura.com/browse/REL-864302) - IAPI Backport - Data Transfer - RAPs that use IAPI - Backported [REL-711458](https://jira.kcura.com/browse/REL-711458) ticket from Server 2022 release.
- [REL-864303](https://jira.kcura.com/browse/REL-864303#add-comment) - Backported from [REL-707113](https://jira.kcura.com/browse/REL-707113) ticket from Server 2022 release.
- [REL-864304](https://jira.kcura.com/browse/REL-864304) - Backported from [REL-732260](https://jira.kcura.com/browse/REL-732260) ticket from Server 2022 release.
- [REL-864305](https://jira.kcura.com/browse/REL-864305) - Backported from [REL-789615](https://jira.kcura.com/browse/REL-789615) ticket from Server 2022 release.


## [2.9.0] - 08-04-2023
 
### Changed
 
- [REL-862766](https://jira.kcura.com/browse/REL-862766) - Updated RDC version to 12.3.18 from 12.3.17

## [2.8.0] - 08-04-2023
 
### Changed
 
- [REL-862766](https://jira.kcura.com/browse/REL-862766) - Create release branch for RDC
- Official Relativity 2023 12.3 release.
- The SUT configuration upgrades the previous release image to the latest release image.

## [2.7.0] - 07-18-2023

### Changed

- [REL-857337](https://jira.kcura.com/browse/REL-857337) - Unrecognized Guid format - Backported [REL-576995](https://jira.kcura.com/browse/REL-576995) ticket from Server 2022 release.

## [2.6.0] - 07-17-2023

### Changed

- [REL-848356](https://jira.kcura.com/browse/REL-848356) - Revved TAPI Gold Package - Backported [REL-712106](https://jira.kcura.com/browse/REL-712106) ticket from Server 2022 release.

## [2.5.0] - 07-06-2023

### Fixed

- Trident build failure fix for server-develop - git hash commit is not returned due to this commitsSinceLastTag was getting some invalid string.

## [2.3.0] - 07-03-2023

### Changed

- [REL-854115](https://jira.kcura.com/browse/REL-854115) - IAPI packages publishing to bld-pkgs - Avoided publishing packages to bld-pkgs location
- Updated README.md file

## [2.2.0] - 06-20-2023

### Changed

- [REL-848315](https://jira.kcura.com/browse/REL-848315) - Updated help link to have Server version of RDC - Backported [REL-645382](https://jira.kcura.com/browse/REL-645382) ticket from Server 2022 release

## [2.1.0] - 06-19-2023

### Changed

- Updated the Relativity.OutsideIn package in nuspec file to version 2023.4.0 [REL-841948] (https://jira.kcura.com/browse/REL-841948)

## [2.0.0] - 06-14-2023

### Changed

- Updated the Relativity.OutsideIn package to 2023.4.0 [REL-841948] (https://jira.kcura.com/browse/REL-841948)

## [1.9.9] - 06-14-2023

### Changed

- Updated tagGitCommit logic to get server git tags [REL-850174] (https://jira.kcura.com/browse/REL-850174)

## [1.17.1] - 06-08-2023

### Changed

- Aligned 'Import-API-RDC' versioning to trident standards [REL-850174] (https://jira.kcura.com/browse/REL-850174)

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