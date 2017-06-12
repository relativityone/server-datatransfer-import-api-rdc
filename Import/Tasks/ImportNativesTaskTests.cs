
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Factories;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Status;
using kCura.WinEDDS.Core.Import.Tasks;
using kCura.WinEDDS.Core.NUnit.Helpers;
using Moq;
using NUnit.Framework;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Import.Tasks
{
	public class ImportNativesTaskTests
	{
		private ImportNativesTask _subjectUnderTest;
		private ImportBatchContext _importBatchContext;
		private ImportContext _importContext;
		private LoadFile _loadFile;
		private CancellationToken _cancellationToken;

		private Mock<IImporterSettings> _importerSettingsMock;
		private Mock<IFileUploaderFactory> _fileUploaderFactoryMock;
		private Mock<IImportNativesAnalyzer> _importNativesAnalyzerMock;
		private Mock<IRepositoryFilePathHelper> _repositoryFilePathHelperMock;
		private readonly IImportExceptionHandlerExec _importExceptionHandlerExec = ImportExceptionHandlerExecFactory.Create();
		private Mock<IUploadErrors> _uploadErrorsMock;
		private Mock<ICancellationProvider> _cancellationProviderMock;
		private Mock<IFileUploader> _fileUploaderMock;
		private Mock<ILog> _logMock;
		private readonly ArtifactFieldCollection _artifactFieldCollection = new ArtifactFieldCollection
			{
				new ArtifactField("File", _ARTIFACT_ID, FieldTypeHelper.FieldType.File, FieldCategory.FileInfo, 1, 1, null, false)
			};
		private const int _ARTIFACT_ID = 1;

		[SetUp]
		public void Init()
		{
			_artifactFieldCollection[_ARTIFACT_ID].Value = "FileName";

			_fileUploaderFactoryMock = new Mock<IFileUploaderFactory>();
			_importNativesAnalyzerMock = new Mock<IImportNativesAnalyzer>();
			_repositoryFilePathHelperMock = new Mock<IRepositoryFilePathHelper>();
			_uploadErrorsMock = new Mock<IUploadErrors>();
			_cancellationProviderMock = new Mock<ICancellationProvider>();
			_fileUploaderMock = new Mock<IFileUploader>();

			_logMock = new Mock<ILog>();

			_subjectUnderTest = new ImportNativesTask(_fileUploaderFactoryMock.Object, _importNativesAnalyzerMock.Object,
				_repositoryFilePathHelperMock.Object, _importExceptionHandlerExec, _uploadErrorsMock.Object,
				_cancellationProviderMock.Object, _logMock.Object);


			_loadFile = new LoadFile {ArtifactTypeID = (int) ArtifactType.Document};

			_importerSettingsMock = new Mock<IImporterSettings>();
			_importerSettingsMock.Setup(obj => obj.LoadFile).Returns(_loadFile);

			_cancellationToken = new CancellationToken();
			_cancellationProviderMock.Setup(obj => obj.GetToken()).Returns(_cancellationToken);


			_importContext = new ImportContext()
			{
				Settings = _importerSettingsMock.Object
			};

			_importBatchContext = new ImportBatchContext(_importContext, 1000)
			{
				FileMetaDataHolder =
				{
					new FileMetadata()
					{
						LineNumber = 1,
						UploadFile = true,
						ArtifactFieldCollection = _artifactFieldCollection,
					},
					new FileMetadata()
					{
						LineNumber = 2,
						UploadFile = true,
						ArtifactFieldCollection = _artifactFieldCollection
					}
				}
			};
		}

		[Test]
		public void ItShouldProcessedDocsWithoutNatives()
		{
			// Arrange
			_loadFile.CopyFilesToDocumentRepository = false;

			// Act
			IDictionary<FileMetadata, UploadResult> fileMetadaResults = _subjectUnderTest.Execute(_importBatchContext);
			
			// Assert
			// we don't expect any upload happened 
			_fileUploaderFactoryMock.Verify(obj => obj.CreateNativeFileUploader(), Times.Never);
			_uploadErrorsMock.Verify(obj => obj.HandleUploadErrors(fileMetadaResults), Times.Once);

			Assert.That(fileMetadaResults.Count, Is.EqualTo(_importBatchContext.FileMetaDataHolder.Count));
			Assert.That(fileMetadaResults.All(item => item.Value.Success));
		}

		[Test]
		public void ItShouldProcessedDocsWithNatives()
		{
			// Arrange
			string destinationDir = "SomePath";

			_loadFile.CopyFilesToDocumentRepository = true;

			_fileUploaderFactoryMock.Setup(obj => obj.CreateNativeFileUploader()).Returns(_fileUploaderMock.Object);

			// Simulate file exists
			FileMetadata notUploadedFileMeta = _importBatchContext.FileMetaDataHolder[0];
			notUploadedFileMeta.FileExists = false;
			_importNativesAnalyzerMock.Setup(obj => obj.Process(notUploadedFileMeta))
				.Returns(notUploadedFileMeta);

			// Simulate file does not exist
			FileMetadata uploadedFileMeta = _importBatchContext.FileMetaDataHolder[1];

			uploadedFileMeta.FileExists = true;
			_importNativesAnalyzerMock.Setup(obj => obj.Process(uploadedFileMeta))
				.Returns(uploadedFileMeta);

			_repositoryFilePathHelperMock.Setup(obj => obj.GetNextDestinationDirectory()).Returns(destinationDir);

			var results = new Dictionary<FileMetadata, UploadResult>
			{
				{ uploadedFileMeta, new UploadResult { Success = true} }
			};
			_fileUploaderMock.Setup(obj => obj.WaitForUploadToComplete()).Returns(results);

			// Act
			IDictionary<FileMetadata, UploadResult> fileMetadaResults = _subjectUnderTest.Execute(_importBatchContext);

			// Assert

			// we expect upload happened
			_fileUploaderFactoryMock.Verify(obj => obj.CreateNativeFileUploader(), Times.Once);
			_uploadErrorsMock.Verify(obj => obj.HandleUploadErrors(fileMetadaResults), Times.Once);

			// one file should be only uploaded
			_fileUploaderMock.Verify(obj => obj.UploadFile(uploadedFileMeta), Times.Once);
			_fileUploaderMock.Verify(obj => obj.WaitForUploadToComplete(), Times.Once);

			// we still expect all FileMetadata objs should be returned as we want to porcessed metadata later for all docs
			Assert.That(fileMetadaResults.Count, Is.EqualTo(_importBatchContext.FileMetaDataHolder.Count));
			Assert.That(uploadedFileMeta.DestinationDirectory, Is.EqualTo(destinationDir));

			Assert.That(fileMetadaResults.All(item => item.Value.Success));
		}
	}
}
