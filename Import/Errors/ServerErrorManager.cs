using kCura.Utility;
using kCura.WinEDDS.Core.Import.Managers;
using kCura.WinEDDS.Core.Import.Statistics;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ServerErrorManager : IServerErrorManager
	{
		private readonly IBulkImportManager _bulkImportManager;
		private readonly IServerErrorFile _serverErrorFile;
		private readonly IServerErrorFileDownloader _serverErrorFileDownloader;
		private readonly IServerErrorStatisticsHandler _serverErrorStatisticsHandler;
		private readonly ILog _log;

		public ServerErrorManager(IBulkImportManager bulkImportManager, IServerErrorFile serverErrorFile,
			IServerErrorFileDownloader serverErrorFileDownloader, IServerErrorStatisticsHandler serverErrorStatisticsHandler, ILog log)
		{
			_bulkImportManager = bulkImportManager;
			_serverErrorFile = serverErrorFile;
			_serverErrorFileDownloader = serverErrorFileDownloader;
			_serverErrorStatisticsHandler = serverErrorStatisticsHandler;
			_log = log;
		}

		public void ManageErrors(ImportContext importContext)
		{
			if (!_bulkImportManager.NativeRunHasErrors(importContext.Settings.LoadFile.CaseInfo.ArtifactID, importContext.Settings.RunId))
			{
				return;
			}

			_log.LogWarning("Metadata Bulk Import process errors found in the current batch.");

			var errorFileKey = _bulkImportManager.GenerateNonImageErrorFiles(importContext.Settings.LoadFile.CaseInfo.ArtifactID, importContext.Settings.RunId,
				importContext.Settings.LoadFile.ArtifactTypeID, true, importContext.Settings.KeyFieldId);

			_serverErrorStatisticsHandler.RaiseRetrievingServerErrorsEvent();

			_log.LogInformation($"Downaloding error file from the server. Log Key: {errorFileKey.LogKey}");
			GenericCsvReader reader = _serverErrorFileDownloader.DownloadErrorFile(errorFileKey.LogKey, importContext.Settings.LoadFile.CaseInfo);
			_serverErrorFile.HandleServerErrors(reader);
			_log.LogInformation("Handling error file from the server completed");
		}
	}
}