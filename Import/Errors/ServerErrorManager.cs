using kCura.Utility;
using kCura.WinEDDS.Service;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ServerErrorManager : IServerErrorManager
	{
		private readonly BulkImportManager _bulkImportManager;
		private readonly ServerErrorFile _serverErrorFile;
		private readonly ServerErrorFileDownloader _serverErrorFileDownloader;

		public ServerErrorManager(BulkImportManager bulkImportManager, ServerErrorFile serverErrorFile, ServerErrorFileDownloader serverErrorFileDownloader)
		{
			_bulkImportManager = bulkImportManager;
			_serverErrorFile = serverErrorFile;
			_serverErrorFileDownloader = serverErrorFileDownloader;
		}

		public void ManageErrors(ImportContext importContext)
		{
			if (!_bulkImportManager.NativeRunHasErrors(importContext.Settings.LoadFile.CaseInfo.ArtifactID, importContext.Settings.RunId))
			{
				return;
			}

			var errorFileKey = _bulkImportManager.GenerateNonImageErrorFiles(importContext.Settings.LoadFile.CaseInfo.ArtifactID, importContext.Settings.RunId,
				importContext.Settings.LoadFile.ArtifactTypeID, true, importContext.Settings.KeyFieldId);
			//TODO write status

			GenericCsvReader reader = _serverErrorFileDownloader.DownloadErrorFile(errorFileKey.LogKey, importContext.Settings.LoadFile.CaseInfo);
			_serverErrorFile.HandleServerErrors(reader);
		}
	}
}