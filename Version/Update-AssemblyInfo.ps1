[CmdletBinding()]
param(
	[Parameter()]
	[String]$assembly_version = "1.0.0.0",
	[Parameter()]
	[String]$path_to_version_folder = (Split-Path -Parent $MyInvocation.MyCommand.Path)
)

$cs_assemblyInfo = @"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Update-AssemblyInfo.ps1
//
//     Changes to this file must be made in that script
// </auto-generated>
//------------------------------------------------------------------------------

using System.Reflection;
using System.Resources;

[assembly: AssemblyProduct("Relativity Import Client")]
[assembly: AssemblyCopyright("Relativity")]
[assembly: AssemblyTrademark("Relativity")]
[assembly: AssemblyCulture("")]

// Note: do NOT modify these values! This is automatically stamped by TFS during the CI builds.
[assembly: AssemblyVersion("$assembly_version")]
[assembly: AssemblyFileVersion("$assembly_version")]
[assembly: AssemblyConfiguration("Release")]

[assembly: AssemblyCompany("Relativity ODA LLC")]
[assembly: NeutralResourcesLanguage("en-US")]
"@

$vb_assemblyInfo = @"
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by Update-AssemblyInfo.ps1
'
'     Changes to this file must be made in that script
' </auto-generated>
'------------------------------------------------------------------------------

Imports System.Reflection
Imports System.Resources

<assembly: AssemblyProduct("Relativity Import Client")>
<assembly: AssemblyCopyright("Relativity")>
<assembly: AssemblyTrademark("Relativity")>
<assembly: AssemblyCulture("")>

' Note: do NOT modify these values! This is automatically stamped by TFS during the CI builds.
<assembly: AssemblyVersion("$assembly_version")>
<assembly: AssemblyFileVersion("$assembly_version")>
<assembly: AssemblyConfiguration("Release")>

<assembly: AssemblyCompany("Relativity ODA LLC")>
<assembly: NeutralResourcesLanguage("en-US")>
"@

$path_to_AssemblyInfo_cs = Join-Path $path_to_version_folder "AssemblySharedInfo.cs"
$path_to_AssemblyInfo_vb = Join-Path $path_to_version_folder "AssemblySharedInfo.vb"
$cs_assemblyInfo | Set-Content $path_to_AssemblyInfo_cs
$vb_assemblyInfo | Set-Content $path_to_AssemblyInfo_vb