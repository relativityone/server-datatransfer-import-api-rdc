# Relativity Import API and RDC for .NET
You can use the Import API (IAPI) to build application components that import documents, objects, images, and productions using a flexible number of transfer clients. The Remote Desktop Client (RDC) is a Windows desktop application built on Import API. It should be noted that this repository also includes Export API; however, this is heavily dependent on the RDC and has never been officially supported.

## System requirements
* .NET 4.6.2
* Visual C++ 2010 x86 Runtime (Aspera transfers)
* Visual C++ 2015 x64 Runtime (Outside In and FreeImage)
* Intel 2Ghz (2-4 cores is recommended)
* 8GB RAM (32GB of RAM is recommended)

## Build requirements
You must have the following installed in order to build the master solution.
* Visual Studio 2017 and above
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

```bash
# Cleans the solution
.\build.ps1 -Target "Clean"
```

```bash
# Builds the solution using a Release configuration, run the standard + extended CA checks, and then StyleCop analyzer.
.\build.ps1 -ExtendedCodeAnalysis
```

```bash
# Rebuilds the solution using a Release configuration, runs standard + extended CA checks, and then StyleCop analyzer.
.\build.ps1 -Target "Rebuild" -ExtendedCodeAnalysis 
```

```bash
# Rebuilds the solution using a Debug configuration, runs standard + extended CA checks, and then StyleCop analyzer.
.\build.ps1 -Target "Rebuild" -Configuration "Debug" -ExtendedCodeAnalysis 
```

### Visual Studio
1. Open the *Relativity.ImportAPI-RDC.sln* solution
2. Open the .paket solution folder
3. Right-click the paket.dependencies file and click the Restore menu item
4. Click CTRL+SHIFT+B to build the solution

***Note:** VS-based builds always run the standard CA checks and StyleCop analyzer. Extended CA checks are never run due to execution time.*

## Unit and integration tests
Unit and integration tests can be executed via the Test Explorer window or the `PowerShell` build script. The following sections focus on `PowerShell` test execution, which relies on the NUnit 3 Console Runner.

### Unit tests
The unit tests are very straight-forward to execute and complete quickly.

```bash
# Skips building the solution and only executes the unit tests
.\build.ps1 -SkipBuild -UnitTests
```

### Integration tests
The integration tests, by their very nature, perform "deeper" operations and take several minutes to complete.

```bash
# Skips building the solution and only executes the integration tests
.\build.ps1 -SkipBuild -IntegrationTests
```

### TestVM support
The build scripts make it very easy to run *all* integration tests with a TestVM.

```bash
# Skips building the solution and only executes the integration tests using the first discovered TestVM
.\build.ps1 -SkipBuild -IntegrationTests -TestVM
```

```bash
# Skips building the solution and only executes the integration tests using the specified TestVM
.\build.ps1 -SkipBuild -IntegrationTests -TestVM -TestVMName "P-DV-VM-NUN5BEE"
```

***Note:** When running integration tests on a `TestVM`, the test parameters are temporarily set via **process** environment variables only.*

### Test categories
To provide a convenient way to filter tests, `TestCategories` class exists within the `Relativity.Import.Export.TestFramework` assembly. The most important category is `Integration` because it's used as a filter when running unit or integration tests.

```csharp
[Test]
[Category(TestCategories.Integration)]
public void ShouldPerformIntegrationTest()
```

***Note:** All integration tests must include the `TestCategories.Integration` to be included in the `PowerShell` integration test run.*

### Test parameters
The majority of integration tests require a Relativity instance to work properly. The combination of test framework code and build scripts provide a number of flexible options to assign test parameters including:

* App.Config
* Environment variables
* JSON File

#### App.Config and environment variables
The following table outlines all available `App.Config` and environment variable test parameters.

