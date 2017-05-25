using System.Net;
using kCura.WinEDDS.Core.Import.Managers;
using kCura.WinEDDS.Service;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Helpers
{
	public class ErrorFileDownloaderFactory : IErrorFileDownloaderFactory
	{
		private readonly IBulkImportManager _bulkImportManager;

		public ErrorFileDownloaderFactory(IBulkImportManager bulkImportManager)
		{
			_bulkImportManager = bulkImportManager;
		}

		public IErrorFileDownloader Create(CaseInfo caseInfo)
		{
			FileDownloader fileDownloader = new FileDownloader((NetworkCredential)_bulkImportManager.Credentials, caseInfo.DocumentPath, caseInfo.DownloadHandlerURL,
				_bulkImportManager.CookieContainer, Settings.AuthenticationToken);
			return new ErrorFileDownloader(fileDownloader);
		}
	}
}