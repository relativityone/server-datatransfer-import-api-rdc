﻿using System;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.EncodingHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class ExportTapiBridgeFactory : IExportTapiBridgeFactory
	{
		private readonly ExportFile _exportSettings;
		private readonly ILog _logger;
		private readonly LongTextEncodingConverterFactory _converterFactory;
		private readonly DownloadProgressManager _downloadProgressManager;
		private readonly IMessagesHandler _messageHandler;
		private readonly ITransferClientHandler _transferClientHandler;
		private readonly FilesStatistics _filesStatistics;
		private readonly MetadataStatistics _metadataStatistics;
		private readonly IExportConfig _exportConfig;

		public ExportTapiBridgeFactory(ExportFile exportSettings, ILog logger, LongTextEncodingConverterFactory converterFactory, DownloadProgressManager downloadProgressManager,
			IMessagesHandler messageHandler, ITransferClientHandler transferClientHandler, FilesStatistics filesStatistics, MetadataStatistics metadataStatistics, IExportConfig exportConfig)
		{
			_exportSettings = exportSettings;
			_logger = logger;
			_converterFactory = converterFactory;
			_downloadProgressManager = downloadProgressManager;
			_messageHandler = messageHandler;
			_transferClientHandler = transferClientHandler;
			_filesStatistics = filesStatistics;
			_metadataStatistics = metadataStatistics;
			_exportConfig = exportConfig;
		}

		public IDownloadTapiBridge CreateForFiles(CancellationToken token)
		{
			ITapiBridge tapiBridge = CreateDownloadTapiBridge(token);

			return new DownloadTapiBridgeForFiles(tapiBridge, new FileDownloadProgressHandler(_downloadProgressManager, _logger), _messageHandler, _filesStatistics, _transferClientHandler,
				_logger);
		}

		public IDownloadTapiBridge CreateForLongText(CancellationToken token)
		{
			TapiBridgeParameters parameters = CreateTapiBridgeParametersFromConfiguration();

			parameters.ForceAsperaClient = false;
			parameters.ForceClientCandidates = string.Empty;
			parameters.ForceFileShareClient = false;
			parameters.ForceHttpClient = true;

			DownloadTapiBridge downloadTapiBridge = TapiBridgeFactory.CreateDownloadBridge(parameters, _logger, token);
			ITapiBridge tapiBridge = new TapiBridgeWrapper(downloadTapiBridge);

			LongTextEncodingConverter longTextEncodingConverter = _converterFactory.Create(token);
			return new DownloadTapiBridgeWithEncodingConversion(tapiBridge, new LongTextProgressHandler(_downloadProgressManager, _logger), _messageHandler, _metadataStatistics,
				longTextEncodingConverter, _logger);
		}

		private ITapiBridge CreateDownloadTapiBridge(CancellationToken token)
		{
			TapiBridgeParameters parameters = CreateTapiBridgeParametersFromConfiguration();
			DownloadTapiBridge tapiBridge = TapiBridgeFactory.CreateDownloadBridge(parameters, _logger, token);
			tapiBridge.DumpInfo();
			return new TapiBridgeWrapper(tapiBridge);
		}

		private TapiBridgeParameters CreateTapiBridgeParametersFromConfiguration()
		{
			TapiBridgeParameters parameters = new TapiBridgeParameters
			{
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
				TargetPath = string.Empty,
				TargetDataRateMbps = Config.TapiTargetDataRateMbps,
				TransferLogDirectory = Config.TapiTransferLogDirectory,
				WaitTimeBetweenRetryAttempts = _exportConfig.ExportIOErrorWaitTime,
				WebCookieContainer = _exportSettings.CookieContainer,
				WebServiceUrl = Config.WebServiceURL,
				WorkspaceId = _exportSettings.CaseInfo.ArtifactID
			};
			return parameters;
		}
	}
}