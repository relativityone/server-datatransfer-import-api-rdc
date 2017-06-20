
using System;
using kCura.EDDS.WebAPI.BulkImportManagerBase;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Statistics;
using kCura.WinEDDS.Core.Import.Status;
using Moq;
using NUnit.Framework;
using Relativity;

namespace kCura.WinEDDS.Core.NUnit.Import.Statistics
{
	public class StatisticsManagerTests
	{
		private StatisticsManager _statisticsManager;
		private LoadFile _loadFile;

		private Mock<IImporterSettings> _importerSettingsMock;
		private Mock<IImportStatusManager> _importStatusManagerMock;
		private Mock<ITransferConfig> _transferConfigMock;
		private Mock<INativeFilesStatisticsHandler> _nativeFilesStatisticsHandlerMock;
		private Mock<IMetadataFilesStatisticsHandler> _metaFileStatsHandlerMock;
		private Mock<IMetadataStatisticsHandler> _metadataStatisticsHandlerMock;
		private Mock<IBulkImportStatisticsHandler> _bulkImportStatisticsHandlerMock;

		private Mock<IImportMetadata> _importMetadataMock;
		private Mock<IServerErrorStatisticsHandler> _serverErrorStatisticsHandlerMock;
		private Mock<IJobFinishStatisticsHandler> _jobFinishStatisticsHandlerMock;

		private WinEDDS.Statistics _stats;

		private const int _BATCH_SIZE = 100;
		private const int _START_LINE = 1;

		[SetUp]
		public void Init()
		{
			_stats = new WinEDDS.Statistics();
			_transferConfigMock = new Mock<ITransferConfig>();
			_importStatusManagerMock = new Mock<IImportStatusManager>();

			_nativeFilesStatisticsHandlerMock = new Mock<INativeFilesStatisticsHandler>();
			_metaFileStatsHandlerMock = new Mock<IMetadataFilesStatisticsHandler>();
			_metadataStatisticsHandlerMock = new Mock<IMetadataStatisticsHandler>();
			_bulkImportStatisticsHandlerMock = new Mock<IBulkImportStatisticsHandler>();
			_serverErrorStatisticsHandlerMock = new Mock<IServerErrorStatisticsHandler>();
			_jobFinishStatisticsHandlerMock = new Mock<IJobFinishStatisticsHandler>();

			_importMetadataMock = new Mock<IImportMetadata>();

			_importMetadataMock.Setup(obj => obj.Statistics).Returns(_stats);

			_loadFile = new LoadFile { ArtifactTypeID = (int)ArtifactType.Document, StartLineNumber = _START_LINE };

			_importerSettingsMock = new Mock<IImporterSettings>();
			_importerSettingsMock.Setup(obj => obj.LoadFile).Returns(_loadFile);

			_transferConfigMock.Setup(obj => obj.ImportBatchSize).Returns(_BATCH_SIZE);

			_statisticsManager = new StatisticsManager(_loadFile, _importStatusManagerMock.Object, _importMetadataMock.Object,
				_transferConfigMock.Object, _nativeFilesStatisticsHandlerMock.Object, _metaFileStatsHandlerMock.Object, 
				_metadataStatisticsHandlerMock.Object, _bulkImportStatisticsHandlerMock.Object,
				_serverErrorStatisticsHandlerMock.Object, _jobFinishStatisticsHandlerMock.Object);
		}

		[Test]
		public void ItShouldHandleTransferRateChangedEvent()
		{
			// Arrange
			var transferBytes = 123;
			var transferTime = 456;

			// Act
			_nativeFilesStatisticsHandlerMock.Raise(obj => obj.TransferRateChanged += null, new TransferRateEventArgs(transferBytes, transferTime));
			_metaFileStatsHandlerMock.Raise(obj => obj.TransferRateChanged += null, new TransferRateEventArgs(transferBytes, transferTime));

			//Assert
			Assert.That(_stats.FileBytes, Is.EqualTo(transferBytes));
			Assert.That(_stats.MetadataBytes, Is.EqualTo(transferBytes));
			Assert.That(_stats.FileTime, Is.EqualTo(transferTime));
			Assert.That(_stats.MetadataTime, Is.EqualTo(transferTime));
		}

		[Test]
		public void ItShouldHandleFileTransferEvent()
		{
			// Arrange
			int filesTranferred = 100;

			// Act
			_nativeFilesStatisticsHandlerMock.Raise(obj => obj.FilesTransferred += null, new FilesTransferredEventArgs(filesTranferred));

			//Assert
			_importStatusManagerMock.Verify(obj => obj.RaiseCustomStatusUpdateEvent(_statisticsManager, StatusUpdateType.Progress,
				"Uploading files", filesTranferred + (int)_loadFile.StartLineNumber ), Times.Once);
		}

		[Test]
		public void ItShouldHandleFileMetadataProcessedEvent()
		{
			// Arrange
			string recId = "1234";
			int lineNumber = 2;

			// Act
			_metadataStatisticsHandlerMock.Raise(obj => obj.FileMetadataProcessed += null, new FileMetadataEventArgs(recId, lineNumber));

			//Assert
			_importStatusManagerMock.Verify(obj => obj.RaiseCustomStatusUpdateEvent(_statisticsManager, StatusUpdateType.Progress,
				It.IsAny<string>(), lineNumber), Times.Once);
		}


