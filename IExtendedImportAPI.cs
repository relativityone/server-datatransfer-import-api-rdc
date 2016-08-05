using System.Collections.Generic;
using System.Net;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.Data;
using kCura.Relativity.ImportAPI.Enumeration;

namespace kCura.Relativity.ImportAPI
{
	public interface IExtendedImportAPI : IImportAPI
	{
		void CleanUpAfterJobWithSpoofing(string onBehalfOfUserToken);
		ImportBulkArtifactJob NewNativeDocumentImportJob(string token);
	}
}