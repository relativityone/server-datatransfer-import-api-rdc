using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Relativity.Import.Export.Transfer;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	using global::Relativity.Import.Export;

	public partial class FileShareSettingsService : IFileShareSettingsService
	{
		private List<RelativityFileShareSettings> _cachedSettings;
		private readonly ILog _logger;
		private readonly int _workspaceId;
		private readonly string _webServiceUrl;
		private readonly NetworkCredential _currentUserCredential;
		private readonly CookieContainer _cookieContainer;

		public FileShareSettingsService(ILog logger, ExportFile exportSettings)
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
            _webServiceUrl = AppSettings.Instance.WebApiServiceUrl;
			_workspaceId = exportSettings.CaseInfo.ArtifactID;
			_currentUserCredential = exportSettings.Credential;
			_cookieContainer = exportSettings.CookieContainer;
		}

		public IRelativityFileShareSettings GetSettingsForFileshare(string fileUrl)
		{
			if (_cachedSettings == null)
			{
				GetFileShareSettingsForWorkspace(AppSettings.Instance.WebApiServiceUrl, _workspaceId, _currentUserCredential.UserName, _currentUserCredential.Password);
			}

			// settings will be null here if the fileUrl belongs to no known file share, e.g. if the path in the database was somehow modified,
			// or if the file share to which it was uploaded no longer exists in Relativity.
			RelativityFileShareSettings settings = _cachedSettings.FirstOrDefault(n => n.IsBaseOf(fileUrl));
			return settings;
		}

		private void GetFileShareSettingsForWorkspace(string hostUrl, int workspaceId, string userName, string password)
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

				ITapiObjectService tapiObjectService = new TapiObjectService();
				RelativityConnectionInfo connectionInfo = tapiObjectService.CreateRelativityConnectionInfo(parameters);
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

					if (results.FileShares.Count == 0)
					{
						throw new FileShareSettingsServiceException(
							$"There are 0 valid FileShares for workspace {workspaceId}.");
					}
					_cachedSettings = results.FileShares.Select(f => new RelativityFileShareSettings(f)).ToList();
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"{nameof(GetFileShareSettingsForWorkspace)}() failed with following error message {0} and stack trace {1}", e.Message, e.StackTrace);
				throw;
			}
		}
	}
}