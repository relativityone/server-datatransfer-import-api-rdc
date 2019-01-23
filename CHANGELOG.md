# Changelog for Import API
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project does *not* yet adhere to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Foxglove EA preview release - v10.2.15.51 (01-22-2019)

**Added**

- Improved I/O resiliency for extracted text and System.IO.* API calls
- New "TempDirectory" config setting to override the directory where all import temp load files are stored

**Fixed**

- Improved text exceed field size error messages [REL-277165]

## Bluestem update 3 preview release - v9.7.209.7 (01-14-2019)

**Added**

- Import API and all platform dependency packages are now available in [nuget.org](https://www.nuget.org/packages?q=Relativity)
- The ImportAPI class provides CreateByRsaBearerToken static method to construct the ImportAPI object using a bearer token. This eliminates the requirement to use integrated security or manage Relativity credentials when creating the API object within an agent or custom page

**Fixed**

- API failures can occur when using integrated security [REL-23429]
- Production PDF export out of order with pagecount over 10 [REL-279296]
- WebAPI/REST service URL can fail in RelativityOne [REL-281370]
- SQL resource locks are never released and can cause deadlocks [REL-276758]
- Creating client-side folders can fail [REL-194231]