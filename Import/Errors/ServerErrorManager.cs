using kCura.Utility;
using kCura.WinEDDS.Service;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ServerErrorManager
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

		public void ManageErrors(int artifactTypeId, CaseInfo caseInfo, string runId, int keyFieldId)
		{
			if (!_bulkImportManager.NativeRunHasErrors(caseInfo.ArtifactID, runId))
			{
				return;
			}

			var errorFileKey = _bulkImportManager.GenerateNonImageErrorFiles(caseInfo.ArtifactID, runId, artifactTypeId, true, keyFieldId);
			//TODO write status

			GenericCsvReader reader = _serverErrorFileDownloader.DownloadErrorFile(errorFileKey.LogKey, caseInfo);
			_serverErrorFile.HandleServerErrors(reader);
		}
	}
}