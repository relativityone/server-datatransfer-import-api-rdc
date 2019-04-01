namespace Relativity.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System.Threading;

	using kCura.WinEDDS;
	using Relativity.Export.VolumeManagerV2.Statistics;

	using Relativity.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.Logging;

	public class LongTextTapiBridgeFactory : ILongTextTapiBridgeFactory
	{
		private readonly IExportConfig _exportConfig;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly LongTextEncodingConverterFactory _converterFactory;
		private readonly DownloadProgressManager _downloadProgressManager;
		private readonly IMessagesHandler _messageHandler;
		private readonly MetadataStatistics _metadataStatistics;
		private readonly ILog _logger;

		public LongTextTapiBridgeFactory(IExportConfig exportConfig, TapiBridgeParametersFactory tapiBridgeParametersFactory, LongTextEncodingConverterFactory converterFactory,
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
			ITapiBridgeWrapperFactory tapiBridgeWrapperFactory = new LongTextTapiBridgeWrapperFactory(_tapiBridgeParametersFactory, _logger, token);
			ITapiBridgeWrapper tapiBridgeWrapper = tapiBridgeWrapperFactory.Create();
			var smartTapiBridge = new EmptyTapiBridge(tapiBridgeWrapper);

			LongTextEncodingConverter longTextEncodingConverter = _converterFactory.Create(token);
			return new DownloadTapiBridgeWithEncodingConversion(smartTapiBridge, new LongTextProgressHandler(_downloadProgressManager, _logger), _messageHandler, _metadataStatistics,
				longTextEncodingConverter, _logger);
		}
	}
}