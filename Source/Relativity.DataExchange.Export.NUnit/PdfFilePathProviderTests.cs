// ----------------------------------------------------------------------------
// <copyright file="PdfFilePathProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;
    using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
    using Relativity.DataExchange.Io;
    using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class PdfFilePathProviderTests : FilePathProviderTests
	{
		protected override string Subdirectory => "pdf_sub";

		protected override FilePathProvider CreateInstance(IDirectory directoryHelper, ILabelManagerForArtifact labelManager, ExportFile exportSettings)
		{
			return new PdfFilePathProvider(labelManager, exportSettings, directoryHelper, new TestNullLogger());
		}
	}
}