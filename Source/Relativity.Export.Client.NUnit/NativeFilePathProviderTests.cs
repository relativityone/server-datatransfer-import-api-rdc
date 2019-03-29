// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeFilePathProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;

    using Relativity.Import.Export.Io;
    using Relativity.Logging;

    public class NativeFilePathProviderTests : FilePathProviderTests
	{
		protected override string Subdirectory => "native_sub";

		protected override FilePathProvider CreateInstance(IDirectory directoryHelper, ILabelManagerForArtifact labelManager, ExportFile exportSettings)
		{
			return new NativeFilePathProvider(labelManager, exportSettings, directoryHelper, new NullLogger());
		}
	}
}