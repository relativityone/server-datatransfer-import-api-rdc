using kCura.Utility;
using kCura.WinEDDS.Service;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ServerErrorManager : IServerErrorManager
	{
		private readonly BulkImportManager _bulkImportManager;
		private readonly ServerErrorFile _serverErrorFile;
		private readonly ServerErrorFileDownloader _serverErrorFileDownloader;
		private readonly ImportContext _importContext;

		public ServerErrorManager(BulkImportManager bulkImportManager, ServerErrorFile serverErrorFile, ServerErrorFileDownloader serverErrorFileDownloader, ImportContext importContext)
		{
			_bulkImportManager = bulkImportManager;
			_serverErrorFile = serverErrorFile;
			_serverErrorFileDownloader = serverErrorFileDownloader;
			_importContext = importContext;
		}

		public void ManageErrors()
		{
			if (!_bulkImportManager.NativeRunHasErrors(_importContext.Settings.CaseInfo.ArtifactID, _importContext.RunId))
			{
				return;
			}

			//TODO
			var keyFieldId = -1;
			var errorFileKey = _bulkImportManager.GenerateNonImageErrorFiles(_importContext.Settings.CaseInfo.ArtifactID, _importContext.RunId,
				_importContext.Settings.ArtifactTypeID, true, keyFieldId);
			//TODO write status

			GenericCsvReader reader = _serverErrorFileDownloader.DownloadErrorFile(errorFileKey.LogKey, _importContext.Settings.CaseInfo);
			_serverErrorFile.HandleServerErrors(reader);
		}
	}
}