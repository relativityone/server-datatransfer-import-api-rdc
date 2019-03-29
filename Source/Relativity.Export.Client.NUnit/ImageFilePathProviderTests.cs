// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageFilePathProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;

    using Relativity.Import.Export.Io;
    using Relativity.Logging;

    public class ImageFilePathProviderTests : FilePathProviderTests
	{
		protected override string Subdirectory => "image_sub";

		protected override FilePathProvider CreateInstance(IDirectory directoryHelper, ILabelManagerForArtifact labelManager, ExportFile exportSettings)
		{
			return new ImageFilePathProvider(labelManager, exportSettings, directoryHelper, new NullLogger());
		}
	}
}