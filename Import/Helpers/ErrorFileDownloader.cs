using Relativity;

namespace kCura.WinEDDS.Core.Import.Helpers
{
	public class ErrorFileDownloader : IErrorFileDownloader
	{
		private readonly FileDownloader _fileDownloader;

		public ErrorFileDownloader(FileDownloader fileDownloader)
		{
			_fileDownloader = fileDownloader;
		}

		public event FileDownloader.UploadStatusEventEventHandler UploadStatusEvent
		{
			add { _fileDownloader.UploadStatusEvent += value; }
			remove { _fileDownloader.UploadStatusEvent -= value; }
		}

		public bool MoveTempFileToLocal(string localFilePath, string remoteFileGuid, CaseInfo caseInfo, bool removeRemoteTempFile)
		{
			return _fileDownloader.MoveTempFileToLocal(localFilePath, remoteFileGuid, caseInfo, removeRemoteTempFile);
		}

		public void RemoveRemoteTempFile(string remoteFileGuid, CaseInfo caseInfo)
		{
			_fileDownloader.RemoveRemoteTempFile(remoteFileGuid, caseInfo);
		}
	}
}