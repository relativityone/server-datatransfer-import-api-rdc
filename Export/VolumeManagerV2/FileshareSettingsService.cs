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
		private readonly string _webServiceUrl;
		private readonly NetworkCredential _currentUserCredential;
		private readonly CookieContainer _cookieContainer;

		public FileshareSettingsService(ILog logger, ExportFile exportSettings)
		{
		    if (logger == null)
		    {
		        throw new ArgumentNullException(nameof(logger));
		    }

		    if (exportSettings == null)
		    {
		        throw new ArgumentNullException(nameof(exportSettings));
		    }

            _logger = logger;
            _webServiceUrl = Config.WebServiceURL;
			_workspaceId = exportSettings.CaseInfo.ArtifactID;
			_currentUserCredential = exportSettings.Credential;
			_cookieContainer = exportSettings.CookieContainer;
		}

		public RelativityFileShareSettings GetSettingsForFileshare(string fileUrl)
		{
			if (_cachedSettings == null)
			{
				GetFileshareSettingsForWorkspace();
			}

			RelativityFileShareSettings settings = _cachedSettings.FirstOrDefault(n => n.IsBaseOf(fileUrl));
			return settings;
		}

		private void GetFileshareSettingsForWorkspace()
		{
			try
			{
				TapiBridgeParameters parameters = new TapiBridgeParameters
				{
					Credentials = _currentUserCredential,
					WebCookieContainer = _cookieContainer,
					WebServiceUrl = _webServiceUrl,
					WorkspaceId = _workspaceId
				};

				RelativityConnectionInfo connectionInfo = TapiWinEddsHelper.CreateRelativityConnectionInfo(parameters);
				using (ITransferLog transferLog = new RelativityTransferLog(_logger, false))
				using (var transferHost = new RelativityTransferHost(connectionInfo, transferLog))
				{
					IFileStorageSearch service = transferHost.CreateFileStorageSearch();
					FileStorageSearchContext context = new FileStorageSearchContext { WorkspaceId = _workspaceId };

					FileStorageSearchResults results = service.SearchAsync(context).ConfigureAwait(false).GetAwaiter().GetResult();
					if (results.InvalidFileShares.Count > 0)
					{
						foreach (var fileShare in results.InvalidFileShares)
						{
							this._logger.LogWarning(
								"The Relativity instance '{Url}' defines workspace '{WorkspaceId}' that references invalid fileshare '{FileShareName}' from the associated resource pool.",
								_webServiceUrl,
								_workspaceId,
								fileShare.Name);
						}
					}

					_cachedSettings = results.FileShares.Select(f => new RelativityFileShareSettings(f)).ToList();
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"{nameof(GetFileshareSettingsForWorkspace)}() failed with following error message {0} and stack trace {1}", e.Message, e.StackTrace);
				throw;
			}
		}
	}
}