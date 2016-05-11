using System;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.Enumeration;
using kCura.WinEDDS.Service;

namespace kCura.Relativity.ImportAPI {
	public class ExtendedImportAPI : ImportAPI, IExtendedImportAPI
	{
		public ExtendedImportAPI(string userName, string password, ExecutionSource executionSource = ExecutionSource.ImportAPI)
			: base(userName, password, executionSource) {
		}

		public ExtendedImportAPI(string userName, string password, string webServiceUrl, ExecutionSource executionSource = ExecutionSource.ImportAPI)
			: base(userName, password, webServiceUrl, executionSource) {
		}

		public ExtendedImportAPI(string webServiceUrl, ExecutionSource executionSource = ExecutionSource.ImportAPI)
			: base(webServiceUrl, executionSource) {
		}

		public void CleanUpAfterJobWithSpoofing(string onBehalfOfUserToken) {
			if (!String.IsNullOrWhiteSpace(onBehalfOfUserToken)) {
				var am = new AuditManager(_credentials, _cookieMonster);

				try {
					am.DeleteAuditToken(onBehalfOfUserToken);
				} catch {
				}
			}
		}

		public ImportBulkArtifactJob NewNativeDocumentImportJob(string token) {
			var returnJob = NewNativeDocumentImportJob();

			returnJob.Settings.OnBehalfOfUserToken = token;

			return returnJob;
		}
	}
}
