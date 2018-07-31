using System.Collections.Generic;
using System.Net;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.Data;
using kCura.Relativity.ImportAPI.Enumeration;
using kCura.WinEDDS;

namespace kCura.Relativity.ImportAPI
{
	public interface IExtendedImportAPI : IImportAPI
	{
		void CleanUpAfterJobWithSpoofing(string onBehalfOfUserToken);
		ImportBulkArtifactJob NewNativeDocumentImportJob(string token);
		ImportBulkArtifactJob NewNativeDocumentImportJob(string token, ITimeKeeperManager timeKeeper);
		ImportBulkArtifactJob NewArtifactImportJob(string token, int artifactTypeID);
	}
}