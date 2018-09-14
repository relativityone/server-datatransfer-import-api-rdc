using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class FileTapiBridgePool : IFileTapiBridgePool
	{
		private readonly IDictionary<RelativityFileShareSettings, PoolEntry> _fileTapiBridges;

		private readonly IExportConfig _exportConfig;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly DownloadProgressManager _downloadProgressManager;
		private readonly FilesStatistics _filesStatistics;
		private readonly IMessagesHandler _messageHandler;
		private readonly ITransferClientHandler _transferClientHandler;
		private readonly ILog _logger;

		public FileTapiBridgePool(IExportConfig exportConfig, TapiBridgeParametersFactory tapiBridgeParametersFactory, DownloadProgressManager downloadProgressManager, FilesStatistics filesStatistics,
			IMessagesHandler messageHandler, ITransferClientHandler transferClientHandler, ILog logger)
		{
			_exportConfig = exportConfig;
			_tapiBridgeParametersFactory = tapiBridgeParametersFactory;
			_downloadProgressManager = downloadProgressManager;
			_filesStatistics = filesStatistics;
			_messageHandler = messageHandler;
			_transferClientHandler = transferClientHandler;
			_logger = logger;

			_fileTapiBridges = new Dictionary<RelativityFileShareSettings, PoolEntry>();
		}

		public IDownloadTapiBridge Request(RelativityFileShareSettings fileshareSettings, CancellationToken token)
		{
			if (!_fileTapiBridges.ContainsKey(fileshareSettings))
			{
				CreateTapiBridge(fileshareSettings, token);
			}

			if (_fileTapiBridges[fileshareSettings].Bridge.ClientType == TapiClient.Web && !_exportConfig.TapiForceHttpClient)
			{
				TryDisposeTapiBridge(_fileTapiBridges[fileshareSettings].Bridge);
				CreateTapiBridge(fileshareSettings, token);
			}

			if (_fileTapiBridges.Values.Count(x => x.Connected) >= _exportConfig.MaxNumberOfFileExportTasks)
			{
				PoolEntry connectedNotUsed = _fileTapiBridges.Values.First(x => x.Connected && !x.InUse);
				connectedNotUsed.Connected = false;
				if (connectedNotUsed != _fileTapiBridges[fileshareSettings])
				{
					connectedNotUsed.Bridge.Disconnect();
				}
			}

			_fileTapiBridges[fileshareSettings].Connected = true;
			_fileTapiBridges[fileshareSettings].InUse = true;

			return _fileTapiBridges[fileshareSettings].Bridge;
		}

		private void CreateTapiBridge(RelativityFileShareSettings fileshareSettings, CancellationToken token)
		{
			ITapiBridgeFactory tapiBridgeFactory =
				new FilesTapiBridgeFactory(_tapiBridgeParametersFactory, _logger, fileshareSettings, token);
			var smartTapiBridge = new SmartTapiBridge(_exportConfig, tapiBridgeFactory, token);

			IProgressHandler progressHandler = new FileDownloadProgressHandler(_downloadProgressManager, _logger);
			var downloadTapiBridgeForFiles = new DownloadTapiBridgeForFiles(smartTapiBridge, progressHandler, _messageHandler, _filesStatistics, _transferClientHandler, _logger);
			_fileTapiBridges[fileshareSettings] = new PoolEntry(downloadTapiBridgeForFiles);
		}

		public void Release(IDownloadTapiBridge bridge)
		{
			_fileTapiBridges.Values.First(x => x.Bridge == bridge).InUse = false;
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