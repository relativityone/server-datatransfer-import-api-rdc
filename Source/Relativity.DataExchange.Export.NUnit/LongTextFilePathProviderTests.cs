// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextFilePathProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.TestFramework;

	public class LongTextFilePathProviderTests : FilePathProviderTests
	{
		protected override string Subdirectory => "text_sub";

		protected override FilePathProvider CreateInstance(IDirectory directoryHelper, ILabelManagerForArtifact labelManager, ExportFile exportSettings)
		{
			return new LongTextFilePathProvider(labelManager, exportSettings, directoryHelper, new TestNullLogger());
		}
	}
}