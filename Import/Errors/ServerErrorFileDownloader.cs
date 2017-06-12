using System.Text;
using kCura.Utility;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Statistics;
using Polly;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ServerErrorFileDownloader : IServerErrorFileDownloader
	{
		private const int _FILE_DOWNLOAD_TRIES = 3;

		private readonly IServerErrorStatisticsHandler _serverErrorStatisticsHandler;
		private readonly ILog _log;
		private readonly IPathHelper _pathHelper;
		private readonly IErrorFileDownloaderFactory _errorFileDownloaderFactory;

		public ServerErrorFileDownloader(IPathHelper pathHelper, IErrorFileDownloaderFactory errorFileDownloaderFactory, 
			IServerErrorStatisticsHandler serverErrorStatisticsHandler, ILog log)
		{
			_pathHelper = pathHelper;
			_errorFileDownloaderFactory = errorFileDownloaderFactory;
			_serverErrorStatisticsHandler = serverErrorStatisticsHandler;
			_log = log;
		}

		public GenericCsvReader DownloadErrorFile(string logKey, CaseInfo caseInfo)
		{
			IErrorFileDownloader fileDownloader = null;
			GenericCsvReader reader = null;
			try
			{
				var errorsLocation = _pathHelper.GetTempFileName();

				_log.LogDebug($"Generated error file location: {errorsLocation}");

				fileDownloader = _errorFileDownloaderFactory.Create(caseInfo);
				fileDownloader.UploadStatusEvent += OnUploadStatusEvent;

				Policy<bool>.HandleResult(x => !x).Retry(_FILE_DOWNLOAD_TRIES - 1).Execute(() =>
				{
					_log.LogDebug("Downloading error from the server to temp file");
					fileDownloader.MoveTempFileToLocal(errorsLocation, logKey, caseInfo, false);

					_log.LogDebug("Creating error file reader");
					reader = new GenericCsvReader(errorsLocation, Encoding.UTF8, true);

					var firstChar = reader.Peek();
					if (firstChar == -1)
					{
						reader.Close();
						reader = null;
						return false;
					}
					return true;
				});
				_log.LogDebug("Removing error temp file on the server");
				fileDownloader.RemoveRemoteTempFile(logKey, caseInfo);
			}
			catch
			{
				reader?.Close();
				throw;
			}
			finally
			{
				if (fileDownloader != null)
				{
					fileDownloader.UploadStatusEvent -= OnUploadStatusEvent;
				}
			}
			_log.LogDebug("Error file reader has been created succesfully");
			return reader;
		}

		private void OnUploadStatusEvent(string message)
		{
			_serverErrorStatisticsHandler.RaiseRetrievingServerErrorStatusUpdatedEvent(message);
		}
	}
}