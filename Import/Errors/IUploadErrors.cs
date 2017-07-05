using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public interface IUploadErrors
	{
		void HandleUploadErrors(IDictionary<FileMetadata, UploadResult> uploadResults);
	}
}