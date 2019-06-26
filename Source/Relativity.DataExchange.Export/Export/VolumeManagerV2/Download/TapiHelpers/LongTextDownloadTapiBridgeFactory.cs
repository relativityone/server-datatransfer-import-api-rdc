namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System.Threading;

	using kCura.WinEDDS;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public class LongTextDownloadTapiBridgeFactory : ILongTextDownloadTapiBridgeFactory
	{
		private readonly IExportConfig _exportConfig;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly LongTextEncodingConverterFactory _converterFactory;
		private readonly DownloadProgressManager _downloadProgressManager;
		private readonly IMessagesHandler _messageHandler;
		private readonly MetadataStatistics _metadataStatistics;
		private readonly ILog _logger;

		public LongTextDownloadTapiBridgeFactory(IExportConfig exportConfig, TapiBridgeParametersFactory tapiBridgeParametersFactory, LongTextEncodingConverterFactory converterFactory,
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

		public IDownloadTapiBridge Create(CancellationToken token)
		{
			ITapiBridgeFactory tapiBridgeFactory = new LongTextTapiBridgeFactory(_tapiBridgeParametersFactory, _logger, token);
			ITapiBridge tapiBridge = tapiBridgeFactory.Create();
			var smartTapiBridge = new EmptyTapiBridge(tapiBridge);

			LongTextEncodingConverter longTextEncodingConverter = _converterFactory.Create(token);
			return new DownloadTapiBridgeWithEncodingConversion(smartTapiBridge, new LongTextProgressHandler(_downloadProgressManager, _logger), _messageHandler, _metadataStatistics,
				longTextEncodingConverter, _logger);
		}
	}
}