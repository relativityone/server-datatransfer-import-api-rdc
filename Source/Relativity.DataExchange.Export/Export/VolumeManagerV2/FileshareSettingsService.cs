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
	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.Resources;
	using Relativity.DataExchange.Transfer;

	public class FileShareSettingsService : IFileShareSettingsService
	{
		private readonly IStatus _status;
		private readonly ITapiObjectService _tapiObjectService;
		private readonly ILog _logger;

		private readonly List<RelativityFileShareSettings> _nonDefaultFileShareSettings = new List<RelativityFileShareSettings>();
		private readonly TapiBridgeParameters2 _parameters;
		private RelativityFileShareSettings _defaultFileShareSettings;
		private bool _readFileShares;
		private bool _cloudInstance;

		public FileShareSettingsService(
			IStatus status,
			ITapiObjectService tapiObjectService,
			ILog logger,
			ExportFile settings)
		{
			_status = status.ThrowIfNull(nameof(status));
			_tapiObjectService = tapiObjectService.ThrowIfNull(nameof(tapiObjectService));
			_logger = logger.ThrowIfNull(nameof(logger));
			var settings1 = settings.ThrowIfNull(nameof(settings));
			if (settings.CaseInfo == null)
			{
				throw new ArgumentException(
					ExportStrings.ExportSettingsNullWorkspaceExceptionMessage,
					nameof(settings));
			}

			if (settings.Credential == null)
			{
				throw new ArgumentException(
					ExportStrings.ExportSettingsNullCredentialExceptionMessage,
					nameof(settings));
			}

			_parameters = new TapiBridgeParameters2
				              {
					              Credentials = settings1.Credential,
					              WebCookieContainer = settings1.CookieContainer,
					              WebServiceUrl = AppSettings.Instance.WebApiServiceUrl,
					              WorkspaceId = settings.CaseInfo.ArtifactID
				              };
		}

		public async Task ReadFileSharesAsync(CancellationToken token)
		{
			// This can be expensive and only need to read 1 time.
			if (_readFileShares)
			{
				return;
			}

			try
			{
				_status.WriteStatusLineWithoutDocCount(
					EventType2.Status,
					ExportStrings.FileStorageStartedStatusMessage,
					true);

				// The code below can completely fail but still allow export to function.
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

				ITapiFileStorageSearchResults results = await _tapiObjectService
					                                        .SearchFileStorageAsync(_parameters, _logger, token)
					                                        .ConfigureAwait(false);
				_logger.LogInformation(
					"File storage search API discovered {TotalValidFileShares} valid file shares and {TotalInvalidFileShares} invalid files shares associated with workspace {WorkspaceId}.",
					results.FileShares.Count,
					results.InvalidFileShares.Count,
					_parameters.WorkspaceId);
				_cloudInstance = results.CloudInstance;
				_defaultFileShareSettings = null;
				_nonDefaultFileShareSettings.Clear();
				if (results.FileShares.Count > 0)
				{
					foreach (RelativityFileShare fileShare in results.FileShares.OrderByDescending(x => x.ArtifactId))
					{
						if (fileShare.ArtifactId == defaultFileShare.ArtifactId)
						{
							_defaultFileShareSettings = new RelativityFileShareSettings(fileShare);
						}
						else
						{
							_nonDefaultFileShareSettings.Add(new RelativityFileShareSettings(fileShare));
						}

						_logger.LogInformation(
							fileShare.ArtifactId == defaultFileShare.ArtifactId
								? "File storage search API discovered valid default file share {FileShareArtifactId} associated with workspace {WorkspaceId} and is added to the valid file share list."
								: "File storage search API discovered valid non-default file share {FileShareArtifactId} associated with workspace {WorkspaceId} and is added to the valid file share list.",
							fileShare.ArtifactId,
							_parameters.WorkspaceId);
					}
				}
				else
				{
					_logger.LogWarning(
						"File storage search API discovered zero valid file shares associated with workspace {WorkspaceId}. This doesn't prevent export from working but performance could be degraded.",
						_parameters.WorkspaceId);
					string warningMessage = string.Format(
						CultureInfo.CurrentCulture,
						ExportStrings.FileStorageZeroValidFileSharesWarningMessage,
						_parameters.WorkspaceId);
					_status.WriteWarningWithoutDocCount(warningMessage);
				}

				if (results.InvalidFileShares.Count > 0)
				{
					foreach (RelativityFileShare invalidFileShare in results.InvalidFileShares)
					{
						if (invalidFileShare.ArtifactId == defaultFileShare.ArtifactId)
						{
							_logger.LogWarning(
								"File storage search API discovered invalid default file share {FileShareArtifactId} associated with workspace {WorkspaceId}. Error: '{FileShareError}'.",
								invalidFileShare.ArtifactId,
								_parameters.WorkspaceId,
								invalidFileShare.Error);
							string warningMessage = string.Format(
								CultureInfo.CurrentCulture,
								ExportStrings.FileStorageInvalidDefaultFileShareWarningMessage,
								invalidFileShare.ArtifactId,
								_parameters.WorkspaceId,
								invalidFileShare.Error);
							_status.WriteWarningWithoutDocCount(warningMessage);
						}
						else
						{
							_logger.LogWarning(
								"File storage search API discovered invalid non-default file share {FileShareArtifactId} associated with workspace {WorkspaceId}. Error: '{FileShareError}'.",
								invalidFileShare.ArtifactId,
								_parameters.WorkspaceId,
								invalidFileShare.Error);
							string warningMessage = string.Format(
								CultureInfo.CurrentCulture,
								ExportStrings.FileStorageInvalidNonDefaultFileShareWarningMessage,
								invalidFileShare.ArtifactId,
								_parameters.WorkspaceId,
								invalidFileShare.Error);
							_status.WriteWarningWithoutDocCount(warningMessage);
						}
					}
				}

				_status.WriteStatusLineWithoutDocCount(
					EventType2.Status,
					ExportStrings.FileStorageCompletedStatusMessage,
					true);
				_readFileShares = true;
			}
			catch (OperationCanceledException)
			{
				throw;
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
				string warningMessage = string.Format(
					CultureInfo.CurrentCulture,
					ExportStrings.FileStorageExceptionWarningMessage,
					_parameters.WorkspaceId,
					e.Message);
				_status.WriteWarningWithoutDocCount(warningMessage);
			}
		}

		public IRelativityFileShareSettings GetSettingsForFileShare(int artifactId, string path)
		{
			// Note: returning null is valid.
			IRelativityFileShareSettings settings;
			try
			{
				if (_defaultFileShareSettings == null)
				{
					// A complete file storage search failure does NOT cause export to fail.
					settings = null;
				}
				else if (!_cloudInstance
				         || (_defaultFileShareSettings != null && _defaultFileShareSettings.IsBaseOf(path)))
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
						"The path for export artifact '{ArtifactId}' does not match the base address on any of the file shares and will be exported by either direct or web mode. This may be caused by an invalid File table or the Resource Pool doesn't include all file shares referenced by this workspace.",
						artifactId);
				}
			}
			catch (Exception e)
			{
				// Note: this exception is caught to prevent the entire batch or job from aborting due to an invalid artifact.
				// Note: data validation checks now exist to ensure invalid artifacts are handled properly.
				if (ExceptionHelper.IsFatalException(e))
				{
					throw;
				}

				_logger.LogWarning(
					e,
					"The path for export artifact '{ArtifactId}' failed trying to match the base address on any of the file shares. This may be caused by an invalid artifact.",
					artifactId);
				settings = null;
			}

			return settings;
		}
	}
}