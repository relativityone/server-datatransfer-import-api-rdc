using kCura.Utility;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public interface IServerErrorFileDownloader
	{
		GenericCsvReader DownloadErrorFile(string logKey, CaseInfo caseInfo);
	}
}