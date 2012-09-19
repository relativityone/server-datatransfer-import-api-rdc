using System;
using kCura.Relativity.DataReaderClient;
using kCura.WinEDDS.Service;

namespace kCura.Relativity.ImportAPI {
	public class ExtendedImportAPI : ImportAPI, IExtendedImportAPI
	{
		public ExtendedImportAPI(string UserName, string Password)
			: base(UserName, Password) {
		}

		public ExtendedImportAPI(string UserName, string Password, string WebServiceURL)
			: base(UserName, Password, WebServiceURL) {
		}

		public ExtendedImportAPI(string WebServiceURL)
			: base(WebServiceURL) {
		}

		public void CleanUpAfterJobWithSpoofing(string onBehalfOfUserToken) {
			if (!String.IsNullOrWhiteSpace(onBehalfOfUserToken)) {
				var am = new AuditManager(Credentials, CookieCache);

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
