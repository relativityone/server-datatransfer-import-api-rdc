using Relativity;

namespace kCura.WinEDDS.Core.Import.Helpers
{
	public interface IErrorFileDownloaderFactory
	{
		IErrorFileDownloader Create(CaseInfo caseInfo);
	}
}