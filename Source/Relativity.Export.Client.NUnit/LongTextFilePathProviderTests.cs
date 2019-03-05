// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextFilePathProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;

    using Relativity.Logging;

    public class LongTextFilePathProviderTests : FilePathProviderTests
	{
		protected override string Subdirectory => "text_sub";

		protected override FilePathProvider CreateInstance(IDirectoryHelper directoryHelper, ILabelManagerForArtifact labelManager, ExportFile exportSettings)
		{
			return new LongTextFilePathProvider(labelManager, exportSettings, directoryHelper, new NullLogger());
		}
	}
}