using kCura.EDDS.WebAPI.BulkImportManagerBase;
using kCura.Utility;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Managers;
using kCura.WinEDDS.Core.Import.Status;
using Moq;
using NUnit.Framework;
using Relativity;

namespace kCura.WinEDDS.Core.NUnit.Import.Errors
{
	[TestFixture]
	public class ServerErrorManagerTests
	{
		private const int _APP_ID = 191829;
		private const string _RUN_ID = "151960";

		private ServerErrorManager _instance;
		private ImportContext _importContext;

		private Mock<IBulkImportManager> _bulkImportManager;
		private Mock<IImportStatusManager> _importStatusManager;
		private Mock<IServerErrorFile> _serverErrorFile;
		private Mock<IServerErrorFileDownloader> _serverErrorFileDownloader;

		[SetUp]
		public void SetUp()
		{
			_bulkImportManager = new Mock<IBulkImportManager>();
			_importStatusManager = new Mock<IImportStatusManager>();
			_serverErrorFile = new Mock<IServerErrorFile>();
			_serverErrorFileDownloader = new Mock<IServerErrorFileDownloader>();

			var importSettings = new Mock<IImporterSettings>();
			importSettings.Setup(x => x.RunId).Returns(_RUN_ID);
			importSettings.Setup(x => x.LoadFile).Returns(new LoadFile
			{
				CaseInfo = new CaseInfo
				{
					ArtifactID = _APP_ID
				}
			});
			_importContext = new ImportContext
			{
				Settings = importSettings.Object
			};

			_instance = new ServerErrorManager(_bulkImportManager.Object, _importStatusManager.Object, _serverErrorFile.Object, _serverErrorFileDownloader.Object);
		}

		[Test]
		public void ItShouldSkipForSuccessfulImport()
		{
			_bulkImportManager.Setup(x => x.NativeRunHasErrors(_APP_ID, _RUN_ID)).Returns(false);

			// ACT
			_instance.ManageErrors(_importContext);

			// ASSERT
			_bulkImportManager.Verify(x => x.GenerateNonImageErrorFiles(_APP_ID, _RUN_ID, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int>()), Times.Never);
			_serverErrorFileDownloader.Verify(x => x.DownloadErrorFile(It.IsAny<string>(), It.IsAny<CaseInfo>()), Times.Never);
			_serverErrorFile.Verify(x => x.HandleServerErrors(It.IsAny<GenericCsvReader>()), Times.Never);
		}

		[Test]
		public void ItShouldHandleServerErrors()
		{
			var errorFileKey = new ErrorFileKey();

			_bulkImportManager.Setup(x => x.NativeRunHasErrors(_APP_ID, _RUN_ID)).Returns(true);
			_bulkImportManager.Setup(x => x.GenerateNonImageErrorFiles(_APP_ID, _RUN_ID, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int>())).Returns(errorFileKey);

			_serverErrorFileDownloader.Setup(x => x.DownloadErrorFile(errorFileKey.LogKey, It.IsAny<CaseInfo>())).Returns((GenericCsvReader) null);

			// ACT
			_instance.ManageErrors(_importContext);

			// ASSERT
			_bulkImportManager.Verify(x => x.GenerateNonImageErrorFiles(_APP_ID, _RUN_ID, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<int>()), Times.Once);

			_serverErrorFileDownloader.Verify(x => x.DownloadErrorFile(errorFileKey.LogKey, It.IsAny<CaseInfo>()), Times.Once);

			_serverErrorFile.Verify(x => x.HandleServerErrors(null), Times.Once);
		}
	}
}