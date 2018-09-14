using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.EncodingHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class ExportTapiBridgePool : IExportTapiBridgePool
	{
		private IDownloadTapiBridge _longTextTapiBridge;

		private readonly IDictionary<RelativityFileShareSettings, IDownloadTapiBridge> _fileTapiBridges = new Dictionary<RelativityFileShareSettings, IDownloadTapiBridge>();

		private readonly IList<IDownloadTapiBridge> _fileTapiBridgesInUse = new List<IDownloadTapiBridge>();
		private readonly IList<IDownloadTapiBridge> _fileTapiBridgesConnected = new List<IDownloadTapiBridge>();

		private readonly DownloadProgressManager _downloadProgressManager;
		private readonly FilesStatistics _filesStatistics;
		private readonly ILog _logger;
		private readonly IMessagesHandler _messageHandler;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly ITransferClientHandler _transferClientHandler;
		private readonly LongTextEncodingConverterFactory _converterFactory;
		private readonly MetadataStatistics _metadataStatistics;
		private readonly IExportConfig _exportConfig;

		public ExportTapiBridgePool(DownloadProgressManager downloadProgressManager, ILog logger,
			IMessagesHandler messageHandler, FilesStatistics filesStatistics,
			ITransferClientHandler transferClientHandler, TapiBridgeParametersFactory tapiBridgeParametersFactory, 
			LongTextEncodingConverterFactory converterFactory, MetadataStatistics metadataStatistics,
			IExportConfig exportConfig)
		{
			_downloadProgressManager = downloadProgressManager;
			_logger = logger;
			_messageHandler = messageHandler;
			_filesStatistics = filesStatistics;
			_transferClientHandler = transferClientHandler;
			_tapiBridgeParametersFactory = tapiBridgeParametersFactory;
			_converterFactory = converterFactory;
			_metadataStatistics = metadataStatistics;
			_exportConfig = exportConfig;
		}

		public IDownloadTapiBridge RequestForFiles(RelativityFileShareSettings fileshareSettings,
			CancellationToken token)
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

		public IDownloadTapiBridge RequestForLongText(CancellationToken token)
		{
			if (_longTextTapiBridge != null)
			{
				return _longTextTapiBridge;
			}
			ITapiBridgeFactory tapiBridgeFactory = new LongTextTapiBridgeFactory(_tapiBridgeParametersFactory, _logger, token);
			var smartTapiBridge = new SmartTapiBridge(_exportConfig, tapiBridgeFactory, token);

			LongTextEncodingConverter longTextEncodingConverter = _converterFactory.Create(token);
			_longTextTapiBridge = new DownloadTapiBridgeWithEncodingConversion(smartTapiBridge, new LongTextProgressHandler(_downloadProgressManager, _logger), _messageHandler, _metadataStatistics,
				longTextEncodingConverter, _logger);
			return _longTextTapiBridge;
		}

		public void ReleaseFiles(IDownloadTapiBridge tapiBridge)
		{
			_fileTapiBridgesInUse.Remove(tapiBridge);
		}

		public void ReleaseLongText(IDownloadTapiBridge tapiBridge)
		{
			//Do nothing as long text tapi bridge is going through web mode
		}


		public void Dispose()
		{
			TryDisposeTapiBridge(_longTextTapiBridge);
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