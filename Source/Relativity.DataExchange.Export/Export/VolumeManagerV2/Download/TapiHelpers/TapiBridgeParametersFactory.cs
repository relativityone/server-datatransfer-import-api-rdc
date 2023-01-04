namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;

	using kCura.WinEDDS;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Resources;
	using Relativity.DataExchange.Transfer;

	public class TapiBridgeParametersFactory
	{
		private readonly IExportConfig _exportConfig;
		private readonly ExportFile _exportSettings;
		private readonly IAppSettings _appSettings;

		public TapiBridgeParametersFactory(ExportFile exportSettings, IExportConfig exportConfig)
			: this(exportSettings, exportConfig, AppSettings.Instance)
		{
		}

		public TapiBridgeParametersFactory(
			ExportFile exportSettings,
			IExportConfig exportConfig,
			IAppSettings appSettings)
		{
			_exportSettings = exportSettings.ThrowIfNull(nameof(exportSettings));
			_exportConfig = exportConfig.ThrowIfNull(nameof(exportConfig));
			_appSettings = appSettings.ThrowIfNull(nameof(appSettings));
		}

		public DownloadTapiBridgeParameters2 CreateTapiBridgeParametersFromConfiguration()
		{
			if (_exportSettings.CaseInfo == null)
			{
				throw new InvalidOperationException(ExportStrings.ExportSettingsNullWorkspaceExceptionMessage);
			}

			DownloadTapiBridgeParameters2 parameters = new DownloadTapiBridgeParameters2
			{
				Application = _appSettings.ApplicationName,
				AsperaBcpRootFolder = _appSettings.TapiAsperaBcpRootFolder,
				AsperaDocRootLevels = _appSettings.TapiAsperaNativeDocRootLevels,
				AsperaDatagramSize = _appSettings.TapiAsperaDatagramSize,
				BadPathErrorsRetry = _appSettings.TapiBadPathErrorsRetry,
				ClientRequestId = Guid.NewGuid(),
				Credentials = _exportSettings.Credential,
				FileShare = _exportSettings.CaseInfo.DocumentPath,
				ForceAsperaClient = _appSettings.TapiForceAsperaClient,
				ForceClientCandidates = _appSettings.TapiForceClientCandidates,
				ForceFileShareClient = _appSettings.TapiForceFileShareClient,
				ForceHttpClient = _appSettings.TapiForceHttpClient,
				LargeFileProgressEnabled = _appSettings.TapiLargeFileProgressEnabled,
				LogConfigFile = _appSettings.LogConfigXmlFileName,
				MaxInactivitySeconds = _appSettings.TapiMaxInactivitySeconds,
				MaxJobParallelism = _appSettings.TapiMaxJobParallelism,
				MaxJobRetryAttempts = _exportConfig.ExportIOErrorNumberOfRetries,
				MinDataRateMbps = _appSettings.TapiMinDataRateMbps,
				PermissionErrorsRetry = _appSettings.ExportPermissionErrorsRetry,
				FileNotFoundErrorsDisabled = _appSettings.TapiExportFileNotFoundErrorsDisabled,
				FileNotFoundErrorsRetry = _appSettings.TapiExportFileNotFoundErrorsRetry,
				PreserveFileTimestamps = _appSettings.TapiPreserveFileTimestamps,
				SubmitApmMetrics = _appSettings.TapiSubmitApmMetrics,
				TargetPath = string.Empty,
				TargetDataRateMbps = _appSettings.TapiTargetDataRateMbps,
				TimeoutSeconds = _appSettings.HttpTimeoutSeconds,
				TransferLogDirectory = _appSettings.TapiTransferLogDirectory,
				WaitTimeBetweenRetryAttempts = _exportConfig.ExportIOErrorWaitTime,
				WebCookieContainer = _exportSettings.CookieContainer,
				WebServiceUrl = _appSettings.WebApiServiceUrl,
				WorkspaceId = _exportSettings.CaseInfo.ArtifactID,
			};

			return parameters;
		}
	}
}