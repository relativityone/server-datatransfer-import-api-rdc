using System;
using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportBatchContext
	{
		public List<FileMetadata> FileMetaDataHolder { get; private set; }

		public MetadataFilesInfo MetadataFilesInfo { get; set; }

		public ImportContext ImportContext { get; private set; }

		public ImportBatchContext(ImportContext importContext, int batchSize)
		{
			FileMetaDataHolder = new List<FileMetadata>(batchSize);
			ImportContext = importContext;
		}
	}
}
