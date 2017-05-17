using System;
using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportBatchContext
	{
		public List<FileMetadata> FileMetaDataHolder { get; private set; }

		public Guid RunId { get; private set; }

		public ImportBatchContext(int batchSize)
		{
			FileMetaDataHolder = new List<FileMetadata>(batchSize);
		}
	}
}
