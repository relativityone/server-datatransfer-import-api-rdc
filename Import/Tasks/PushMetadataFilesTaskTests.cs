using System.Collections.Generic;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Factories;
using kCura.WinEDDS.Core.Import.Statistics;
using kCura.WinEDDS.Core.Import.Status;
using kCura.WinEDDS.Core.Import.Tasks;
using kCura.WinEDDS.Core.Import.Tasks.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Import.Tasks
{
	[TestFixture]
	public class PushMetadataFilesTaskTests
	{
		private PushMetadataFilesTask _instance;

		private ImportBatchContext _importBatchContext;
		private ImportContext _importContext;
		private MetadataFilesInfo _metadataFilesInfo;

		private Mock<IFileUploader> _fileUploader;
		private Mock<IMetadataFilesServerExecution> _metadataFilesServerExecution;
		private Mock<IServerErrorManager> _serverErrorManager;

		[SetUp]
		public void SetUp()
		{
			_fileUploader = new Mock<IFileUploader>();

			_metadataFilesServerExecution = new Mock<IMetadataFilesServerExecution>();
			_serverErrorManager = new Mock<IServerErrorManager>();

			var cancellationProvider = new Mock<ICancellationProvider>();

			_importContext = new ImportContext();
			_metadataFilesInfo = new MetadataFilesInfo();
			_importBatchContext = new ImportBatchContext(_importContext, 1000)
			{
				MetadataFilesInfo = new List<MetadataFilesInfo> {_metadataFilesInfo}
			};

			var fileUploaderFactory = new Mock<IFileUploaderFactory>();
			fileUploaderFactory.Setup(x => x.CreateBcpFileUploader()).Returns(_fileUploader.Object);

			var importMetadata = new Mock<IImportMetadata>();
			importMetadata.Setup(x => x.BatchSizeHistoryList).Returns(new List<int>());

			var importExceptionHandlerExec = new ImportExceptionHandlerExec(new Mock<IImportStatusManager>().Object, importMetadata.Object, new Mock<IErrorContainer>().Object,
				new Mock<ILog>().Object);

			var metadataStatisticsHandler = new Mock<IMetadataStatisticsHandler>();

			_instance = new PushMetadataFilesTask(_metadataFilesServerExecution.Object, fileUploaderFactory.Object, _serverErrorManager.Object, importMetadata.Object,
				cancellationProvider.Object, importExceptionHandlerExec, metadataStatisticsHandler.Object);
		}

		[Test]
		public void ItShouldUploadMetadataFiles_GoldWorkflow()
		{
			_fileUploader.Setup(x => x.WaitForUploadToComplete()).Returns(new Dictionary<FileMetadata, UploadResult>());

			// ACT
			_instance.PushMetadataFiles(_importBatchContext);

			// ASSERT
			_fileUploader.Verify(x => x.UploadFile(It.IsAny<FileMetadata>()), Times.Exactly(4));
			_metadataFilesServerExecution.Verify(x => x.Import(_metadataFilesInfo), Times.Once);
			_serverErrorManager.Verify(x => x.ManageErrors(_importContext), Times.Once);
		}

		[Test]
		public void ItShouldThrowExceptionWhenFileUploadFailed()
		{
			IDictionary<FileMetadata, UploadResult> uploadResults = new Dictionary<FileMetadata, UploadResult>
			{
				{new FileMetadata {FileGuid = "1"}, new UploadResult {Success = true}},
				{new FileMetadata {FileGuid = "2"}, new UploadResult {Success = false}}
			};
			_fileUploader.Setup(x => x.WaitForUploadToComplete()).Returns(uploadResults);

			// ACT
			Assert.That(() => _instance.PushMetadataFiles(_importBatchContext), Throws.Exception);

			// ASSERT
			_metadataFilesServerExecution.Verify(x => x.Import(It.IsAny<MetadataFilesInfo>()), Times.Never);
			_serverErrorManager.Verify(x => x.ManageErrors(_importContext), Times.Never);
		}
	}
}