using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.EncodingHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class ExportTapiBridgePool : IExportTapiBridgePool
	{
		private IDownloadTapiBridge _longTextTapiBridge;

		private readonly IDictionary<RelativityFileShareSettings, IDownloadTapiBridge> _fileTapiBridges = new Dictionary<RelativityFileShareSettings, IDownloadTapiBridge>();

		private readonly DownloadProgressManager _downloadProgressManager;
		private readonly FilesStatistics _filesStatistics;
		private readonly ILog _logger;
		private readonly IMessagesHandler _messageHandler;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly ITransferClientHandler _transferClientHandler;
		private readonly LongTextEncodingConverterFactory _converterFactory;
		private readonly MetadataStatistics _metadataStatistics;
		
		public ExportTapiBridgePool(DownloadProgressManager downloadProgressManager, ILog logger,
			IMessagesHandler messageHandler, FilesStatistics filesStatistics,
			ITransferClientHandler transferClientHandler, TapiBridgeParametersFactory tapiBridgeParametersFactory, LongTextEncodingConverterFactory converterFactory, MetadataStatistics metadataStatistics)
		{
			_downloadProgressManager = downloadProgressManager;
			_logger = logger;
			_messageHandler = messageHandler;
			_filesStatistics = filesStatistics;
			_transferClientHandler = transferClientHandler;
			_tapiBridgeParametersFactory = tapiBridgeParametersFactory;
			_converterFactory = converterFactory;
			_metadataStatistics = metadataStatistics;
		}

		public IDownloadTapiBridge CreateForFiles(RelativityFileShareSettings fileshareSettings,
			CancellationToken token)
		{
			if (_fileTapiBridges.ContainsKey(fileshareSettings))
			{
				return _fileTapiBridges[fileshareSettings];
			}
			ITapiBridgeFactory tapiBridgeFactory = new FilesTapiBridgeFactory(_tapiBridgeParametersFactory, _logger, fileshareSettings, token);
			var smartTapiBridge = new SmartTapiBridge(tapiBridgeFactory);

			_fileTapiBridges[fileshareSettings] = new DownloadTapiBridgeForFiles(smartTapiBridge,
				new FileDownloadProgressHandler(_downloadProgressManager, _logger),
				_messageHandler, _filesStatistics, _transferClientHandler, _logger);
			return _fileTapiBridges[fileshareSettings];
		}

		public IDownloadTapiBridge CreateForLongText(CancellationToken token)
		{
			if (_longTextTapiBridge != null)
			{
				return _longTextTapiBridge;
			}
			ITapiBridgeFactory tapiBridgeFactory = new LongTextTapiBridgeFactory(_tapiBridgeParametersFactory, _logger, token);
			var smartTapiBridge = new SmartTapiBridge(tapiBridgeFactory);

			LongTextEncodingConverter longTextEncodingConverter = _converterFactory.Create(token);
			_longTextTapiBridge = new DownloadTapiBridgeWithEncodingConversion(smartTapiBridge, new LongTextProgressHandler(_downloadProgressManager, _logger), _messageHandler, _metadataStatistics,
				longTextEncodingConverter, _logger);
			return _longTextTapiBridge;
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
			catch
			{
				// We do not want to crash container disposal
			}
		}
	}
}