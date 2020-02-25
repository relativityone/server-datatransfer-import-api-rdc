// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeFileExportRequestBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.TestFramework;

	public class NativeFileExportRequestBuilderTests : ExportRequestBuilderTests
	{
		protected override ExportRequestBuilder CreateInstance(
			IFilePathProvider filePathProvider,
			IFileNameProvider fileNameProvider,
			IExportFileValidator exportFileValidator,
			IFileProcessingStatistics fileProcessingStatistics)
		{
			return new NativeFileExportRequestBuilder(filePathProvider, fileNameProvider, exportFileValidator, fileProcessingStatistics, new TestNullLogger());
		}

		[Test]
		public void ItShouldSkipNativeWithEmptyGuid()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeFileGuid = string.Empty
			};

			// ACT
			IList<ExportRequest> requests = this.Instance.Create(artifact, CancellationToken.None);

			// ASSERT
			CollectionAssert.IsEmpty(requests);
		}
	}
}