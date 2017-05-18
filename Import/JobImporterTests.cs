
using System;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Status;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Import
{
	public class JobImporterTests
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

		[SetUp]
		public void Init()
		{
			_importJobBatchFactoryMock = new Mock<IImportBatchJobFactory>();
			_transferConfigMock = new Mock<ITransferConfig>();
			_importJobBatchMock = new Mock<IImportBatchJob>();
			_artifactReaderMock = new Mock<IArtifactReader>();

			_errorContainerMock = new Mock<IErrorContainer>();
			_impStatusManagerMock = new Mock<IImportStatusManager>();
			_importSettings = new Mock<IImporterSettings>();
			_importMetadataMock = new Mock<IImportMetadata>();

			_importMetadataMock.Setup(item => item.ArtifactReader).Returns(_artifactReaderMock.Object);

			_subjectUnderTest = new ImportJob(_transferConfigMock.Object, _importJobBatchFactoryMock.Object, 
				_errorContainerMock.Object, _impStatusManagerMock.Object, 
				_importMetadataMock.Object, _importSettings.Object);

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
