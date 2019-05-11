// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeFilePathProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using kCura.WinEDDS;

    using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
    using Relativity.DataExchange.Io;
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