// ----------------------------------------------------------------------------
// <copyright file="PdfFileExportRequestBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;
	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class PdfFileExportRequestBuilderTests : ExportRequestBuilderTests
	{
		protected override ExportRequestBuilder CreateInstance(
			IFilePathProvider filePathProvider,
			IFileNameProvider fileNameProvider,
			IExportFileValidator exportFileValidator,
			IFileProcessingStatistics fileProcessingStatistics)
		{
			return new PdfFileExportRequestBuilder(filePathProvider, fileNameProvider, exportFileValidator, fileProcessingStatistics, new TestNullLogger());
		}

		[TestCase("")]
		[TestCase(null)]
		public void ItShouldSkipPdfWithEmptyOrNullGuid(string guid)
		{
			// ARRANGE
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				PdfFileGuid = guid
			};

			// ACT
			IList<ExportRequest> requests = this.Instance.Create(artifact, CancellationToken.None);

			// ASSERT
			CollectionAssert.IsEmpty(requests);
		}

		[Test]
		public void ItShouldCreateRequestWithCorrectSourceLocationAndGuid()
		{
			// ARRANGE
			const string ExpectedSourceLocation = "pdf_source_location";
			string expectedGuid = Guid.NewGuid().ToString();

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeFileGuid = Guid.NewGuid().ToString(),
				FileID = 1,
				NativeSourceLocation = "native_source_location",
				PdfFileGuid = expectedGuid,
				PdfSourceLocation = ExpectedSourceLocation
			};

			// ACT
			IList<ExportRequest> requests = this.Instance.Create(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(requests.Count, Is.EqualTo(1));
			Assert.That(requests[0].SourceLocation, Is.EqualTo(ExpectedSourceLocation));
			Assert.That(((PhysicalFileExportRequest)requests[0]).RemoteFileGuid, Is.EqualTo(expectedGuid));
		}
	}
}