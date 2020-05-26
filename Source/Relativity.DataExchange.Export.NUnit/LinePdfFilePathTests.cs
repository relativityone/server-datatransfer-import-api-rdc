// ----------------------------------------------------------------------------
// <copyright file="LinePdfFilePathTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;
    using kCura.WinEDDS.Exporters;
    using kCura.WinEDDS.LoadFileEntry;
    using Moq;
    using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
    using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Pdfs;
    using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class LinePdfFilePathTests
	{
		private LinePdfFilePath _instance;
		private ExportFile _exportSettings;
		private Mock<ILoadFileCellFormatter> _loadFileCellFormatter;
		private Mock<IFilePathTransformer> _filePathTransformer;

		[SetUp]
		public void SetUp()
		{
			this._exportSettings = new ExportFile(10);
			this._loadFileCellFormatter = new Mock<ILoadFileCellFormatter>();
			this._loadFileCellFormatter.Setup(x => x.CreatePdfCell(It.IsAny<string>(), It.IsAny<ObjectExportInfo>()))
				.Returns((string location, ObjectExportInfo artifact) => location);
			this._filePathTransformer = new Mock<IFilePathTransformer>();
			this._filePathTransformer.Setup(x => x.TransformPath(It.IsAny<string>())).Returns((string path) => path);
			this._instance = new LinePdfFilePath(this._loadFileCellFormatter.Object, this._exportSettings, this._filePathTransformer.Object, new TestNullLogger());
		}

		[Test]
		public void ShouldNotAddFilePathIfNotExportingPdfs()
		{
			// ARRANGE
			this._exportSettings.ExportPdf = false;

			DeferredEntry loadFileEntry = new DeferredEntry();
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				PdfDestinationLocation = "pdf_temp_location",
				PdfSourceLocation = "pdf_source_location"
			};

			// ACT
			this._instance.AddPdfFilePath(loadFileEntry, artifact);

			// ASSERT
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(string.Empty));
		}

		[Test]
		public void ShouldUseTempLocationWhenCopyingPdfFiles()
		{
			// ARRANGE
			const string ExpectedLocation = "pdf_temp_location";

			this._exportSettings.ExportPdf = true;
			this._exportSettings.VolumeInfo = new VolumeInfo
			{
				CopyPdfFilesFromRepository = true
			};
			DeferredEntry loadFileEntry = new DeferredEntry();
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				PdfDestinationLocation = ExpectedLocation,
				PdfSourceLocation = "pdf_source_location"
			};

			// ACT
			this._instance.AddPdfFilePath(loadFileEntry, artifact);

			// ASSERT
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(ExpectedLocation));
			this._filePathTransformer.Verify(x => x.TransformPath(ExpectedLocation), Times.Once);
		}

		[Test]
		public void ShouldUseSourceLocationWhenNotCopyingPdfFiles()
		{
			// ARRANGE
			const string ExpectedLocation = "pdf_source_location";

			this._exportSettings.ExportPdf = true;
			this._exportSettings.VolumeInfo = new VolumeInfo
			{
				CopyPdfFilesFromRepository = false
			};
			DeferredEntry loadFileEntry = new DeferredEntry();
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				PdfDestinationLocation = "pdf_temp_location",
				PdfSourceLocation = ExpectedLocation
			};

			// ACT
			this._instance.AddPdfFilePath(loadFileEntry, artifact);

			// ASSERT
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(ExpectedLocation));
		}

		[Test]
		public void ItShouldHandleEmptyLocation()
		{
			// ARRANGE
			const string ExpectedLocation = "";

			this._exportSettings.ExportPdf = true;
			this._exportSettings.VolumeInfo = new VolumeInfo
			{
				CopyPdfFilesFromRepository = true
			};
			DeferredEntry loadFileEntry = new DeferredEntry();
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				PdfDestinationLocation = ExpectedLocation,
				PdfSourceLocation = "pdf_source_location"
			};

			// ACT
			this._instance.AddPdfFilePath(loadFileEntry, artifact);

			// ASSERT
			Assert.That(LoadFileTestHelpers.GetStringFromEntry(loadFileEntry), Is.EqualTo(ExpectedLocation));
			this._filePathTransformer.Verify(x => x.TransformPath(It.IsAny<string>()), Times.Never);
		}
	}
}