|Application Setting      |Environment Variable                        |Description                                                        |Example                                              |
|:------------------------|:------------------------------------------|:------------------------------------------------------------------|:----------------------------------------------------|
|RelativityUrl            | IAPI_INTEGRATION_RELATIVITYURL            | The Relativity instance URL.                                      | https://hostname.mycompany.corp                     |
|RelativityRestUrl        | IAPI_INTEGRATION_RELATIVITYRESTURL        | The Relativity Rest API URL.                                      | https://hostname.mycompany.corp/relativity.rest/api |
|RelativityServicesUrl    | IAPI_INTEGRATION_RELATIVITYSERVICEURL     | The Relativity Services API URL.                                  | https://hostname.mycompany.corp/relativity.services |
|RelativityWebApiUrl      | IAPI_INTEGRATION_RELATIVITYWEBAPIURL      | The Relativity Web API URL.	                                      | https://hostname.mycompany.corp/relativitywebapi    |
|RelativityUserName       | IAPI_INTEGRATION_RELATIVITYUSERNAME       | The Relativity login user name.                                   | email@company.com                                   |
|RelativityPassword       | IAPI_INTEGRATION_RELATIVITYPASSWORD       | The Relativity login password.                                    | SomePassword!                                       |
|SkipAsperaModeTests      | IAPI_INTEGRATION_SKIPASPERAMODETESTS      | Skips any tests that require Aspera mode.                         | true                                                |
|SkipDirectModeTests      | IAPI_INTEGRATION_SKIPDIRECTMODETESTS      | Skips any tests that require Direct mode.                         | false                                               |
|SkipIntegrationTests     | IAPI_INTEGRATION_SKIPINTEGRATIONTESTS     | Skips running all integration tests.                              | false                                               |
|SqlDropWorkspaceDatabase | IAPI_INTEGRATION_SQLDROPWORKSPACEDATABASE | Specify whether to drop the SQL database when the test completes. | true                                                |
|SqlInstanceName          | IAPI_INTEGRATION_SQLINSTANCENAME          | The SQL instance where the workspace databases are stored.        | hostname.mycompany.corp\EDDSINSTANCE001             |
|SqlAdminUserName         | IAPI_INTEGRATION_SQLADMINUSERNAME         | The SQL system administrator user name.                           | sa                                                  |
|SqlAdminPassword         | IAPI_INTEGRATION_SQLADMINPASSWORD         | The SQL system administrator password.                            | SomePassword!                                       |
|WorkspaceTemplate        | IAPI_INTEGRATION_WORKSPACETEMPLATE        | The workspace template used to create the test workspace.         |	Relativity Starter Template                         |

#### JSON
The same test parameters described above can also be used in JSON.

