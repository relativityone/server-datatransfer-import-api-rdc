namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Transfer;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.Logging;

	public class FileTapiBridgePool : IFileTapiBridgePool
	{
		private readonly object _sync = new object();
		private readonly IDictionary<IRelativityFileShareSettings, PoolEntry> _fileTapiBridges;
		private readonly IExportConfig _exportConfig;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly DownloadProgressManager _downloadProgressManager;
		private readonly FilesStatistics _filesStatistics;
		private readonly IMessagesHandler _messageHandler;
		private readonly ITransferClientHandler _transferClientHandler;
		private readonly ILog _logger;

		public FileTapiBridgePool(IExportConfig exportConfig,
			TapiBridgeParametersFactory factory,
			DownloadProgressManager downloadProgressManager,
			FilesStatistics filesStatistics,
			IMessagesHandler messageHandler,
			ITransferClientHandler transferClientHandler,
			ILog logger)
		{
			exportConfig.ThrowIfNull(nameof(exportConfig));
			factory.ThrowIfNull(nameof(factory));
			downloadProgressManager.ThrowIfNull(nameof(downloadProgressManager));
			filesStatistics.ThrowIfNull(nameof(filesStatistics));
			messageHandler.ThrowIfNull(nameof(messageHandler));
			transferClientHandler.ThrowIfNull(nameof(transferClientHandler));
			logger.ThrowIfNull(nameof(logger));
			_exportConfig = exportConfig;
			_tapiBridgeParametersFactory = factory;
			_downloadProgressManager = downloadProgressManager;
			_filesStatistics = filesStatistics;
			_messageHandler = messageHandler;
			_transferClientHandler = transferClientHandler;
			_logger = logger;
			_fileTapiBridges = new Dictionary<IRelativityFileShareSettings, PoolEntry>();
		}

		public IDownloadTapiBridge Request(IRelativityFileShareSettings settings, CancellationToken token)
		{
			if (settings == null)
			{
				// settings will be null if the source path for a file does not correspond to any known file server.
				// Since we can't create a bridge without file share settings, we return a default implementation that reports
				// errors for all attempted downloads.

				return ErrorReportingTapiBridge;
			}

			if (!_fileTapiBridges.ContainsKey(settings))
			{
				CreateTapiBridge(settings, token);
			}

			if (_fileTapiBridges[settings].Bridge.Client == TapiClient.Web && !_exportConfig.TapiForceHttpClient)
			{
				TryDisposeTapiBridge(_fileTapiBridges[settings].Bridge);
				CreateTapiBridge(settings, token);
			}

			PoolEntry connectedNotUsed = FindRedundantTapiBridge();

			if (connectedNotUsed != null && connectedNotUsed != _fileTapiBridges[settings])
			{
				connectedNotUsed.Bridge.Disconnect();
			}

			_fileTapiBridges[settings].Connected = true;
			_fileTapiBridges[settings].InUse = true;
			return _fileTapiBridges[settings].Bridge;
		}

		private PoolEntry FindRedundantTapiBridge()
		{
			PoolEntry connectedNotUsed = null;
			lock (_sync)
			{
				if (_fileTapiBridges.Values.Count(x => x.Connected) >= _exportConfig.MaxNumberOfFileExportTasks)
				{
					connectedNotUsed = _fileTapiBridges.Values.FirstOrDefault(x => x.Connected && !x.InUse);
					if (connectedNotUsed != null)
					{
						connectedNotUsed.Connected = false;
					}
				}
			}

			return connectedNotUsed;
		}

		private IDownloadTapiBridge ErrorReportingTapiBridge
		{
			get
			{
				var tapiBridge = new ErrorReportingTapiBridge();
				IProgressHandler progressHandler = new FileDownloadProgressHandler(_downloadProgressManager, _logger);
				var downloadTapiBridge = new DownloadTapiBridgeForFiles(tapiBridge, progressHandler, _messageHandler, _filesStatistics, _transferClientHandler, _logger);
				return downloadTapiBridge;
			}
		}

		private void CreateTapiBridge(IRelativityFileShareSettings settings, CancellationToken token)
		{
			ITapiBridgeFactory tapiBridgeFactory = new FilesTapiBridgeFactory(
				_tapiBridgeParametersFactory,
				_logger,
				settings,
				token);
			ITapiBridge bridge = tapiBridgeFactory.Create();
			IProgressHandler progressHandler = new FileDownloadProgressHandler(_downloadProgressManager, _logger);
			var downloadTapiBridgeForFiles = new DownloadTapiBridgeForFiles(bridge, progressHandler, _messageHandler, _filesStatistics, _transferClientHandler, _logger);
			_fileTapiBridges[settings] = new PoolEntry(downloadTapiBridgeForFiles);
		}

		public void Release(IDownloadTapiBridge bridge)
		{
			PoolEntry entryForBridge = _fileTapiBridges.Values.FirstOrDefault(x => x.Bridge == bridge);

			// entryForBridge will be null if we returned an ErrorReportingTapiBridge when this bridge was requested.
			if (entryForBridge != null)
			{
				entryForBridge.InUse = false;
			}
		}

		public void Dispose()
		{
			foreach (var tapiBridge in _fileTapiBridges.Values)
			{
				TryDisposeTapiBridge(tapiBridge.Bridge);
			}
		}

		private void TryDisposeTapiBridge(IDownloadTapiBridge bridge)
		{
			try
			{
				bridge?.Dispose();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to dispose Tapi Bridge.");
				// We do not want to crash container disposal
			}
		}
	}
}