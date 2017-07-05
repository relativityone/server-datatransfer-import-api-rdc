using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportBatchContext
	{
		public List<FileMetadata> FileMetaDataHolder { get; }

		public List<MetadataFilesInfo> MetadataFilesInfo { get; set; }

		public ImportContext ImportContext { get; }

		public ImportBatchContext(ImportContext importContext, int batchSize)
		{
			FileMetaDataHolder = new List<FileMetadata>(batchSize);
			MetadataFilesInfo = new List<MetadataFilesInfo>();
			ImportContext = importContext;
		}
	}
}