```json
{
	"RelativityUrl" : "https://hostname.mycompany.corp",
	"RelativityRestUrl" : "https://hostname.mycompany.corp/relativity.rest/api",
	"RelativityServicesUrl" : "https://hostname.mycompany.corp/relativity.services",
	"RelativityWebApiUrl" : "https://hostname.mycompany.corp/relativitywebapi",
	"RelativityUserName" : "email@company.com",
	"RelativityPassword" : "SomePassword!",
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
.\build.ps1 -SkipBuild -IntegrationTests -TestParametersFile "C:\Temp\test-parameters.json"
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
│   │   Invoke-ExtendedCodeAnalysis.ps1
├───Source
│   ├───Relativity.Desktop.Client.Legacy
│   ├───Relativity.Desktop.Client.Legacy.NUnit
│   ├───Relativity.Desktop.Client.NUnit.Integration
│   ├───Relativity.Export.Client
│   ├───Relativity.Export.Client.NUnit
│   ├───Relativity.Export.Client.NUnit.Integration
│   ├───Relativity.Import.Client
│   ├───Relativity.Import.Client.Legacy.NUnit
│   ├───Relativity.Import.Client.NUnit
│   ├───Relativity.Import.Client.NUnit.Integration
│   ├───Relativity.Import.Client.Samples.NUnit
│   ├───Relativity.Import.Export.Core
│   ├───Relativity.Import.Export.Core.Legacy
│   ├───Relativity.Import.Export.TestFramework
│   │
│   │   Relativity.ImportAPI-RDC.sln
│   │   Relativity.ImportAPI-RDC.sln.DotSettings
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
|Name                                           |Project Type|Published|Description                                                                                                                 |
|:----------------------------------------------|:----------:|:-------:|:---------------------------------------------------------------------------------------------------------------------------|
|Relativity.Desktop.Client.Legacy               |   `VB.NET` |    X    |The RDC application/EXE project.                                                                                            |
|Relativity.Desktop.Client.Legacy.NUnit         |   `VB.NET` |         |The RDC unit test project.                                                                                                  |
|Relativity.Desktop.Client.NUnit.Integration    |     C#     |         |The RDC integration test project.                                                                                           |
|Relativity.Export.Client                       |     C#     |         |The export API project.                                                                                                     |
|Relativity.Export.Client.NUnit                 |     C#     |         |The export API unit test project.                                                                                           |
|Relativity.Export.Client.NUnit.Integration     |     C#     |         |The export API integration test project.                                                                                    |
|Relativity.Import.Client                       |     C#     |    X    |The import API project.                                                                                                     |
|Relativity.Import.Client.Legacy.NUnit          |   `VB.NET` |         |The import API unit test project.                                                                                           |
|Relativity.Import.Client.NUnit                 |     C#     |         |The import API unit test project.                                                                                           |
|Relativity.Import.Client.NUnit.Integration     |     C#     |         |The C# import API integration test project.                                                                                 |
|Relativity.Import.Client.Samples.NUnit         |     C#     |    X    |The C# import API Github samples test project.                                                                              |
|Relativity.Import.Export.Core                  |     C#     |         |The C# import/export class library project.                                                                                 |
|Relativity.Import.Export.Core.Legacy           |   `VB.NET` |         |The import/export shared class library project.                                                                             |
|Relativity.Import.Export.Services.Interfaces   |     C#     |    X    |The import/export web-service contract project. e.g.<ul><li>enum</li><li>dto</li><li>constants</li><li>interfaces</li></ul> |
|Relativity.Import.Export.TestFramework         |     C#     |         |The import/export test framework projecty.                                                                                  |

### Resharper Team Shared Layer
To ensure that all team members use the same `Resharper` settings, `Relativity.ImportAPI-RDC.sln.DotSettings` is added to source control and adjacent to the master solution file.

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

The main build script supports an optional "-AssemblyVersion" parameter which, when assigned, passes the value to the `.\Version\Update-AssemblyInfo.ps1` script where the value is "stamped" within the source file.

### Code Analysis
Roslyn represents the future for .NET based code analysis tools. Although Microsoft.CodeAnalysis.FxCopAnalyzers is a solid replacement for FxCop, additional work is required to migrate.

***Note:** This will be re-evaluated in the near future once all source has been migrated from the `Relativity` repo.*

#### Microsoft Code Analysis (FxCop)
All of the projects within the solution are configured to support *standard* Microsoft code analysis (FxCop) for both `Debug` and `Release` builds. Because some projects are better designed than others, it makes more sense to define two separate CA rulesets:

* `.\booltools\AllErrors.ruleset`: 99.99% of the available rules are enabled.
* `.\booltools\Legacy.ruleset`: retains most design rules but relaxes globalization, naming, and usage rules.

#### StyleCop.Analyzers
The `StyleCop.Analyzers` Roslyn-based analyzer is the de-facto replacement for the original `StyleCop` extension, works flawlessly, and is integrated within *all* projects.

* `.\booltools\*.ruleset`: Controls whether SC rules are enabled or disabled.
* `.\booltools\stylecop.json`: retains most design rules but relaxes globalization, naming, and usage rules.

***Note:** The `AllErrors.ruleset` enforces virtually all SC rules and is configured for all public facing projects.*

#### Resharper Code Analysis (PowerShell only)
Over the last few years, improper async/await usage caused `ASP.NET` applications to "hang" and resulted in PD alerts. After some investigation, the Resharper community developed an extension to identify this type of violation. Not only does the extension write a "squiggly" on the offending line, but it can be invoked using the Resharper CLI tools.

The `.\Scripts\Invoke-ExtendedCodeAnalysis.ps1` PS script scans the solution and identifies all violations. The PSake-based build scripts are designed to fail the build for a single violation.

***Note:** The extension is limited to C# projects.*