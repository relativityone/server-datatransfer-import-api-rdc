using System;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.EncodingHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class LongTextTapiBridgePool : ILongTextTapiBridgePool
	{
		private IDownloadTapiBridge _longTextTapiBridge;

		private readonly IExportConfig _exportConfig;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly LongTextEncodingConverterFactory _converterFactory;
		private readonly DownloadProgressManager _downloadProgressManager;
		private readonly IMessagesHandler _messageHandler;
		private readonly MetadataStatistics _metadataStatistics;
		private readonly ILog _logger;

		public LongTextTapiBridgePool(IExportConfig exportConfig, TapiBridgeParametersFactory tapiBridgeParametersFactory, LongTextEncodingConverterFactory converterFactory,
			DownloadProgressManager downloadProgressManager, IMessagesHandler messageHandler, MetadataStatistics metadataStatistics, ILog logger)
		{
			_exportConfig = exportConfig;
			_tapiBridgeParametersFactory = tapiBridgeParametersFactory;
			_converterFactory = converterFactory;
			_downloadProgressManager = downloadProgressManager;
			_messageHandler = messageHandler;
			_metadataStatistics = metadataStatistics;
			_logger = logger;
		}

		public IDownloadTapiBridge Request(CancellationToken token)
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

		public void Release(IDownloadTapiBridge bridge)
		{
			//Do nothing as long text tapi bridge is going through web mode
		}

		public void Dispose()
		{
			try
			{
				_longTextTapiBridge?.Dispose();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to dispose Tapi Bridge.");
				// We do not want to crash container disposal
			}
		}
	}
}