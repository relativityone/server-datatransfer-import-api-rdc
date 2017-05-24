using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public interface IImportNativesTask
	{
		IDictionary<FileMetadata, UploadResult> Execute(ImportBatchContext importBatchContext);
	}
}