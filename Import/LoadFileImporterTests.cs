
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Import
{
	public class LoadFileImporterTests
	{
		public class LoadFileImporterTest : LoadFileImporter
		{
			public bool InitManagersCheck { get; set; }

			public LoadFileImporterTest(ImportContext context, ITransferConfig config, IImportBatchJobFactory batchJobBatchJobFactory, IArtifactReader artifactReader) : base(context, config, batchJobBatchJobFactory, artifactReader)
			{
			}

			protected override void InitializeManagers(LoadFile args)
			{
				InitManagersCheck = true;
			}
		}


		private LoadFileImporterTest _subjectUnderTest;
		private LoadFile _loadFile;
		private ImportContext _importContext;

		private Mock<ITransferConfig> _transferConfigMock;
		private Mock<IImportBatchJobFactory> _importJobBatchFactory;
		private Mock<IImportBatchJob> _importJobBatch;
		private Mock<IArtifactReader> _artifactReader;

		[SetUp]
		public void Init()
		{
			_importContext = new ImportContext()
			{
				Args = new LoadFile()
			};
			_importJobBatchFactory = new Mock<IImportBatchJobFactory>();
			_transferConfigMock = new Mock<ITransferConfig>();
			_importJobBatch = new Mock<IImportBatchJob>();
			_artifactReader = new Mock<IArtifactReader>();

			_loadFile = new LoadFile();

			_subjectUnderTest = new LoadFileImporterTest(_importContext, _transferConfigMock.Object, _importJobBatchFactory.Object, _artifactReader.Object);

			_artifactReader.Setup(reader => reader.AdvanceRecord());
		}

		[Test]
		public void ItShouldReadOnlyHeader()
		{
			// Arrange
			_artifactReader.Setup(reader => reader.HasMoreRecords).Returns(false);

			// Act
			_subjectUnderTest.ReadFile("");

			// Assert
			_artifactReader.Verify(reader => reader.ReadArtifact(), Times.Never);

			Assert.That(_subjectUnderTest.InitManagersCheck, Is.EqualTo(true));
		}

		[Test]
		public void ItShouldCreateBatch()
		{
			const int maxBatchSize = 3;
			// Arrange
			_artifactReader.SetupSequence(reader => reader.HasMoreRecords)
				.Returns(true)
				.Returns(true)
				.Returns(true)
				.Returns(true)
				.Returns(false);
			_artifactReader.Setup(reader => reader.AdvanceRecord());
			_artifactReader.Setup(reader => reader.ReadArtifact()).Returns(new ArtifactFieldCollection());

			_importJobBatchFactory.Setup(jobFactory => jobFactory.Create(It.IsAny<ImportBatchContext>()))
				.Returns(_importJobBatch.Object);

			_transferConfigMock.SetupGet(config => config.ImportBatchSize).Returns(maxBatchSize);


			// Act
			_subjectUnderTest.ReadFile("");

			// Assert
			_artifactReader.Verify(reader => reader.ReadArtifact(), Times.Exactly(maxBatchSize));
			_artifactReader.Verify(reader => reader.AdvanceRecord(), Times.Once);

			_importJobBatch.Verify(job => job.Run(It.Is<ImportBatchContext>( context => context.FileMetaDataHolder.Count == maxBatchSize)));
		}
	}
}
