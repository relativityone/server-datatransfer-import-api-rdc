using System;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class ExportTapiBridgeFactory
	{
		private readonly ExportFile _exportSettings;
		private readonly ILog _logger;
		private readonly ExportFileDownloaderStatus _exportFileDownloaderStatus;
		private readonly LongTextEncodingConverterFactory _converterFactory;

		public ExportTapiBridgeFactory(ExportFile exportSettings, ILog logger, ExportFileDownloaderStatus exportFileDownloaderStatus, LongTextEncodingConverterFactory converterFactory)
		{
			_exportSettings = exportSettings;
			_logger = logger;
			_exportFileDownloaderStatus = exportFileDownloaderStatus;
			_converterFactory = converterFactory;
		}

		public IDownloadTapiBridge Create(CancellationToken token)
		{
			//TODO probably we should create parameters for download first (in tapi bridge)
			TapiBridgeParameters parameters = new TapiBridgeParameters
			{
				BcpFileTransfer = false,
				AsperaBcpRootFolder = string.Empty,
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
				MaxFilesPerFolder = 0,
				MaxJobParallelism = kCura.Utility.Config.IOErrorNumberOfRetries,
				MinDataRateMbps = Config.TapiMinDataRateMbps,
				TargetPath = string.Empty, /*TODO*/
				TargetDataRateMbps = Config.TapiTargetDataRateMbps,
				TransferLogDirectory = Config.TapiTransferLogDirectory,
				WaitTimeBetweenRetryAttempts = kCura.Utility.Config.IOErrorWaitTimeInSeconds,
				WebCookieContainer = _exportSettings.CookieContainer,
				WebServiceUrl = Config.WebServiceURL,
				WorkspaceId = _exportSettings.CaseInfo.ArtifactID
			};

			DownloadTapiBridge tapiBridge = TapiBridgeFactory.CreateDownloadBridge(parameters, _logger, token);

			AttachEventHandlers(tapiBridge);

			return new DownloadTapiBridgeWrapper(tapiBridge);
		}

		private void AttachEventHandlers(DownloadTapiBridge tapiBridge)
		{
			tapiBridge.TapiClientChanged += _exportFileDownloaderStatus.OnTapiClientChanged;
		}

		public IDownloadTapiBridge CreateForLongText(CancellationToken token)
		{
			//TODO probably we should create parameters for download first (in tapi bridge)
			TapiBridgeParameters parameters = new TapiBridgeParameters
			{
				BcpFileTransfer = false,
				AsperaBcpRootFolder = string.Empty,
				ClientRequestId = Guid.NewGuid(),
				Credentials = _exportSettings.Credential,
				AsperaDocRootLevels = Config.TapiAsperaNativeDocRootLevels,
				FileShare = _exportSettings.CaseInfo.DocumentPath,
				ForceAsperaClient = false,
				ForceClientCandidates = string.Empty,
				ForceFileShareClient = false,
				ForceHttpClient = true,
				LargeFileProgressEnabled = Config.TapiLargeFileProgressEnabled,
				LogConfigFile = Config.LogConfigFile,
				MaxFilesPerFolder = 0,
				MaxJobParallelism = kCura.Utility.Config.IOErrorNumberOfRetries,
				MinDataRateMbps = Config.TapiMinDataRateMbps,
				TargetPath = string.Empty, /*TODO*/
				TargetDataRateMbps = Config.TapiTargetDataRateMbps,
				TransferLogDirectory = Config.TapiTransferLogDirectory,
				WaitTimeBetweenRetryAttempts = kCura.Utility.Config.IOErrorWaitTimeInSeconds,
				WebCookieContainer = _exportSettings.CookieContainer,
				WebServiceUrl = Config.WebServiceURL,
				WorkspaceId = _exportSettings.CaseInfo.ArtifactID
			};

			return CreateTapiBridgeForLongText(parameters, token);
		}

		private IDownloadTapiBridge CreateTapiBridgeForLongText(TapiBridgeParameters parameters, CancellationToken token)
		{
			LongTextEncodingConverter longTextEncodingConverter = _converterFactory.Create(token);
			DownloadTapiBridge tapiBridge = TapiBridgeFactory.CreateDownloadBridge(parameters, _logger, token);
			return new DownloadTapiBridgeWithEncodingConversion(tapiBridge, longTextEncodingConverter, _logger);
		}
	}
}