		[Test]
		public void ItShouldHandleUploadingMetadataFileEvent()
		{
			// Arrange
			string recId = "1234";
			int lineNumber = 2;

			// Act
			_metadataStatisticsHandlerMock.Raise(obj => obj.FileMetadataProcessed += null, new FileMetadataEventArgs(recId, lineNumber));

			//Assert
			_importStatusManagerMock.Verify(obj => obj.RaiseCustomStatusUpdateEvent(_statisticsManager, StatusUpdateType.Progress,
				It.Is<string>(S => S.Contains($"Processing file metadata: Item '{recId}' processed [line {lineNumber}].")), lineNumber), Times.Once);
		}


		[Test]
		public void ItShouldHandleBulkImportMetadataStartedEvent()
		{
			// Arrange
			string recId = "1234";
			int lineNumber = 1;

			// Act
			_metadataStatisticsHandlerMock.Raise(obj => obj.FileMetadataProcessed += null, new FileMetadataEventArgs(recId, lineNumber));
			_metadataStatisticsHandlerMock.Raise(obj => obj.BulkImportMetadataStarted += null, new EventArgs());

			//Assert
			_importStatusManagerMock.Verify(obj => obj.RaiseCustomStatusUpdateEvent(_statisticsManager, StatusUpdateType.Progress,
				"Processing metadata on server", It.IsAny<int>()), Times.Once);
		}

		[Test]
		public void ItShouldHandleBulkImportCompletedEvent()
		{
			// Arrange
			string recId = "1";
			int lineNumber = 1;

			int documentsCreated = 1;
			int documentsUpdated = 2;
			int filesProcessed = 3;
			long time = 4;

			// Act
			_metadataStatisticsHandlerMock.Raise(obj => obj.FileMetadataProcessed += null, new FileMetadataEventArgs(recId, lineNumber));
			_bulkImportStatisticsHandlerMock.Raise(obj => obj.BulkImportCompleted += null, _statisticsManager, new BulkImportCompletedEventArgs(
				new MassImportResults()
				{
					ArtifactsCreated = documentsCreated,
					ArtifactsUpdated = documentsUpdated,
					FilesProcessed = filesProcessed
				}, time));

			//Assert
			Assert.That(_stats.DocumentsCreated, Is.EqualTo(documentsCreated));
			Assert.That(_stats.DocumentsUpdated, Is.EqualTo(documentsUpdated));
			Assert.That(_stats.FilesProcessed, Is.EqualTo(filesProcessed));
			Assert.That(_stats.SqlTime, Is.EqualTo(time));
			Assert.That(_stats.DocCount, Is.EqualTo(documentsUpdated + documentsCreated));


			_importStatusManagerMock.Verify(obj => obj.RaiseCustomStatusUpdateEvent(_statisticsManager, StatusUpdateType.Progress,
				"Metadata uploaded", It.IsAny<int>()), Times.Once);
		}


		[Test]
		public void ItShouldHandleIoWarningOccurredEvent()
		{
			// Arrange
			var ex = new Exception();
			int waitTime = 1;

			_transferConfigMock.Setup(obj => obj.IoErrorWaitTimeInSeconds).Returns(waitTime);
			// Act
			_bulkImportStatisticsHandlerMock.Raise(obj => obj.IoWarningOccurred += null, _statisticsManager, ex);

			//Assert
			_importStatusManagerMock.Verify(obj => obj.RaiseIoWarningEvent(_statisticsManager, waitTime, It.IsAny<int>(), ex), Times.Once);
		}

		[Test]
		public void ItShouldHandleRetrievingServerErrorsEvent()
		{
			// Arrange
			string recId = "1234";
			int lineNumber = 1;

			// Act
			_metadataStatisticsHandlerMock.Raise(obj => obj.FileMetadataProcessed += null, new FileMetadataEventArgs(recId, lineNumber));
			_serverErrorStatisticsHandlerMock.Raise(obj => obj.RetrievingServerErrors += null, new EventArgs());

			//Assert
			_importStatusManagerMock.Verify(obj => obj.RaiseCustomStatusUpdateEvent(_statisticsManager, StatusUpdateType.Progress,
				"Retrieving errors from server", It.IsAny<int>()), Times.Once);
		}

		[Test]
		public void ItShouldHandleRetrievingServerErrorStatusUpdatedEvent()
		{
			// Arrange
			string recId = "1234";
			int lineNumber = 1;

			// Act
			_metadataStatisticsHandlerMock.Raise(obj => obj.FileMetadataProcessed += null, new FileMetadataEventArgs(recId, lineNumber));
			_serverErrorStatisticsHandlerMock.Raise(obj => obj.RetrievingServerErrorStatusUpdated += null, _statisticsManager, recId);

			//Assert
			_importStatusManagerMock.Verify(obj => obj.RaiseCustomStatusUpdateEvent(_statisticsManager, StatusUpdateType.Progress,
				recId, It.IsAny<int>()), Times.Once);
		}

		[Test]
		public void ItShouldHandleJobFinishedEvent()
		{
			// Arrange
			string recId = "1234";
			int lineNumber = 1;

			// Act
			_metadataStatisticsHandlerMock.Raise(obj => obj.FileMetadataProcessed += null, new FileMetadataEventArgs(recId, lineNumber));
			_jobFinishStatisticsHandlerMock.Raise(obj => obj.JobFinished += null, _statisticsManager, recId);

			//Assert
			_importStatusManagerMock.Verify(obj => obj.RaiseCustomStatusUpdateEvent(_statisticsManager, StatusUpdateType.End,
				recId, It.IsAny<int>()), Times.Once);
		}
	}
}
