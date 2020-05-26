// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportRequestBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;

	[TestFixture]
	public abstract class ExportRequestBuilderTests
	{
		private const string FileName = "file_name";
		private const string ExportPath = "export_path";

		private Mock<IFilePathProvider> _filePathProvider;
		private Mock<IFileNameProvider> _fileNameProvider;
		private Mock<IExportFileValidator> _validator;
		private Mock<IFileProcessingStatistics> _fileProcessingStatistics;

		protected ExportRequestBuilder Instance { get; private set; }

		[SetUp]
		public void SetUp()
		{
			this._filePathProvider = new Mock<IFilePathProvider>();
			this._fileNameProvider = new Mock<IFileNameProvider>();
			this._validator = new Mock<IExportFileValidator>();
			this._fileProcessingStatistics = new Mock<IFileProcessingStatistics>();

			this._fileNameProvider.Setup(x => x.GetName(It.IsAny<ObjectExportInfo>())).Returns(FileName);
			this._fileNameProvider.Setup(x => x.GetPdfName(It.IsAny<ObjectExportInfo>())).Returns(FileName);
			this._filePathProvider.Setup(x => x.GetPathForFile(FileName, It.IsAny<int>())).Returns(ExportPath);
			this._validator.Setup(x => x.CanExport(ExportPath, It.IsAny<string>())).Returns(true);

			this.Instance = this.CreateInstance(this._filePathProvider.Object, this._fileNameProvider.Object, this._validator.Object, this._fileProcessingStatistics.Object);
		}

		protected abstract ExportRequestBuilder CreateInstance(
			IFilePathProvider filePathProvider,
			IFileNameProvider fileNameProvider,
			IExportFileValidator exportFileValidator,
			IFileProcessingStatistics fileProcessingStatistics);

		[Test]
		public void ItShouldUpdateStatisticsAndSkipImageWhenCannotExport()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeFileGuid = Guid.NewGuid().ToString(),
				FileID = 1,
				NativeSourceLocation = "source_location",
				PdfFileGuid = Guid.NewGuid().ToString(),
				PdfSourceLocation = "pdf_source_location"
			};

			this._validator.Setup(x => x.CanExport(ExportPath, It.IsAny<string>())).Returns(false);

			// ACT
			IList<ExportRequest> requests = this.Instance.Create(artifact, CancellationToken.None);

			// ASSERT
			CollectionAssert.IsEmpty(requests);
			this._fileProcessingStatistics.Verify(x => x.UpdateStatisticsForFile(ExportPath));
		}

		[Test]
		public void ItShouldCreateRequests()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				NativeFileGuid = Guid.NewGuid().ToString(),
				FileID = 1,
				NativeSourceLocation = "source_location",
				PdfFileGuid = Guid.NewGuid().ToString(),
				PdfSourceLocation = "pdf_source_location"
			};

			// ACT
			IList<ExportRequest> requests = this.Instance.Create(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(requests.Count, Is.EqualTo(1));
			Assert.That(requests[0].DestinationLocation, Is.EqualTo(ExportPath));
		}
	}
}