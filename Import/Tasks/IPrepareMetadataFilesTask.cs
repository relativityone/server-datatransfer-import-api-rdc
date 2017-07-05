using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public interface IPrepareMetadataFilesTask
	{
		void Execute(IDictionary<FileMetadata, UploadResult> uploadResults);
	}
}