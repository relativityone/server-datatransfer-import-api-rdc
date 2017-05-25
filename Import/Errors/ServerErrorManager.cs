using kCura.Utility;
using kCura.WinEDDS.Core.Import.Managers;
using kCura.WinEDDS.Core.Import.Status;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ServerErrorManager : IServerErrorManager
	{
		private readonly IBulkImportManager _bulkImportManager;
		private readonly IImportStatusManager _importStatusManager;
		private readonly IServerErrorFile _serverErrorFile;
		private readonly IServerErrorFileDownloader _serverErrorFileDownloader;

		public ServerErrorManager(IBulkImportManager bulkImportManager, IImportStatusManager importStatusManager, IServerErrorFile serverErrorFile,
			IServerErrorFileDownloader serverErrorFileDownloader)
		{
			_bulkImportManager = bulkImportManager;
			_serverErrorFile = serverErrorFile;
			_serverErrorFileDownloader = serverErrorFileDownloader;
			_importStatusManager = importStatusManager;
		}

		public void ManageErrors(ImportContext importContext)
		{
			if (!_bulkImportManager.NativeRunHasErrors(importContext.Settings.LoadFile.CaseInfo.ArtifactID, importContext.Settings.RunId))
			{
				return;
			}

			var errorFileKey = _bulkImportManager.GenerateNonImageErrorFiles(importContext.Settings.LoadFile.CaseInfo.ArtifactID, importContext.Settings.RunId,
				importContext.Settings.LoadFile.ArtifactTypeID, true, importContext.Settings.KeyFieldId);

			_importStatusManager.RaiseStatusUpdateEvent(this, StatusUpdateType.Update, "Retrieving errors from server", -1);

			GenericCsvReader reader = _serverErrorFileDownloader.DownloadErrorFile(errorFileKey.LogKey, importContext.Settings.LoadFile.CaseInfo);
			_serverErrorFile.HandleServerErrors(reader);
		}
	}
}