using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import
{
	public interface IFileUploader
	{
		void UploadFile(string sourceFile, string fileName);

		IDictionary<string, bool> WaitForUploadToComplete();
	}
}