# Relativity Data Exchange API and RDC for .NET
You can use the data exchange API to build application components that import documents, objects, images, and productions using a flexible number of transfer clients. The Remote Desktop Client (RDC) is a Windows desktop application built on Import API. It should be noted that this repository also includes Export API; however, this is heavily dependent on the RDC and has never been officially supported.

## System requirements
* .NET 4.6.2
* Visual C++ 2010 x86 Runtime (Aspera transfers)
* Visual C++ 2015 x64 Runtime (Outside In)
* Visual C++ 2017 x86 and x64 Runtime (FreeImage)
* Intel 2Ghz (2-4 cores is recommended)
* 8GB RAM (32GB of RAM is recommended)

## Build requirements
You must have the following installed in order to build the master solution.
* Visual Studio 2017/2019
* NUnit 3 Test Adapter v3.12.0.0 (required to run NUnit3 tests)
* Paket for Visual Studio v0.38.3 (required for IDE builds)
* ConfigureAwait Checker Resharper Extension

***Note:** Paket for Visual Studio adds Update|Install|Restore menu items when right-clicking a paket.dependencies file.*

## Building the master solution
Generally speaking, PowerShell is used to perform initial and pre-commit builds and Visual Studio for interim builds.

### PowerShell
1. Open a Windows PowerShell window
2. Navigate to the root of the repository
3. Use any of the build commands below

***Note:** The first parameter specifies the task to execute and defaults to the Build task. The examples below explicitly include Build for clarity.*

```bash
# Cleans the solution.
.\build.ps1 Clean
```

```bash
# Builds the solution using a Release configuration, run the standard + extended CA checks, and then StyleCop analyzer.
.\build.ps1 Build,ExtendedCodeAnalysis
```

```bash
# Rebuilds the solution using a Release configuration, runs standard + extended CA checks, and then StyleCop analyzer.
.\build.ps1 Build,ExtendedCodeAnalysis -Target Rebuild
```

```bash
# Rebuilds the solution using a Debug configuration, runs standard + extended CA checks, and then StyleCop analyzer.
.\build.ps1 Build,ExtendedCodeAnalysis -Target Rebuild -Configuration Debug
```

### Visual Studio
1. Open the *Master.sln* solution
2. Open the .paket solution folder
3. Right-click the paket.dependencies file and click the Restore menu item
4. Click CTRL+SHIFT+B to build the solution

***Note:** VS-based builds always run the standard CA checks and StyleCop analyzer. Extended CA checks are never run due to execution time.*

### ILMerge and SDK Assembly
When IAPI was decoupled from Relativity, a key objective was to simplify deployment by reducing the number of package assemblies from 11 all the way down to a **single lightweight assembly**. To achieve this goal, a combination of MSBUILD "tricks" and ILMERGE is used to produce the new ligthweight SDK assembly.

The implementation details are as follows:

* New build configurations added to drive overall build behavior
  * **Debug|Release**: Only project references used (no change in behavior)
  * **Debug-ILMerge|Release-ILMerge**: Force assembly references and build a single SDK assembly
* .\buildtools\Relativity.Build.DynamicReferences.targets
  * Dynamically switches project/assembly references via build configuration
  * Unit and integration tests import this custom targets file
* .\Scripts\Invoke-ILMerge.ps1
  * Performs all ILMERGE functionality
  * The final assembly is stored in the .\Artifacts\binaries\sdk sub-folder
* .\.paket\paket.template.relativity.dataexchange.client.sdk
  * Represents the new SDK package template
  * References the ILMERGE'd assembly file
* Relativity.DataExchange.Export.csproj
  * Post-build event calls Invoke-ILMerge.ps1
  * MSBUILD parameters are passed
  * **Debug|Release**: Do nothing; otherwise, execute ILMERGE
* Master-ILMerge.sln
  * The new solution was added to force **solution dependencies**
  * **A 2nd solution file ensures project reference behavior is *not* impacted**
* .\build.ps1
  * **-ILMerge** switch parameter drives build behavior
  * Decides which solution file to use
  * Decides whether to append "-ILMerge" to the specified build configuration


***Note:** Master.sln should **not** be used to build/test the ILMERGE'd SDK assembly because it's configured for project references!*

