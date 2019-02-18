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
* ConfigureAwait Checker Resharper Extension ()

## Building the master solution
Generally speaking, PowerShell is used to perform initial and pre-commit builds and Visual Studio for interim builds.

### PowerShell
1. Open a Windows PowerShell window
2. Navigate to the root of the repository
3. Use any of the build commands below

```bash
# builds the solution, standard + extended CA checks, and StyleCop verification
.\build.ps1 -ExtendedCodeAnalysis
```

### Visual Studio
1. Open the *Relativity.ImportAPI-RDC.sln* solution
2. Open the .paket solution folder
3. Right-click the paket.dependencies file and click the Restore menu item
4. Click CTRL+SHIFT+B to build the solution

***Note:** Paket for Visual Studio adds Update|Install|Restore menu items when right-clicking a paket.dependencies file.*

## Unit and Integration Tests
TBD.

```bash
# builds the solution, standard + extended CA checks, and StyleCop verification
.\build.ps1 -UnitTests -IntegrationTests
```

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
|Relativity.Desktop.Client.Legacy               |   VB.NET   |    X    |The RDC application/EXE project.                                                                                            |
|Relativity.Desktop.Client.Legacy.NUnit         |   VB.NET   |         |The RDC unit test project.                                                                                                  |
|Relativity.Desktop.Client.NUnit.Integration    |     C#     |         |The RDC integration test project.                                                                                           |
|Relativity.Export.Client                       |     C#     |         |The export API project.                                                                                                     |
|Relativity.Export.Client.NUnit                 |     C#     |         |The export API unit test project.                                                                                           |
|Relativity.Import.Client                       |     C#     |    X    |The import API project.                                                                                                     |
|Relativity.Import.Client.Legacy.NUnit          |   VB.NET   |         |The import API unit test project.                                                                                           |
|Relativity.Import.Client.NUnit                 |     C#     |         |The import API unit test project.                                                                                           |
|Relativity.Import.Client.NUnit.Integration     |     C#     |         |The C# import API integration test project.                                                                                 |
|Relativity.Import.Client.Samples.NUnit         |     C#     |    X    |The C# import API Github samples test project.                                                                              |
|Relativity.Import.Export.Core                  |     C#     |         |The C# import/export class library project.                                                                                 |
|Relativity.Import.Export.Core.Legacy           |   VB.NET   |         |The import/export shared class library project.                                                                             |
|Relativity.Import.Export.Services.Interfaces   |     C#     |    X    |The import/export web-service contract project. e.g.<ul><li>enum</li><li>dto</li><li>constants</li><li>interfaces</li></ul> |
|Relativity.Import.Export.TestFramework         |     C#     |         |The import/export test framework projecty.                                                                                  |

### Paket and NuGet Packages
The bootstrapped tools and master solution build uses [Paket](https://fsprojects.github.io/Paket/) to support all package requirements. The repo contains 3 key Paket file types:

* **paket.dependencies:** The file that defines all packages contained within the repo.
* **paket.lock:** The file that gets updated whenever dependencies are added or changed. The advantage with adding this file to source control is that packages can simply be restored.
* **paket.references:** Each project includes this file to specify all package references.

### Assembly Versioning
All C# and VB.NET projects contained within the master solution link in one of the following source files to control the assembly and file versions:

* **AssemblySharedInfo.cs:**
* **AssemblySharedInfo.vb:**

The main build script supports an optional "-AssemblyVersion" value which, when set by a Jenkins pipeline, passes the value to the **.\Version\Update-AssemblyInfo.ps1** script where the value is "stamped" into source file.

### Code Analysis
Roslyn represents the future for .NET based code analysis tools. Although Microsoft.CodeAnalysis.FxCopAnalyzers is a solid replacement for FxCop, it's 1 good release from being a viable FxCop replacement.

***Note:** This will be re-evaluated once Visual Studio 2019 ships in April 2019.*

#### Microsoft Code Analysis (FxCop)
All of the projects within the solution are configured to support *standard* Microsoft code analysis (FxCop) for both debug and release builds. Because some projects are better designed 
than others, it makes more sense to define two separate CA rulesets:

* **.\booltools\AllErrors.ruleset**: 99.99% of the available rules are enabled.
* **.\booltools\Legacy.ruleset**: retains most design rules but relaxes globalization, naming, and usage rules.

#### StyleCop.Analyzers
The StyleCop.Analyzers Roslyn-based analyzer is the de-facto replacement for the original StyleCop extension, works flawlessly, and is integrated within *all* projects.

* **.\booltools\*.ruleset**: Controls whether SC rules are enabled or disabled.
* **.\booltools\stylecop.json**: retains most design rules but relaxes globalization, naming, and usage rules.

***Note:** The AllErrors.ruleset enforces virtually all SC rules and is configured for all public facing projects.*

#### Resharper Code Analysis (PowerShell only)
Over the last few years, improper async/await usage caused ASP.NET applications to "hang" and resulted in PD alerts. After some investigation, somebody within the Resharper community had already developed an extension for this *exact* problem. Not only does the extension write a "squiggly" on the offending line, but it can be invoked rather easily using the Resharper CLI tools.

The **.\Scripts\Invoke-ExtendedCodeAnalysis.ps1** PS script scans the solution and identifies all violations. The PSake-based build scripts are designed to fail the build for a single violation.

***Note:** The extension is limited to C# projects.*