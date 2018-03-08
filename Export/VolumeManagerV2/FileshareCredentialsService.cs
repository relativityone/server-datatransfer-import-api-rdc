using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using kCura.WinEDDS.TApi;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class FileshareCredentialsService : IFileshareCredentialsService
	{
		private Dictionary<string, AsperaCredential> _cachedCredentials;
		private readonly ILog _logger;
		private readonly int _workspaceId;
		private readonly NetworkCredential _currentUserCredential;

		public FileshareCredentialsService(ILog logger, ExportFile exportSettings)
		{
			_logger = logger;
			_workspaceId = exportSettings.CaseInfo.ArtifactID;
			_currentUserCredential = exportSettings.Credential;
		}

		public AsperaCredential GetCredentialsForFileshare(string fileUrl)
		{
			if (_cachedCredentials == null)
			{
				_cachedCredentials = new Dictionary<string, AsperaCredential>();
				GetCredentialsForWorskpace(Config.WebServiceURL, _workspaceId, _currentUserCredential.UserName, _currentUserCredential.Password);
			}

			return _cachedCredentials.FirstOrDefault(n => n.Key == fileUrl).Value;
		}

		private void GetCredentialsForWorskpace(string hostUrl, int workspaceId, string userName, string password )
		{
			try
			{
				RelativityConnectionInfo connectionInfo = TapiWinEddsHelper.CreateRelativityConnectionInfo(hostUrl, workspaceId, userName, password);

				using (ITransferLog transferLog = new RelativityTransferLog(_logger, false))
				using (var transferHost = new RelativityTransferHost(connectionInfo, transferLog))
				{
					IFileStorageSearch service = transferHost.CreateFileStorageSearch();
						
					FileStorageSearchResults results = service.GetWorkspaceFileSharesAsync(_workspaceId).ConfigureAwait(false).GetAwaiter().GetResult();

					foreach (RelativityFileShare fileShare in results.FileShares)
					{
						_cachedCredentials[fileShare.Url] = fileShare.TransferCredential;
					}
				}
				
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"{nameof(GetCredentialsForWorskpace)}() failed with following error message {0} and stack trace {1}", e.Message, e.StackTrace);
				throw;
			}
		}
	}
}