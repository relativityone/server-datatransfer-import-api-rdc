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
		private readonly DownloadProgressManager _downloadProgressManager;
		private readonly IMessagesHandler _messageHandler;
		private readonly ITransferClientHandler _transferClientHandler;
		private readonly FilesStatistics _filesStatistics;
		private readonly MetadataStatistics _metadataStatistics;

		public ExportTapiBridgeFactory(ExportFile exportSettings, ILog logger, LongTextEncodingConverterFactory converterFactory, DownloadProgressManager downloadProgressManager,
			IMessagesHandler messageHandler, ITransferClientHandler transferClientHandler, FilesStatistics filesStatistics, MetadataStatistics metadataStatistics)
		{
			_exportSettings = exportSettings;
			_logger = logger;
			_converterFactory = converterFactory;
			_downloadProgressManager = downloadProgressManager;
			_messageHandler = messageHandler;
			_transferClientHandler = transferClientHandler;
			_filesStatistics = filesStatistics;
			_metadataStatistics = metadataStatistics;
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
			tapiBridge.DumpInfo();
			
			return new DownloadTapiBridgeForFiles(tapiBridge, new NativeFilesProgressHandler(_downloadProgressManager, _logger), _messageHandler, _filesStatistics, _transferClientHandler,
				_logger);
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
			tapiBridge.DumpInfo();
			return new DownloadTapiBridgeWithEncodingConversion(tapiBridge, new LongTextProgressHandler(_downloadProgressManager, _logger), _messageHandler, _metadataStatistics,
				longTextEncodingConverter, _logger);
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
			tapiBridge.DumpInfo();

			return new DownloadTapiBridgeForFiles(tapiBridge, new ImageFilesProgressHandler(_downloadProgressManager, _logger), _messageHandler, _filesStatistics, _transferClientHandler,
				_logger);
		}
	}
}