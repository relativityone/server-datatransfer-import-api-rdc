using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import
{
	public interface IFileUploader
	{
		void UploadFile(FileMetadata fileMetadata);

		IDictionary<FileMetadata, UploadResult> WaitForUploadToComplete();
	}
}