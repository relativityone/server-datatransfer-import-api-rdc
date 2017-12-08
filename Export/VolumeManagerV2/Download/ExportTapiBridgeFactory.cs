using System;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	/// <summary>
	///     TODO refactor!
	/// </summary>
	public class ExportTapiBridgeFactory
	{
		private readonly ExportFile _exportSettings;
		private readonly ILog _logger;
		private readonly LongTextEncodingConverterFactory _converterFactory;
		private readonly DownloadStatistics _downloadStatistics;
		private readonly IMessagesHandler _messageHandler;
		private readonly ITransferClientHandler _transferClientHandler;

		public ExportTapiBridgeFactory(ExportFile exportSettings, ILog logger, LongTextEncodingConverterFactory converterFactory, DownloadStatistics downloadStatistics,
			IMessagesHandler messageHandler, ITransferClientHandler transferClientHandler)
		{
			_exportSettings = exportSettings;
			_logger = logger;
			_converterFactory = converterFactory;
			_downloadStatistics = downloadStatistics;
			_messageHandler = messageHandler;
			_transferClientHandler = transferClientHandler;
		}

		public IDownloadTapiBridge CreateForNatives(CancellationToken token)
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

			//AttachEventHandlers(tapiBridgeAdapter);

			return new DownloadTapiBridgeForFiles(tapiBridge, new NativeFilesProgressHandler(_downloadStatistics, _logger), _messageHandler, _transferClientHandler, _logger);
		}

		private void AttachEventHandlers(DownloadTapiBridgeAdapter tapiBridgeAdapter)
		{
			//TODO
			//tapiBridgeAdapter.TapiClientChanged += _exportFileDownloaderStatus.OnTapiClientChanged;
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
			return new DownloadTapiBridgeWithEncodingConversion(tapiBridge, new LongTextProgressHandler(_downloadStatistics, _logger), _messageHandler, longTextEncodingConverter, _logger);
		}

		public IDownloadTapiBridge CreateForImages(CancellationToken token)
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

			//AttachEventHandlers(tapiBridgeAdapter);

			return new DownloadTapiBridgeForFiles(tapiBridge, new ImageFilesProgressHandler(_downloadStatistics, _logger), _messageHandler, _transferClientHandler, _logger);
		}
	}
}