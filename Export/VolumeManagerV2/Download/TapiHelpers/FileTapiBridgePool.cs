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
		private readonly IDictionary<RelativityFileShareSettings, IDownloadTapiBridge> _fileTapiBridges;
		private readonly IList<IDownloadTapiBridge> _fileTapiBridgesInUse;
		private readonly IList<IDownloadTapiBridge> _fileTapiBridgesConnected;

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

			_fileTapiBridges = new Dictionary<RelativityFileShareSettings, IDownloadTapiBridge>();
			_fileTapiBridgesInUse = new List<IDownloadTapiBridge>();
			_fileTapiBridgesConnected = new List<IDownloadTapiBridge>();
		}

		public IDownloadTapiBridge Request(RelativityFileShareSettings fileshareSettings, CancellationToken token)
		{
			if (!_fileTapiBridges.ContainsKey(fileshareSettings))
			{
				CreateTapiBridge(fileshareSettings, token);
			}

			if (_fileTapiBridges[fileshareSettings].ClientType == TapiClient.Web && !_exportConfig.TapiForceHttpClient)
			{
				TryDisposeTapiBridge(_fileTapiBridges[fileshareSettings]);
				CreateTapiBridge(fileshareSettings, token);
			}

			if (_fileTapiBridgesConnected.Count >= _exportConfig.MaxNumberOfFileExportTasks)
			{
				IDownloadTapiBridge connectedNotUsed = _fileTapiBridgesConnected.First(x => !_fileTapiBridgesInUse.Contains(x));
				_fileTapiBridgesConnected.Remove(connectedNotUsed);
				if (connectedNotUsed != _fileTapiBridges[fileshareSettings])
				{
					connectedNotUsed.Disconnect();
				}
			}

			_fileTapiBridgesConnected.Add(_fileTapiBridges[fileshareSettings]);
			_fileTapiBridgesInUse.Add(_fileTapiBridges[fileshareSettings]);

			return _fileTapiBridges[fileshareSettings];
		}

		private void CreateTapiBridge(RelativityFileShareSettings fileshareSettings, CancellationToken token)
		{
			ITapiBridgeFactory tapiBridgeFactory =
				new FilesTapiBridgeFactory(_tapiBridgeParametersFactory, _logger, fileshareSettings, token);
			var smartTapiBridge = new SmartTapiBridge(_exportConfig, tapiBridgeFactory, token);

			_fileTapiBridges[fileshareSettings] = new DownloadTapiBridgeForFiles(smartTapiBridge,
				new FileDownloadProgressHandler(_downloadProgressManager, _logger),
				_messageHandler, _filesStatistics, _transferClientHandler, _logger);
		}

		public void Release(IDownloadTapiBridge bridge)
		{
			_fileTapiBridgesInUse.Remove(bridge);
		}

		public void Dispose()
		{
			foreach (var tapiBridge in _fileTapiBridges.Values)
			{
				TryDisposeTapiBridge(tapiBridge);
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