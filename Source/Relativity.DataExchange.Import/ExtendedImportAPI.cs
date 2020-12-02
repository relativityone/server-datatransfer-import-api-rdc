using System;
using kCura.Relativity.DataReaderClient;
using kCura.WinEDDS.Service;

namespace kCura.Relativity.ImportAPI {
	using global::Relativity.DataExchange;

	public class ExtendedImportAPI : ImportAPI, IExtendedImportAPI
	{
		public ExtendedImportAPI(string userName, string password)
			: base(userName, password) {
		}

		public ExtendedImportAPI(string userName, string password, string webServiceUrl)
			: base(userName, password, webServiceUrl) {
		}

		/// <summary>
		/// This factory method returns new <see cref="ImportAPI"/> object initialized with <see cref="IRelativityTokenProvider"/>
		/// </summary>
		/// <param name="webServiceUrl">RelativityWebApi url</param>
		/// <param name="relativityTokenProvider">Relativity token provider object</param>
		/// <returns></returns>
		public new static ImportAPI CreateByTokenProvider(
			string webServiceUrl, IRelativityTokenProvider relativityTokenProvider)
		{
			return ImportAPI.CreateByTokenProvider(webServiceUrl, relativityTokenProvider);
		}

		public void CleanUpAfterJobWithSpoofing(string onBehalfOfUserToken) {
			if (!String.IsNullOrWhiteSpace(onBehalfOfUserToken))
			{
				using (var am = new AuditManager(_credentials, _cookieMonster))
				{
					try
					{
						am.DeleteAuditToken(onBehalfOfUserToken);
					}
					catch { }
				}
			}
		}

		public ImportBulkArtifactJob NewNativeDocumentImportJob(string token) {
			var returnJob = NewNativeDocumentImportJob();

			returnJob.Settings.OnBehalfOfUserToken = token;

			return returnJob;
		}

		public ImportBulkArtifactJob NewArtifactImportJob(string token, int artifactTypeID)
		{
			var returnJob = NewObjectImportJob(artifactTypeID);

			returnJob.Settings.OnBehalfOfUserToken = token;

			return returnJob;
		}

	}
}
