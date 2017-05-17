using System.IO;
using System.Net;
using System.Text;
using kCura.Utility;
using kCura.WinEDDS.Service;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ServerErrorFileDownloader
	{
		private readonly BulkImportManager _bulkImportManager;

		public ServerErrorFileDownloader(BulkImportManager bulkImportManager)
		{
			_bulkImportManager = bulkImportManager;
		}

		public GenericCsvReader DownloadErrorFile(string logKey, CaseInfo caseInfo)
		{
			FileDownloader fileDownloader = null;
			GenericCsvReader reader = null;
			try
			{
				var errorsLocation = Path.GetTempFileName();

				fileDownloader = new FileDownloader((NetworkCredential) _bulkImportManager.Credentials, caseInfo.DocumentPath, caseInfo.DownloadHandlerURL, _bulkImportManager.CookieContainer,
					Settings.AuthenticationToken);
				fileDownloader.UploadStatusEvent += OnUploadStatusEvent;

				int triesLeft = 3;

				while (triesLeft > 0)
				{
					fileDownloader.MoveTempFileToLocal(errorsLocation, logKey, caseInfo, false);
					reader = new GenericCsvReader(errorsLocation, Encoding.UTF8, true);

					var firstChar = reader.Peek();
					if (firstChar == -1)
					{
						triesLeft--;
						reader.Close();
						reader = null;
					}
					else
					{
						break;
					}
				}
				fileDownloader.RemoveRemoteTempFile(logKey, caseInfo);
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
			//TODO
		}
	}
}