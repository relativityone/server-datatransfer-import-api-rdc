# Changelog for Import API
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project does *not* yet adhere to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Foxglove EA preview release - v10.2.15.51 (01-22-2019)

**Added**

- Improved I/O resiliency for extracted text and System.IO.* API calls
- New "TempDirectory" config setting to override the directory where all import temp load files are stored
- New "TapiPreserveFileTimestamps" configuration setting that enables/disables preserving file timestamps in Aspera and Direct mode
- Performance and resiliency improvement in Folder creation process


**Fixed**

- Improved text exceed field size error messages [REL-277165]
- SQL locks weren't always released and can cause deadlocks [REL-293445]
- Import failures may continue to update closed load file streams and throw ObjectDisposedException [REL-277572]
- Fixing thread unsafe file stream closing [REL-277572]
- Fixing issue with deep folder structure creation [REL-268845]

## Blazingstar update 1 release - v10.1.169.1 (03-25-2019)

**Fixed**

- Reviewed and fixed all exception classes that didn't implement serialization properly [REL-266229]
- Enabled APM by default for all RelativityOne import jobs [REL-270750]
- The compatibility check exception thrown uses the ApplicationName configuration setting to format the error message instead of a hard-coded RDC string [REL-268853]
- Temp load files use a unique filename to avoid exceeding the max number of temp files [REL-135101]
- Native file identification throws a NullReferenceException in some import workflows [REL-273473]
- Thread locks weren't getting released due to imbalanced Monitor Enter/Exit [REL-244186]
- An explicit check is made to prevent integrated security from being used in non-interactive processes (e.g. Agent). API users should use CreateByRsaBearerToken instead [REL-269088]

## Larkspur update 2 release - v10.0.233.5 (03-15-2019)

**Fixed**

- WebAPI/REST Service URL can cause failures  [REL-281370]
- Updated the app.config file and public documentation [REL-285349]
- Added support to preserve file timestamps for both import and production export workflows [REL-298418]
- Production export can fail when there are more than 1000 files [REL-292896]

## Bluestem release - v9.7.229.5 (02-04-2019)

**Added**

- Import API and all platform dependency packages are now available in [nuget.org](https://www.nuget.org/packages?q=Relativity)
- The ImportAPI class provides CreateByRsaBearerToken static method to construct the ImportAPI object using a bearer token. This eliminates the requirement to use integrated security or manage Relativity credentials when creating the API object within an agent or custom page

**Fixed**

- API failures can occur when using integrated security [REL-23429]
- Production PDF export out of order with pagecount over 10 [REL-279296]
- WebAPI/REST service URL can fail in RelativityOne [REL-281370]
- SQL resource locks are never released and can cause deadlocks [REL-276758]
- Creating client-side folders can fail [REL-194231]

**Code Isolation**

- Isolated the code using sundrop tag 1.15.33 [REL-825271]
- Bumped up the RDC version to 12.3.13x
- Converted paket reference to CPM
