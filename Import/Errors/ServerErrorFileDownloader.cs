using System.Text;
using kCura.Utility;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Status;
using Polly;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ServerErrorFileDownloader : IServerErrorFileDownloader
	{
		private const int _FILE_DOWNLOAD_TRIES = 3;

		private readonly IImportStatusManager _importStatusManager;
		private readonly IPathHelper _pathHelper;
		private readonly IErrorFileDownloaderFactory _errorFileDownloaderFactory;

		public ServerErrorFileDownloader(IImportStatusManager importStatusManager, IPathHelper pathHelper, IErrorFileDownloaderFactory errorFileDownloaderFactory)
		{
			_importStatusManager = importStatusManager;
			_pathHelper = pathHelper;
			_errorFileDownloaderFactory = errorFileDownloaderFactory;
		}

		public GenericCsvReader DownloadErrorFile(string logKey, CaseInfo caseInfo)
		{
			IErrorFileDownloader fileDownloader = null;
			GenericCsvReader reader = null;
			try
			{
				var errorsLocation = _pathHelper.GetTempFileName();

				fileDownloader = _errorFileDownloaderFactory.Create(caseInfo);
				fileDownloader.UploadStatusEvent += OnUploadStatusEvent;

				Policy<bool>.HandleResult(x => !x).Retry(_FILE_DOWNLOAD_TRIES - 1).Execute(() =>
				{
					fileDownloader.MoveTempFileToLocal(errorsLocation, logKey, caseInfo, false);
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

			return reader;
		}

		private void OnUploadStatusEvent(string message)
		{
			_importStatusManager.RaiseStatusUpdateEvent(this, StatusUpdateType.Update, message, -1);
		}
	}
}