```bash
# Build the solution, ILMERGE the SDK assembly, and use assembly file references.
.\build.ps1 -ILMerge
```

```bash
# Same as above but run the unit and integration tests.
.\build.ps1 Build,UnitTests,IntegrationTests -ILMerge
```

## Testing and code coverage
Unit and integration tests can be executed via the Test Explorer window or the `PowerShell` build script. The following sections focus on `PowerShell` test execution, which relies on the NUnit 3 Console Runner.

***Note:** All test results are stored within the .\TestResults sub-folder.*

### Unit tests
The unit tests are very straight-forward to execute and complete quickly.

```bash
# Run the unit tests.
.\build.ps1 UnitTests
```

```bash
# Same as above but generate a test report underneath the .\TestReports\unit-tests sub-folder.
.\build.ps1 UnitTests,TestReports
```

### Integration tests
The integration tests, by their very nature, perform "deeper" operations and take several minutes to complete. See [test parameters](#test-parameters) for details on how to target different test instances.

```bash
# Run the integration tests using either environment variables or modified app.config setting for all integration test parameters.
.\build.ps1 IntegrationTests
```

```bash
# Run the integration tests using the hyper-v internal test environment for all integration test parameters.
.\build.ps1 IntegrationTests
```

```bash
# Same as above but generate a test report underneath the .\TestReports\integration-tests sub-folder.
.\build.ps1 IntegrationTests,TestReports
```

### TestVM support
The build scripts make it very easy to run *all* integration tests with a TestVM.

```bash
# Run the integration tests using the first discovered TestVM.
.\build.ps1 IntegrationTests -TestVM
```

```bash
# Same as above but use the specified TestVM.
.\build.ps1 IntegrationTests -TestVM -TestVMName "P-DV-VM-NUN5BEE"
```

***Note:** When running integration tests on a `TestVM`, the test parameters are temporarily set via **process** environment variables only.*

### Code coverage
The build scripts extend the testing framework to include code coverage using the [dotCover](https://www.jetbrains.com/dotcover) CLI package. When performing a code coverage run, both unit and integration tests are used; therefore, [test parameters](#test-parameters) are required and the same options apply.

```bash
# Run a code coverage test and generate a coverage report underneath the .\TestReports\code-coverage sub-folder.
.\build.ps1 CodeCoverageReport
```

```bash
# Same as above but use The hyper-v internal test environment for all integration test parameters.
.\build.ps1 CodeCoverageReport -TestEnvironment "hyperv"
```

### Test categories
To provide a convenient way to filter tests, `TestCategories` class exists within the `Relativity.DataExchange.TestFramework` assembly. The most important category is `Integration` because it's used as a filter when running unit or integration tests.

```csharp
[Test]
[Category(TestCategories.Integration)]
public void ShouldPerformIntegrationTest()
```

***Note:** All integration tests must include the `TestCategories.Integration` to be included in the `PowerShell` integration test run.*

### Test parameters
The majority of integration tests require a Relativity instance to work properly. The combination of test framework code and build scripts provide a number of flexible options to assign test parameters including:

* [App.Config and Environment Variables](#appconfig-and-environment-variables)
* [JSON File](#json-file)
* [Test Environment Build Parameter](#test-environment-build-parameter)

#### App.Config and environment variables
The following table outlines all available `App.Config` and environment variable test parameters.

|Application Setting         |Environment Variable                          |Description                                                        |Example                                              |
|:---------------------------|:---------------------------------------------|:------------------------------------------------------------------|:----------------------------------------------------|
|RelativityUrl               | IAPI_INTEGRATION_RELATIVITYURL               | The Relativity instance URL.                                      | https://hostname.mycompany.corp                     |
|RelativityRestUrl           | IAPI_INTEGRATION_RELATIVITYRESTURL           | The Relativity Rest API URL.                                      | https://hostname.mycompany.corp/relativity.rest/api |
|RelativityServicesUrl       | IAPI_INTEGRATION_RELATIVITYSERVICEURL        | The Relativity Services API URL.                                  | https://hostname.mycompany.corp/relativity.services |
|RelativityWebApiUrl         | IAPI_INTEGRATION_RELATIVITYWEBAPIURL         | The Relativity Web API URL.	                                    | https://hostname.mycompany.corp/relativitywebapi    |
|RelativityUserName          | IAPI_INTEGRATION_RELATIVITYUSERNAME          | The Relativity login user name.                                   | email@company.com                                   |
|RelativityPassword          | IAPI_INTEGRATION_RELATIVITYPASSWORD          | The Relativity login password.                                    | SomePassword!                                       |
|ServerCertificateValidation | IAPI_INTEGRATION_SERVERCERTIFICATEVALIDATION | Specify whether to enforce server certificate validation errors.  | false                                               |
|SkipAsperaModeTests         | IAPI_INTEGRATION_SKIPASPERAMODETESTS         | Skips any tests that require Aspera mode.                         | true                                                |
|SkipDirectModeTests         | IAPI_INTEGRATION_SKIPDIRECTMODETESTS         | Skips any tests that require Direct mode.                         | false                                               |
|SkipIntegrationTests        | IAPI_INTEGRATION_SKIPINTEGRATIONTESTS        | Skips running all integration tests.                              | false                                               |
|SqlDropWorkspaceDatabase    | IAPI_INTEGRATION_SQLDROPWORKSPACEDATABASE    | Specify whether to drop the SQL database when the test completes. | true                                                |
|SqlInstanceName             | IAPI_INTEGRATION_SQLINSTANCENAME             | The SQL instance where the workspace databases are stored.        | hostname.mycompany.corp\EDDSINSTANCE001             |
|SqlAdminUserName            | IAPI_INTEGRATION_SQLADMINUSERNAME            | The SQL system administrator user name.                           | sa                                                  |
|SqlAdminPassword            | IAPI_INTEGRATION_SQLADMINPASSWORD            | The SQL system administrator password.                            | SomePassword!                                       |
|WorkspaceTemplate           | IAPI_INTEGRATION_WORKSPACETEMPLATE           | The workspace template used to create the test workspace.         |	Relativity Starter Template                       |

#### JSON File
The same test parameters described above can also be used in JSON.

```json
{
	"RelativityUrl" : "https://hostname.mycompany.corp",
	"RelativityRestUrl" : "https://hostname.mycompany.corp/relativity.rest/api",
	"RelativityServicesUrl" : "https://hostname.mycompany.corp/relativity.services",
	"RelativityWebApiUrl" : "https://hostname.mycompany.corp/relativitywebapi",
	"RelativityUserName" : "email@company.com",
	"RelativityPassword" : "SomePassword!",
	"ServerCertificateValidation" : "False",
	"SkipAsperaModeTests" : "False",
	"SkipDirectModeTests" : "False",
	"SkipIntegrationTests" : "False",
	"SqlDropWorkspaceDatabase" : "True",
	"SqlInstanceName" : "hostname.mycompany.corp\\EDDSINSTANCE001",
	"SqlAdminUserName" : "sa",
	"SqlAdminPassword" : "SomePassword!",
	"WorkspaceTemplate" : "Relativity Starter Template"
}
```

The `PowerShell` build scripts can support JSON-based test parameters as well.

```bash
# Skips building the solution and only executes the integration tests using the JSON test parameters file
.\build.ps1 IntegrationTests -TestParametersFile "C:\Temp\test-parameters.json"
```

#### Test Environment Parameter
Given the ability to use a JSON-based test parameters file, the repo includes test parameter files for *well-known* test environments. This simplifies running all integration tests against production-like Relativity instances without specifying the file path.

***Note:** The JSON test parameter files are located within the .\Scripts folder.*

```bash
# Runs the integration tests using the Hyper-V test environment
.\build.ps1 IntegrationTests -TestEnvironment hyperv
```

### AssemblySetup
All integration tests rely on a uniform set of [test parameters](#test-parameters) that are used for both setup and the tests themselves. To ensure isolation exists between test assemblies, each integration test includes an `AssemblySetup` class and relies on the `SetUpFixtureAttribute` to guarantee that all setup operations complete before any tests within the assembly are executed.

The `IntegrationTestHelper` is used by all `AssemblySetup` test classes to perform the following:

* Retrieve all [test parameters](#test-parameters) and dump to the console
* Create a new [IntegrationTestParameters](#test-parameters) instance
* Create a `Relativity.Logging.ILog` instance using [Seq](https://getseq.net/), HTTP, and File sinks
* Create a test workspace *unless `SkipIntegrationTests` is true*

***Note:** The integration test setup can take 30-60 seconds.*

## MSBuild Details
The next sections provide details on relevant MSBuild and build tool details.

### Project Structure
The project structure is similar to other repos. Important folders and files are identified in the tree below.

```
├───.paket
├───buildtools
│   │
│   │   AllErrors.ruleset
│   │   Legacy.ruleset
│   │   stylecop.json
├───Scripts
│   │
│   │   Find-ChangedMigratedFiles.ps1
│   │   Invoke-ExtendedCodeAnalysis.ps1
│   │   Local.runsettings
│   │   Local_x64.runsettings
│   │   Test-PackageUpgrade.ps1
│   │   test-parameters-hyperv.json
│   │   test-parameters-sample.json
│   │   TestParameters-Template.reg
├───Source
│   ├───Relativity.Desktop.Client.Controls.Legacy
│   ├───Relativity.Desktop.Client.CustomActions
│   ├───Relativity.Desktop.Client.CustomActions.NUnit
│   ├───Relativity.Desktop.Client.Legacy
│   ├───Relativity.Desktop.Client.Legacy.NUnit
│   ├───Relativity.Desktop.Client.NUnit.Integration
│   ├───Relativity.Desktop.Client.Setup
│   ├───Relativity.DataExchange.Export
│   ├───Relativity.DataExchange.Export.NUnit
│   ├───Relativity.DataExchange.Export.NUnit.Integration
│   ├───Relativity.DataExchange.Import
│   ├───Relativity.DataExchange.Import.Legacy.NUnit
│   ├───Relativity.DataExchange.Import.NUnit
│   ├───Relativity.DataExchange.Import.NUnit.Integration
│   ├───Relativity.DataExchange.Import.Samples.NUnit
│   ├───Relativity.DataExchange.Core
│   ├───Relativity.DataExchange.Legacy
│   ├───Relativity.DataExchange.NUnit
│   ├───Relativity.DataExchange.NUnit.Integration
│   ├───Relativity.DataExchange.Services.Interfaces
│   ├───Relativity.DataExchange.TestFramework
│   │
│   │   Installers.sln
│   │   Master.sln
│   │   Master.sln.DotSettings
│   │   Master-ILMerge.sln
├───Vendor
│   │
│   │   iTextSharp
│   │   Licenses
│   │   Microsoft
│   │   WIX
│   │   FreeImage
├───Version
│   │
│   │   AssemblySharedInfo.cs
│   │   AssemblySharedInfo.vb
│   │   Update-AssemblyInfo.ps1
│
│   .gitignore
│   build.ps1
│   default.ps1
│   paket.dependencies
│   paket.lock
```
### Projects
|Name                                             |Project Type|Published|Description                                                                                                                 |
|:------------------------------------------------|:----------:|:-------:|:---------------------------------------------------------------------------------------------------------------------------|
|Relativity.Desktop.Client.Bundle                 |     WIX    |         |The RDC WIX bootstrapper project.                                                                                           |
|Relativity.Desktop.Client.CustomActions          |     C#     |         |The RDC WIX custom actions project.                                                                                         |
|Relativity.Desktop.Client.CustomActions.NUnit    |     C#     |         |The RDC WIX custom actions test project.                                                                                    |
|Relativity.Desktop.Client.Legacy                 |   VB.NET   |    X    |The RDC application/EXE project.                                                                                            |
|Relativity.Desktop.Client.Legacy.NUnit           |   VB.NET   |         |The RDC unit test project.                                                                                                  |
|Relativity.Desktop.Client.NUnit.Integration      |     C#     |         |The RDC integration test project.                                                                                           |
|Relativity.Desktop.Client.Setup                  |     WIX    |         |The RDC WIX setup/MSI project.                                                                                              |
|Relativity.DataExchange.Export                   |     C#     |         |The export API project.                                                                                                     |
|Relativity.DataExchange.Export.NUnit             |     C#     |         |The export API unit test project.                                                                                           |
|Relativity.DataExchange.Export.NUnit.Integration |     C#     |         |The export API integration test project.                                                                                    |
|Relativity.DataExchange.Import                   |     C#     |    X    |The import API project.                                                                                                     |
|Relativity.DataExchange.Import.Legacy.NUnit      |   VB.NET   |         |The import API unit test project.                                                                                           |
|Relativity.DataExchange.Import.NUnit             |     C#     |         |The import API unit test project.                                                                                           |
|Relativity.DataExchange.Import.NUnit.Integration |     C#     |         |The C# import API integration test project.                                                                                 |
|Relativity.DataExchange.Import.Samples.NUnit     |     C#     |    x    |The C# import API Github samples test project.                                                                              |
|Relativity.DataExchange                          |     C#     |         |The C# import/export class library project.                                                                                 |
|Relativity.DataExchange.Legacy                   |   VB.NET   |         |The import/export shared class library project.                                                                             |
|Relativity.DataExchange.NUnit                    |     C#     |         |The C# import/export unit test project.                                                                                     |
|Relativity.DataExchange.NUnit.Integration        |     C#     |         |The C# import/export integration test project.                                                                              |
|Relativity.DataExchange.TestFramework            |     C#     |         |The import/export test framework projecty.                                                                                  |

### Resharper Team Shared Layer
To ensure that all team members use the same `Resharper` settings, `Master.sln.DotSettings` is added to source control and adjacent to the master solution file.

### Paket and NuGet Packages
The bootstrapped tools and master solution build uses [Paket](https://fsprojects.github.io/Paket/) to support all package requirements. The repo contains 3 key Paket file types:

* `paket.dependencies:` The file that defines all packages contained within the repo.
* `paket.lock:` The file that gets updated whenever dependencies are added or changed.
* `paket.references:` Each project includes this file to specify all package references.

***Note:** Adding paket.lock to source control allows fast package restore and improves tracking dependency changes. It's understood that any future package changes requires a paket "install" in order to recompute the dependency graph. This is the only scenario where paket.lock should ever change.*

### Assembly Versioning
All `C#` and `VB.NET` projects contained within the master solution link in one of the following source files to control the assembly and file versions:

* `AssemblySharedInfo.cs`
* `AssemblySharedInfo.vb`

The main build script supports an optional `UpdateAssemblyInfo` task which, when specified, uses `GitVersion` to automatically apply semantic versioning info to both of these shared source files.

***Note:** Only Jenkins pipeline builds execute this task to avoid modifying source-controlled files during DEV builds.*

### Code Analysis
Roslyn represents the future for .NET based code analysis tools. Although Microsoft.CodeAnalysis.FxCopAnalyzers is a solid replacement for FxCop, additional work is required to migrate.

***Note:** This will be re-evaluated in the near future once all source has been migrated from the `Relativity` repo.*

#### Microsoft Code Analysis (FxCop)
All of the projects within the solution are configured to support *standard* Microsoft code analysis (FxCop) for both `Debug` and `Release` builds. Because some projects are better designed than others, it makes more sense to define two separate CA rulesets:

* `.\booltools\AllErrors.ruleset`: 99.99% of the available rules are enabled.
* `.\booltools\Autogen.ruleset`: relaxes a significant number of rules due to the nature of auto-generated code.
* `.\booltools\Legacy.ruleset`: retains most design rules but relaxes globalization, naming, and usage rules.
* `.\booltools\Tests.ruleset`: retains a smaller number of design rules to ensure test code is well maintained.

#### StyleCop.Analyzers
The `StyleCop.Analyzers` Roslyn-based analyzer is the de-facto replacement for the original `StyleCop` extension, works flawlessly, and is integrated within *all* projects.

* `.\booltools\*.ruleset`: Controls whether SC rules are enabled or disabled.
* `.\booltools\stylecop.json`: retains most design rules but relaxes globalization, naming, and usage rules.

***Note:** The `AllErrors.ruleset` enforces virtually all SC rules and is configured for all public facing projects.*

#### Resharper Code Analysis (PowerShell only)
Over the last few years, improper async/await usage caused `ASP.NET` applications to "hang" and resulted in PD alerts. After some investigation, the Resharper community developed an extension to identify this type of violation. Not only does the extension write a "squiggly" on the offending line, but it can be invoked using the Resharper CLI tools.

The `.\Scripts\Invoke-ExtendedCodeAnalysis.ps1` PS script scans the solution and identifies all violations. The PSake-based build scripts are designed to fail the build for a single violation.

***Note:** The extension is limited to C# projects.*