using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Status;
using kCura.WinEDDS.Core.Import.Tasks;
using kCura.WinEDDS.Core.NUnit.Helpers;
using Moq;
using NUnit.Framework;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Import.Tasks
{
	
	public class PrepareMetadataFilesTaskTests
	{
		private PrepareMetadataFilesTask _subjectUnderTest;
		private ImportBatchContext _importBatchContext;
		private ImportContext _importContext;
		private LoadFile _loadFile;

		private CancellationToken _cancellationToken;

		private Mock<IImporterSettings> _importerSettingsMock;
		private Mock<ICancellationProvider> _cancellationProviderMock;
		private Mock<ILog> _logMock;
		private Mock<IImportFoldersTask> _importFoldersTaskMock;
		private Mock<IImportPrepareMetadataTask> _importCreateMetadataTaskMock;
		private Mock<IImportMetadata> _importMetadataMock;
		private Mock<ITransferConfig> _transferConfigMock;

		private readonly IImportExceptionHandlerExec _importExceptionHandlerExec = ImportExceptionHandlerExecFactory.Create();

		private readonly ArtifactFieldCollection _artifactFieldCollection = new ArtifactFieldCollection
			{
				new ArtifactField("File", _ARTIFACT_ID, FieldTypeHelper.FieldType.File, FieldCategory.FileInfo, 1, 1, null, false)
			};
		private const int _ARTIFACT_ID = 1;

		[SetUp]
		public void Init()
		{
			_cancellationToken = new CancellationToken();
			_loadFile = new LoadFile { ArtifactTypeID = (int)ArtifactType.Document };

			_logMock = new Mock<ILog>();
			_importCreateMetadataTaskMock = new Mock<IImportPrepareMetadataTask>();

			_transferConfigMock = new Mock<ITransferConfig>();
			_importFoldersTaskMock = new Mock<IImportFoldersTask>();
			_cancellationProviderMock = new Mock<ICancellationProvider>();
			_importMetadataMock = new Mock<IImportMetadata>();

			_importerSettingsMock = new Mock<IImporterSettings>();
			_importerSettingsMock.Setup(obj => obj.LoadFile).Returns(_loadFile);

			_importContext = new ImportContext();

			_importBatchContext = new ImportBatchContext(_importContext, 1)
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

			_subjectUnderTest = new PrepareMetadataFilesTask(_importBatchContext, _importExceptionHandlerExec, _cancellationProviderMock.Object,
				_importFoldersTaskMock.Object, _importCreateMetadataTaskMock.Object, _importMetadataMock.Object, _transferConfigMock.Object, _logMock.Object);
		}

		[Test]
		public void ItShouldNotCreateMetadataForInvalidRecords()
		{
			// Arrange
			var uploadResults = new Dictionary<FileMetadata, UploadResult>
			{
				{_importBatchContext.FileMetaDataHolder[0], new UploadResult {Success = false}}
			};

			_importMetadataMock.Setup(obj => obj.InitNewMetadataProcess()).Returns(new MetadataFilesInfo() {BatchSize = 1000});
			// Act
			_subjectUnderTest.Execute(uploadResults);

			// Assert
			_importCreateMetadataTaskMock.Verify(obj => obj.Execute(It.IsAny<FileMetadata>(), It.IsAny<ImportBatchContext>()), Times.Never);
			_importMetadataMock.Verify(obj => obj.InitNewMetadataProcess(), Times.Once);

			Assert.That(_importBatchContext.MetadataFilesInfo.Count, Is.EqualTo(1));
			Assert.That(_importBatchContext.MetadataFilesInfo[0], Is.Not.Null);
		}

		[Test]
		public void ItShouldCreateMetadata()
		{
			// Arrange
			const int batchSize = 1000;
			var uploadResults = new Dictionary<FileMetadata, UploadResult>
			{
				{_importBatchContext.FileMetaDataHolder[0], new UploadResult {Success = true}},
				{_importBatchContext.FileMetaDataHolder[1], new UploadResult {Success = true}}
			};

			_importMetadataMock.Setup(obj => obj.InitNewMetadataProcess()).Returns(new MetadataFilesInfo() { BatchSize = batchSize });

			_transferConfigMock.Setup(obj => obj.ImportBatchMaxVolume).Returns(batchSize);

			_importMetadataMock.Setup(obj => obj.CurrentMetadataFileStreamLength()).Returns(batchSize - 1);
			// Act
			_subjectUnderTest.Execute(uploadResults);

			// Assert
			_importMetadataMock.Verify(obj => obj.InitNewMetadataProcess(), Times.Once);

			_importFoldersTaskMock.Verify(obj => obj.Execute(_importBatchContext.FileMetaDataHolder[0], _importBatchContext), Times.Once);
			_importFoldersTaskMock.Verify(obj => obj.Execute(_importBatchContext.FileMetaDataHolder[1], _importBatchContext), Times.Once);

			_importCreateMetadataTaskMock.Verify(obj => obj.Execute(_importBatchContext.FileMetaDataHolder[0], _importBatchContext), Times.Once);
			_importCreateMetadataTaskMock.Verify(obj => obj.Execute(_importBatchContext.FileMetaDataHolder[1], _importBatchContext), Times.Once);
			Assert.That(_importBatchContext.MetadataFilesInfo.Count, Is.EqualTo(1));
			Assert.That(_importBatchContext.MetadataFilesInfo[0], Is.Not.Null);
		}

		[Test]
		public void ItShouldCheckBcpFileSize()
		{
			// Arrange
			const int batchSize = 1000;
			var uploadResults = new Dictionary<FileMetadata, UploadResult>
			{
				{_importBatchContext.FileMetaDataHolder[0], new UploadResult {Success = true}}
			};

			_importMetadataMock.Setup(obj => obj.InitNewMetadataProcess()).Returns(new MetadataFilesInfo() { BatchSize = batchSize });

			_transferConfigMock.Setup(obj => obj.ImportBatchMaxVolume).Returns(batchSize);

			// we exceed total metadata file chunk size
			_importMetadataMock.Setup(obj => obj.CurrentMetadataFileStreamLength()).Returns(batchSize + 1);
			// Act
			_subjectUnderTest.Execute(uploadResults);

			// Assert
			_importMetadataMock.Verify(obj => obj.InitNewMetadataProcess(), Times.Exactly(uploadResults.Count + 1));
			_importMetadataMock.Verify(obj => obj.EndMetadataProcess(), Times.Exactly(uploadResults.Count + 1));
		}
	}
}
