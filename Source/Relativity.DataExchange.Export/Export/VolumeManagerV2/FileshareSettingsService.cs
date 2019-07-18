namespace Relativity.DataExchange.Export.VolumeManagerV2
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Relativity.Logging;
	using Relativity.Transfer;

	using kCura.WinEDDS;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Resources;
	using Relativity.DataExchange.Transfer;

	public class FileShareSettingsService : IFileShareSettingsService
	{
		private readonly ITapiObjectService _tapiObjectService;
		private readonly ILog _logger;
		private readonly ExportFile _settings;
		private readonly List<RelativityFileShareSettings> _nonDefaultFileShareSettings = new List<RelativityFileShareSettings>();
		private readonly TapiBridgeParameters2 _parameters;
		private RelativityFileShareSettings _defaultFileShareSettings;
		private bool _cloudInstance;

		public FileShareSettingsService(ITapiObjectService tapiObjectService, ILog logger, ExportFile settings)
		{
			_tapiObjectService = tapiObjectService.ThrowIfNull(nameof(tapiObjectService));
			_logger = logger.ThrowIfNull(nameof(logger));
			_settings = settings.ThrowIfNull(nameof(settings));
			if (settings.CaseInfo == null)
			{
				throw new ArgumentException(ExportStrings.ExportSettingsNullWorkspaceExceptionMessage, nameof(settings));
			}

			if (settings.Credential == null)
			{
				throw new ArgumentException(ExportStrings.ExportSettingsNullCredentialExceptionMessage, nameof(settings));
			}

			_parameters = new TapiBridgeParameters2
				              {
					              Credentials = _settings.Credential,
					              WebCookieContainer = _settings.CookieContainer,
					              WebServiceUrl = AppSettings.Instance.WebApiServiceUrl,
					              WorkspaceId = settings.CaseInfo.ArtifactID
				              };
		}

		public async Task ReadFileSharesAsync(CancellationToken token)
		{
			// The code below can completely fail but still allow export to function.
			if (_defaultFileShareSettings != null)
			{
				return;
			}

			try
			{
				RelativityFileShare defaultFileShare = await _tapiObjectService
					                                       .GetWorkspaceDefaultFileShareAsync(
						                                       _parameters,
						                                       _logger,
						                                       token).ConfigureAwait(false);
				if (defaultFileShare == null)
				{
					string message = string.Format(
						CultureInfo.CurrentCulture,
						ExportStrings.WorkspaceDefaultFileshareNullExceptionMessage,
						_parameters.WorkspaceId);
					throw new TransferException(message);
				}

				_defaultFileShareSettings = new RelativityFileShareSettings(defaultFileShare);
				ITapiFileStorageSearchResults results =
					await _tapiObjectService.SearchFileStorageAsync(_parameters, _logger, token);
				_logger.LogInformation(
					"File storage search API discovered {TotalValidFileShares} valid file shares and {TotalInvalidFileShares} invalid files shares associated with workspace {WorkspaceId}.",
					results.FileShares.Count,
					results.InvalidFileShares.Count,
					_parameters.WorkspaceId);
				_cloudInstance = results.CloudInstance;
				List<RelativityFileShareSettings> validFileShares = new List<RelativityFileShareSettings>();
				if (results.FileShares.Count > 0)
				{
					foreach (RelativityFileShare fileShare in results.FileShares)
					{
						validFileShares.Add(new RelativityFileShareSettings(fileShare));
						_logger.LogInformation(
							"File storage search API discovered valid file share {FileShareArtifactId} associated with workspace {WorkspaceId} and is added to the valid file share list.",
							fileShare.ArtifactId,
							_parameters.WorkspaceId);
					}
				}
				else
				{
					_logger.LogWarning(
						"File storage search API discovered zero valid file shares associated with workspace {WorkspaceId}. This doesn't prevent export from working but performance could be degraded.",
						_parameters.WorkspaceId);
				}

				if (results.InvalidFileShares.Count > 0)
				{
					foreach (RelativityFileShare invalidFileShare in results.InvalidFileShares)
					{
						_logger.LogWarning(
							"File storage search API discovered invalid file share {FileShareArtifactId} associated with workspace {WorkspaceId}. Error: '{FileShareError}'.",
							invalidFileShare.ArtifactId,
							_parameters.WorkspaceId,
							invalidFileShare.Error);
					}
				}

				// Maintain a separate list for all other non-default file shares that are available.
				// Sorting in descending order to ensure the more recent file shares are accessed first.
				_nonDefaultFileShareSettings.Clear();
				foreach (RelativityFileShareSettings settings in validFileShares.OrderByDescending(x => x.ArtifactId))
				{
					if (!settings.Equals(_defaultFileShareSettings))
					{
						_nonDefaultFileShareSettings.Add(settings);
					}
				}
			}
			catch (Exception e)
			{
				if (ExceptionHelper.IsFatalException(e))
				{
					throw;
				}

				// Intentionally eating this exception. See below for details.
				_logger.LogWarning(
					e,
					"Failed to retrieve the file shares associated with workspace {WorkspaceId}. This doesn't prevent export from working but performance could be degraded.",
					_parameters.WorkspaceId);
			}
		}

		public IRelativityFileShareSettings GetSettingsForFileShare(int artifactId, string path)
		{
			// Note: returning null is valid.
			IRelativityFileShareSettings settings;
			if (_defaultFileShareSettings == null)
			{
				// A complete file storage search failure does NOT cause export to fail.
				settings = null;
			}
			else if (!_cloudInstance || (_defaultFileShareSettings != null && _defaultFileShareSettings.IsBaseOf(path)))
			{
				// For non-cloud instances, all supported transfer clients work using only the default file share.
				settings = _defaultFileShareSettings;
			}
			else
			{
				// Check all other file shares to find a match.
				settings = _nonDefaultFileShareSettings.FirstOrDefault(n => n.IsBaseOf(path));
			}

			if (settings == null)
			{
				_logger.LogWarning(
					"The export path for export artifact '{ArtifactId}' does not match the base address on any of the file shares and will be exported by either direct or web mode. This may be caused by an invalid File table or the Resource Pool doesn't include all file shares referenced by this workspace.",
					artifactId);
			}

			return settings;
		}
	}
}