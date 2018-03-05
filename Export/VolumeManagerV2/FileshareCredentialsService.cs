using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class FileshareCredentialsService : IFileshareCredentialsService
	{
		private readonly ILog _logger;
		private readonly ExportFile _exportSettings;

		public FileshareCredentialsService(ILog logger, ExportFile exportSettings)
		{
			_logger = logger;
			_exportSettings = exportSettings;
			CachedCredentials = new Dictionary<string, AsperaCredential>();
		}

		public Dictionary<string, AsperaCredential> CachedCredentials { get; set; }

		public AsperaCredential GetCredentialsForFileshare(Uri fileUri)
		{
			if (CachedCredentials.Count == 0)
			{
				GetAndStoreCredentialsForCurrentWorskpace();
			}

			return CachedCredentials.FirstOrDefault(n => n.Key == fileUri.ToString()).Value;
		}

		private void GetAndStoreCredentialsForCurrentWorskpace()
		{
			try
			{
				Uri host = GetHostBasedOneWebServiceUrl();
				var connectionInfo = new RelativityConnectionInfo(
					host, 
					new BearerTokenCredential(_exportSettings.Credential.Password), 
					_exportSettings.CaseArtifactID);
				using (ITransferLog transferLog = new RelativityTransferLog(_logger, false))
				{
					using (var transferHost = new RelativityTransferHost(connectionInfo, transferLog))
					{
						// The storage search API is obtained through the transfer host.
						IFileStorageSearch service = transferHost.CreateFileStorageSearch();

						// Note: until the Kepler API changes have been made, you must be an admin and supply the resource pool.

						//TODO:Remove below comment once Certificate issue is resolved - to be confirmed and tested when Scott says so. Worskpace method should work as good as the ResPool One.
						FileStorageSearchResults results = service.GetWorkspaceFileSharesAsync(_exportSettings.CaseArtifactID).ConfigureAwait(false).GetAwaiter().GetResult();

						foreach (RelativityFileShare fileShare in results.FileShares)
						{
							CachedCredentials[fileShare.Url] = fileShare.TransferCredential;
						}
					}
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"{nameof(GetAndStoreCredentialsForCurrentWorskpace)}() failed with following error message {0} and stack trace {1}", e.Message, e.StackTrace);
				throw;
			}
		}

		private Uri GetHostBasedOneWebServiceUrl()
		{
			string webServiceUrl = Config.WebServiceURL;
			var baseUri = new Uri(webServiceUrl);
			return new Uri(baseUri.GetLeftPart(UriPartial.Authority));
		}
	}
}