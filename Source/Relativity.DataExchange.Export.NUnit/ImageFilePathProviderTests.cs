// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageFilePathProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.TestFramework;

	public class ImageFilePathProviderTests : FilePathProviderTests
	{
		protected override string Subdirectory => "image_sub";

		protected override FilePathProvider CreateInstance(IDirectory directoryHelper, ILabelManagerForArtifact labelManager, ExportFile exportSettings)
		{
			return new ImageFilePathProvider(labelManager, exportSettings, directoryHelper, new TestNullLogger());
		}
	}
}