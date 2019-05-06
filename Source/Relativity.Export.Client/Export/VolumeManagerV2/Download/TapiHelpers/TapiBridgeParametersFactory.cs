namespace Relativity.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;

	using global::Relativity.Import.Export;

	using kCura.WinEDDS;

	using Relativity.Import.Export.Transfer;

	public class TapiBridgeParametersFactory
	{
		private readonly IExportConfig _exportConfig;
		private readonly ExportFile _exportSettings;

		public TapiBridgeParametersFactory(ExportFile exportSettings, IExportConfig exportConfig)
		{
			_exportSettings = exportSettings;
			_exportConfig = exportConfig;
		}

		public TapiBridgeParameters2 CreateTapiBridgeParametersFromConfiguration()
		{
			TapiBridgeParameters2 parameters = new TapiBridgeParameters2
			{
				Application = AppSettings.Instance.ApplicationName,
				AsperaBcpRootFolder = AppSettings.Instance.TapiAsperaBcpRootFolder,
				AsperaDocRootLevels = AppSettings.Instance.TapiAsperaNativeDocRootLevels,
				BadPathErrorsRetry = AppSettings.Instance.TapiBadPathErrorsRetry,
				ClientRequestId = Guid.NewGuid(),
				Credentials = _exportSettings.Credential,
				FileShare = _exportSettings.CaseInfo.DocumentPath,
				ForceAsperaClient = AppSettings.Instance.TapiForceAsperaClient,
				ForceClientCandidates = AppSettings.Instance.TapiForceClientCandidates,
				ForceFileShareClient = AppSettings.Instance.TapiForceFileShareClient,
				ForceHttpClient = AppSettings.Instance.TapiForceHttpClient,
				LargeFileProgressEnabled = AppSettings.Instance.TapiLargeFileProgressEnabled,
				LogConfigFile = AppSettings.Instance.LogConfigXmlFileName,
				MaxJobParallelism = AppSettings.Instance.TapiMaxJobParallelism,
				MaxJobRetryAttempts = _exportConfig.ExportIOErrorNumberOfRetries,
				MinDataRateMbps = AppSettings.Instance.TapiMinDataRateMbps,
				PermissionErrorsRetry = AppSettings.Instance.PermissionErrorsRetry,
				PreserveFileTimestamps = AppSettings.Instance.TapiPreserveFileTimestamps,
				SubmitApmMetrics = AppSettings.Instance.TapiSubmitApmMetrics,
				TargetPath = string.Empty,
				TargetDataRateMbps = AppSettings.Instance.TapiTargetDataRateMbps,
				TimeoutSeconds = AppSettings.Instance.HttpTimeoutSeconds,
				TransferLogDirectory = AppSettings.Instance.TapiTransferLogDirectory,
				WaitTimeBetweenRetryAttempts = _exportConfig.ExportIOErrorWaitTime,
				WebCookieContainer = _exportSettings.CookieContainer,
				WebServiceUrl = AppSettings.Instance.WebApiServiceUrl,
				WorkspaceId = _exportSettings.CaseInfo.ArtifactID,
			};

			return parameters;
		}
	}
}