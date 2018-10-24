using System;
using kCura.WinEDDS.TApi;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class TapiBridgeParametersFactory
	{
		private readonly IExportConfig _exportConfig;
		private readonly ExportFile _exportSettings;

		public TapiBridgeParametersFactory(ExportFile exportSettings, IExportConfig exportConfig)
		{
			_exportSettings = exportSettings;
			_exportConfig = exportConfig;
		}

		public TapiBridgeParameters CreateTapiBridgeParametersFromConfiguration()
		{
			TapiBridgeParameters parameters = new TapiBridgeParameters
			{
				Application = Config.ApplicationName,
				ClientRequestId = Guid.NewGuid(),
				Credentials = _exportSettings.Credential,
				AsperaDocRootLevels = Config.TapiAsperaNativeDocRootLevels,
				FileShare = _exportSettings.CaseInfo.DocumentPath,
				ForceAsperaClient = Config.TapiForceAsperaClient,
				ForceClientCandidates = Config.TapiForceClientCandidates,
				ForceFileShareClient = Config.TapiForceFileShareClient,
				ForceHttpClient = Config.TapiForceHttpClient,
				LargeFileProgressEnabled = Config.TapiLargeFileProgressEnabled,
				LogConfigFile = Config.LogConfigFile,
				MaxJobParallelism = Config.TapiMaxJobParallelism,
				MaxJobRetryAttempts = _exportConfig.ExportIOErrorNumberOfRetries,
				MinDataRateMbps = Config.TapiMinDataRateMbps,
				SubmitApmMetrics = Config.TapiSubmitApmMetrics,
				TargetPath = string.Empty,
				TargetDataRateMbps = Config.TapiTargetDataRateMbps,
				TransferLogDirectory = Config.TapiTransferLogDirectory,
				WaitTimeBetweenRetryAttempts = _exportConfig.ExportIOErrorWaitTime,
				WebCookieContainer = _exportSettings.CookieContainer,
				WebServiceUrl = Config.WebServiceURL,
				WorkspaceId = _exportSettings.CaseInfo.ArtifactID,
			};
			return parameters;
		}
	}
}