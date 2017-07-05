using Relativity;

namespace kCura.WinEDDS.Core.Import.Helpers
{
	public interface IErrorFileDownloader
	{
		event FileDownloader.UploadStatusEventEventHandler UploadStatusEvent;

		bool MoveTempFileToLocal(string localFilePath, string remoteFileGuid, CaseInfo caseInfo, bool removeRemoteTempFile);

		void RemoveRemoteTempFile(string remoteFileGuid, CaseInfo caseInfo);
	}
}