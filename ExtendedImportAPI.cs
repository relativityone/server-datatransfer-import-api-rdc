using System;
using kCura.Relativity.DataReaderClient;
using kCura.WinEDDS.Service;

namespace kCura.Relativity.ImportAPI {
	public class ExtendedImportAPI : ImportAPI {
		public ExtendedImportAPI(string UserName, string Password)
			: base(UserName, Password) {
		}

		public ExtendedImportAPI(string UserName, string Password, string WebServiceURL)
			: base(UserName, Password, WebServiceURL) {
		}

		public ExtendedImportAPI(string WebServiceURL)
			: base(WebServiceURL) {
		}

		public void CleanUpAfterJobWithSpoofing(ImportBulkArtifactJob job) {
			if (job != null && !String.IsNullOrWhiteSpace(job.Settings.OnBehalfOfUserToken)) {
				var am = new AuditManager(Credentials, CookieCache);

				try {
					am.DeleteAuditToken(job.Settings.OnBehalfOfUserToken);
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
