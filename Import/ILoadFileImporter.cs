using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import
{
	public interface IImportMetadata
	{
		IArtifactReader ArtifactReader { get; }

		NameValueCollection ProcessedDocIdentifiers { get; set; }

		MetadataFilesInfo InitMetadataProcess();

		void SubmitMetadataProcess();

		string PrepareFieldsAndExtractIdentityValue(FileMetadata fileMetadata);

		void ProcessDocumentMetadata(MetaDocument metaDocument);

		void InitializeJob(ImportContext importContext);

		void CleanUp();

		WinEDDS.Statistics Statistics { get; }

		List<Int32> BatchSizeHistoryList { get; }
	}
}
