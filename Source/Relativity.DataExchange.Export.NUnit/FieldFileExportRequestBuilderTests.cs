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
	using Relativity.Logging;

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
				new NullLogger(),
				new FieldFileExportRequestFactory(exportSettings));
		}

		[Test]
		public void ItShouldSkipFieldFileWithMissingId()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				FileID = 0,
				NativeSourceLocation = "location"
			};

			// ACT
			IList<ExportRequest> requests = this.Instance.Create(artifact, CancellationToken.None);

			// ASSERT
			CollectionAssert.IsEmpty(requests);
		}

		[Test]
		public void ItShouldSkipFieldFileWithEmptyLocation()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				FileID = 1,
				NativeSourceLocation = string.Empty
			};

			// ACT
			IList<ExportRequest> requests = this.Instance.Create(artifact, CancellationToken.None);

			// ASSERT
			CollectionAssert.IsEmpty(requests);
		}
	}
}