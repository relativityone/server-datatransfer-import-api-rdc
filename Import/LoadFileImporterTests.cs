
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Status;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Import
{
	public class LoadFileImporterTests
	{
		public class LoadFileImporterTest : LoadFileImporter
		{
			public bool InitManagersCheck { get; set; }

			public LoadFileImporterTest(ImportContext context, ITransferConfig config, IImportBatchJobFactory batchJobBatchJobFactory, 
				IArtifactReader artifactReader, IErrorContainer errorContainer, IImportStatusManager importStatusManager) 
				: base(context, config, batchJobBatchJobFactory, errorContainer, importStatusManager)
			{
				_artifactReader = artifactReader;
			}

			protected override void InitializeManagers(LoadFile args)
			{
				InitManagersCheck = true;
			}

			protected override IArtifactReader GetArtifactReader()
			{
				return _artifactReader;
			}
		}

		private LoadFileImporterTest _subjectUnderTest;
		private LoadFile _loadFile;
		private ImportContext _importContext;

		private Mock<ITransferConfig> _transferConfigMock;
		private Mock<IImportBatchJobFactory> _importJobBatchFactoryMock;
		private Mock<IImportBatchJob> _importJobBatchMock;
		private Mock<IArtifactReader> _artifactReaderMock;
		private Mock<IErrorContainer> _errorContainerMock;
		private Mock<IImportStatusManager> _impStatusManagerMock;

		[SetUp]
		public void Init()
		{
			_importContext = new ImportContext()
			{
				Args = new LoadFile()
			};
			_importJobBatchFactoryMock = new Mock<IImportBatchJobFactory>();
			_transferConfigMock = new Mock<ITransferConfig>();
			_importJobBatchMock = new Mock<IImportBatchJob>();
			_artifactReaderMock = new Mock<IArtifactReader>();

			_errorContainerMock = new Mock<IErrorContainer>();
			_impStatusManagerMock = new Mock<IImportStatusManager>();

			_loadFile = new LoadFile();

			_subjectUnderTest = new LoadFileImporterTest(_importContext, _transferConfigMock.Object, _importJobBatchFactoryMock.Object, 
				_artifactReaderMock.Object, _errorContainerMock.Object, _impStatusManagerMock.Object);

			_artifactReaderMock.Setup(reader => reader.AdvanceRecord());
		}

		[Test]
		public void ItShouldReadOnlyHeader()
		{
			// Arrange
			_artifactReaderMock.Setup(reader => reader.HasMoreRecords).Returns(false);

			// Act
			_subjectUnderTest.ReadFile("");

			// Assert
			_artifactReaderMock.Verify(reader => reader.ReadArtifact(), Times.Never);

			Assert.That(_subjectUnderTest.InitManagersCheck, Is.EqualTo(true));
		}

		[Test]
		public void ItShouldCreateBatch()
		{
			const int maxBatchSize = 3;
			// Arrange
			_artifactReaderMock.SetupSequence(reader => reader.HasMoreRecords)
				.Returns(true)
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
			_subjectUnderTest.ReadFile("");

			// Assert
			_artifactReaderMock.Verify(reader => reader.ReadArtifact(), Times.Exactly(maxBatchSize));
			_artifactReaderMock.Verify(reader => reader.AdvanceRecord(), Times.Once);
			_artifactReaderMock.Verify(reader => reader.CountRecords(), Times.Once);

			_importJobBatchMock.Verify(job => job.Run(It.Is<ImportBatchContext>( context => context.FileMetaDataHolder.Count == maxBatchSize)));
		}
	}
}
