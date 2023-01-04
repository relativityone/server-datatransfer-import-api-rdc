// -----------------------------------------------------------------------------------------------------
// <copyright file="FieldFileExportRequestBuilderTests.cs" company="Relativity ODA LLC">
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

	public class FieldFileExportRequestBuilderTests : ExportRequestBuilderTests
	{
		protected override ExportRequestBuilder CreateInstance(
			IFilePathProvider filePathProvider,
			IFileNameProvider fileNameProvider,
			IExportFileValidator exportFileValidator,
			IFileProcessingStatistics fileProcessingStatistics)
		{
			ExportFile exportSettings = new ExportFile(1)
				                            {
					                            FileField = new DocumentField(
						                            string.Empty,
						                            1,
						                            1,
						                            1,
						                            null,
						                            null,
						                            null,
						                            true,
						                            null,
						                            null,
						                            false)
				                            };
			return new FieldFileExportRequestBuilder(
				filePathProvider,
				fileNameProvider,
				exportFileValidator,
				fileProcessingStatistics,
				new TestNullLogger(),
				new FieldFileExportRequestFactory(exportSettings));
		}

		[Test]
		[TestCase(0, "someLocation")]
		[TestCase(1, "")]
		public void ItShouldSkipFiledFileWhenFileMetadataIsNotAvailable(int fileId, string sourceLocation)
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				FileID = fileId,
				NativeSourceLocation = sourceLocation
			};

			// ACT
			IList<ExportRequest> requests = this.Instance.Create(artifact, CancellationToken.None);

			// ASSERT
			CollectionAssert.IsEmpty(requests);
		}
	}
}