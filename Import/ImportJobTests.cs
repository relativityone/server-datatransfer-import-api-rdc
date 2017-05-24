
using System;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Factories;
using kCura.WinEDDS.Core.Import.Status;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Import
{
	public class ImportJobTests
	{
		private ImportJob _subjectUnderTest;

		private Mock<ITransferConfig> _transferConfigMock;
		private Mock<IImportBatchJobFactory> _importJobBatchFactoryMock;
		private Mock<IImportBatchJob> _importJobBatchMock;
		private Mock<IArtifactReader> _artifactReaderMock;
		private Mock<IErrorContainer> _errorContainerMock;
		private Mock<IImportStatusManager> _impStatusManagerMock;
		private Mock<IImportMetadata> _importMetadataMock;
		private Mock<IImporterSettings> _importSettings;
		private ImportExceptionHandlerExec _importExceptionHandlerExec;

		private LoadFile _loadFile;

		[SetUp]
		public void Init()
		{
			_importJobBatchFactoryMock = new Mock<IImportBatchJobFactory>();
			_transferConfigMock = new Mock<ITransferConfig>();
			_importJobBatchMock = new Mock<IImportBatchJob>();
			_artifactReaderMock = new Mock<IArtifactReader>();

			_impStatusManagerMock = new Mock<IImportStatusManager>();
			_importSettings = new Mock<IImporterSettings>();
			_importMetadataMock = new Mock<IImportMetadata>();
			_errorContainerMock = new Mock<IErrorContainer>();

			_importMetadataMock.Setup(item => item.ArtifactReader).Returns(_artifactReaderMock.Object);

			_importExceptionHandlerExec = new ImportExceptionHandlerExec(_impStatusManagerMock.Object, _importMetadataMock.Object,
				_errorContainerMock.Object);

			_subjectUnderTest = new ImportJob(_transferConfigMock.Object, _importJobBatchFactoryMock.Object, _impStatusManagerMock.Object, 
				_importMetadataMock.Object, _importSettings.Object, _importExceptionHandlerExec);

			_artifactReaderMock.Setup(reader => reader.AdvanceRecord());

			_loadFile = new LoadFile();
			_importSettings.SetupGet(obj => obj.LoadFile).Returns(_loadFile);
		}
		
		[Test]
		public void ItShouldReadOnlyHeader()
		{
			// Arrange
			_artifactReaderMock.Setup(reader => reader.HasMoreRecords).Returns(false);

			// Act
			var actualRes = (bool?)_subjectUnderTest.ReadFile("");

			// Assert
			_artifactReaderMock.Verify(reader => reader.ReadArtifact(), Times.Never);
			Assert.That(actualRes.Value);
			ValidateSharedConditions();
		}

		[Test]
		public void ItShouldCreateBatch()
		{
			const int maxBatchSize = 3;
			_loadFile.StartLineNumber = 1000;
			
			// Arrange
			_artifactReaderMock.SetupSequence(reader => reader.HasMoreRecords)
				.Returns(true)
				.Returns(false) // Here we should only call once AdvanceRecord methos on artifact reader to move record index(init import)
				.Returns(true)  // Here we should read lines "maxBatchSize"-times
				.Returns(true)
				.Returns(true)
				.Returns(true)
				.Returns(false);
			_artifactReaderMock.Setup(reader => reader.AdvanceRecord());
			_artifactReaderMock.Setup(reader => reader.ReadArtifact()).Returns(new ArtifactFieldCollection());
			_artifactReaderMock.Setup(reader => reader.CountRecords()).Returns(maxBatchSize);

			_importJobBatchFactoryMock.Setup(jobFactory => jobFactory.Create(It.IsAny<ImportBatchContext>()))
				.Returns(_importJobBatchMock.Object);

			_transferConfigMock.SetupGet(config => config.ImportBatchSize).Returns(maxBatchSize);

			// Act
			var actualRes = (bool?)_subjectUnderTest.ReadFile("");

			// Assert
			_artifactReaderMock.Verify(reader => reader.ReadArtifact(), Times.Exactly(maxBatchSize));
			_artifactReaderMock.Verify(reader => reader.AdvanceRecord(), Times.Once);
			_artifactReaderMock.Verify(reader => reader.CountRecords(), Times.Once);

			_importJobBatchMock.Verify(job => job.Run(It.Is<ImportBatchContext>( context => context.FileMetaDataHolder.Count == maxBatchSize)));

			Assert.That(actualRes.Value);

			ValidateSharedConditions();
		}

		[Test]
		[TestCase(-1, true)]
		[TestCase(0, false)]
		[TestCase(1, false)]
		public void ItShouldCheckRecordCount(int recCount, bool raiseCancelEvent)
		{
			// Arrange

			_artifactReaderMock.Setup(reader => reader.HasMoreRecords).Returns(false);
			_artifactReaderMock.Setup(reader => reader.CountRecords()).Returns(recCount);
			// Act
			var actualResult = (bool?)_subjectUnderTest.ReadFile("");

			// Assert
			var expectedCallCount = raiseCancelEvent ? new Func<Times>(Times.Once) : Times.Never;

			_impStatusManagerMock.Verify(obj => obj.RaiseStatusUpdateEvent(_subjectUnderTest, StatusUpdateType.Progress, "cancel import", 
				It.IsAny<int>()), expectedCallCount);

			Assert.That(actualResult.Value, Is.EqualTo(!raiseCancelEvent));

			ValidateSharedConditions();
		}

		[Test]
		[TestCase(true, "\\Natives", "Aspera")]
		[TestCase(false, "", "not copied")]
		[TestCase(false, null, "not copied")]
		[TestCase(false, "\\Natives", "not copied")]
		public void ItShouldValidateUploadMode(bool copyFilesToDocumentRepository, string nativeFilePathColumn, string filesModeDesac)
		{
			// Arrange
			string expectedDesc = $"Metadata: Aspera - Files: {filesModeDesac}";

			_loadFile.CopyFilesToDocumentRepository = copyFilesToDocumentRepository;
			_loadFile.NativeFilePathColumn = nativeFilePathColumn;
			_artifactReaderMock.Setup(reader => reader.HasMoreRecords).Returns(false);
			_artifactReaderMock.Setup(reader => reader.CountRecords()).Returns(1);

			//Act
			_subjectUnderTest.ReadFile("");

			//Assert
			_impStatusManagerMock.Verify(obj => obj.RaiseTranserModeChangedEvent(_subjectUnderTest, expectedDesc), Times.Once);
			ValidateSharedConditions();
		}

		private void ValidateSharedConditions()
		{
			_impStatusManagerMock.Verify(obj => obj.RaiseStartImportEvent(_subjectUnderTest), Times.Once);
			_importMetadataMock.Verify(obj => obj.CleanUp(), Times.Once);
		}
	}
}
