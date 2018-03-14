using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using kCura.WinEDDS.TApi;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class FileshareSettingsService : IFileshareSettingsService
	{
		private List<RelativityFileShareSettings> _cachedSettings;
		private readonly ILog _logger;
		private readonly int _workspaceId;
		private readonly NetworkCredential _currentUserCredential;

		public FileshareSettingsService(ILog logger, ExportFile exportSettings)
		{
			_logger = logger;
			_workspaceId = exportSettings.CaseInfo.ArtifactID;
			_currentUserCredential = exportSettings.Credential;
		}

		public RelativityFileShareSettings GetSettingsForFileshare(string fileUrl)
		{
			if (_cachedSettings == null)
			{
				_cachedSettings = new List<RelativityFileShareSettings>();
				GetFileshareSettingsForWorskpace(Config.WebServiceURL, _workspaceId, _currentUserCredential.UserName, _currentUserCredential.Password);
			}

			return _cachedSettings.FirstOrDefault(n => n.FileshareUri.IsBaseOf(new Uri(fileUrl)));
		}

		private void GetFileshareSettingsForWorskpace(string hostUrl, int workspaceId, string userName, string password )
		{
			try
			{
				RelativityConnectionInfo connectionInfo = TapiWinEddsHelper.CreateRelativityConnectionInfo(hostUrl, workspaceId, userName, password);

				using (ITransferLog transferLog = new RelativityTransferLog(_logger, false))
				using (var transferHost = new RelativityTransferHost(connectionInfo, transferLog))
				{
					IFileStorageSearch service = transferHost.CreateFileStorageSearch();
						
					FileStorageSearchResults results = service.GetWorkspaceFileSharesAsync(_workspaceId).ConfigureAwait(false).GetAwaiter().GetResult();

					_cachedSettings = results.FileShares.Select(f => new RelativityFileShareSettings(f)).ToList();
				}
				
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"{nameof(GetFileshareSettingsForWorskpace)}() failed with following error message {0} and stack trace {1}", e.Message, e.StackTrace);
				throw;
			}
		}
	}
}