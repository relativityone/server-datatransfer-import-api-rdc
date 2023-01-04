using System;
using kCura.Relativity.DataReaderClient;
using kCura.WinEDDS.Service;

namespace kCura.Relativity.ImportAPI {
	using global::Relativity.DataExchange;
	using global::Relativity.Logging;

	public class ExtendedImportAPI : ImportAPI, IExtendedImportAPI
	{
		public ExtendedImportAPI(string userName, string password)
			: base(userName, password) {
		}

		public ExtendedImportAPI(string userName, string password, ILog logger)
			: base(userName, password, logger)
		{
		}

		public ExtendedImportAPI(string userName, string password, string webServiceUrl)
			: base(userName, password, webServiceUrl) {
		}

		public ExtendedImportAPI(string userName, string password, string webServiceUrl, ILog logger)
			: base(userName, password, webServiceUrl, logger)
		{
		}

		/// <summary>
		/// This factory method returns new <see cref="ImportAPI"/> object initialized with <see cref="IRelativityTokenProvider"/>
		/// </summary>
		/// <param name="webServiceUrl">RelativityWebApi url</param>
		/// <param name="relativityTokenProvider">Relativity token provider object</param>
		/// <param name="logger">Custom logger.</param>
		/// <returns></returns>
		public new static ImportAPI CreateByTokenProvider(
			string webServiceUrl, IRelativityTokenProvider relativityTokenProvider, ILog logger = null)
		{
			return ImportAPI.CreateByTokenProvider(webServiceUrl, relativityTokenProvider, logger);
		}

		public void CleanUpAfterJobWithSpoofing(string onBehalfOfUserToken) {
			if (!String.IsNullOrWhiteSpace(onBehalfOfUserToken))
			{
				using (var am = ManagerFactory.CreateAuditManager(_credentials, _cookieMonster, GetCorrelationId))
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
			this._runningContext.CallingAssembly = System.Reflection.Assembly.GetCallingAssembly().GetName().Name;

			var returnJob = NewNativeDocumentImportJob();

			returnJob.Settings.OnBehalfOfUserToken = token;

			return returnJob;
		}

		public ImportBulkArtifactJob NewArtifactImportJob(string token, int artifactTypeID)
		{
			this._runningContext.CallingAssembly = System.Reflection.Assembly.GetCallingAssembly().GetName().Name;

			var returnJob = NewObjectImportJob(artifactTypeID);

			returnJob.Settings.OnBehalfOfUserToken = token;

			return returnJob;
		}

	}
}
