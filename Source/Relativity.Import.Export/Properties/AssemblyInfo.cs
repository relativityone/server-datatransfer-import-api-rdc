// -----------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyProduct("Relativity Import Export Framework")]
[assembly: AssemblyTitle("Relativity.Import.Export")]
[assembly: AssemblyDescription("A Relativity import export shared class library.")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]
[assembly: InternalsVisibleTo("Relativity.Import.Client")]
[assembly: InternalsVisibleTo("Relativity.Import.Export.NUnit")]
[assembly: InternalsVisibleTo("Relativity.Import.Export.NUnit.Integration")]
[assembly: InternalsVisibleTo("Relativity.Import.Export.Legacy")]
[assembly: InternalsVisibleTo("Relativity.Export.Client")]
[assembly: InternalsVisibleTo("Relativity.Import.Client.NUnit")]
[assembly: InternalsVisibleTo("Relativity.Desktop.Client")]