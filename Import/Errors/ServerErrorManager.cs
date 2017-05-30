using kCura.Utility;
using kCura.WinEDDS.Core.Import.Managers;
using kCura.WinEDDS.Core.Import.Statistics;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ServerErrorManager : IServerErrorManager
	{
		private readonly IBulkImportManager _bulkImportManager;
		private readonly IServerErrorFile _serverErrorFile;
		private readonly IServerErrorFileDownloader _serverErrorFileDownloader;
		private readonly IServerErrorStatisticsHandler _serverErrorStatisticsHandler;

		public ServerErrorManager(IBulkImportManager bulkImportManager, IServerErrorFile serverErrorFile,
			IServerErrorFileDownloader serverErrorFileDownloader, IServerErrorStatisticsHandler serverErrorStatisticsHandler)
		{
			_bulkImportManager = bulkImportManager;
			_serverErrorFile = serverErrorFile;
			_serverErrorFileDownloader = serverErrorFileDownloader;
			_serverErrorStatisticsHandler = serverErrorStatisticsHandler;
		}

		public void ManageErrors(ImportContext importContext)
		{
			if (!_bulkImportManager.NativeRunHasErrors(importContext.Settings.LoadFile.CaseInfo.ArtifactID, importContext.Settings.RunId))
			{
				return;
			}

			var errorFileKey = _bulkImportManager.GenerateNonImageErrorFiles(importContext.Settings.LoadFile.CaseInfo.ArtifactID, importContext.Settings.RunId,
				importContext.Settings.LoadFile.ArtifactTypeID, true, importContext.Settings.KeyFieldId);

			_serverErrorStatisticsHandler.RaiseRetrievingServerErrorsEvent();

			GenericCsvReader reader = _serverErrorFileDownloader.DownloadErrorFile(errorFileKey.LogKey, importContext.Settings.LoadFile.CaseInfo);
			_serverErrorFile.HandleServerErrors(reader);
		}
	}
}