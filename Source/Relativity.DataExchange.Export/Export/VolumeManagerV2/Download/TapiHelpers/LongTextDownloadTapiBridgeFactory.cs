namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System.Threading;

	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public class LongTextDownloadTapiBridgeFactory : ILongTextDownloadTapiBridgeFactory
	{
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly LongTextEncodingConverterFactory _converterFactory;
		private readonly DownloadProgressManager _downloadProgressManager;
		private readonly IMessagesHandler _messageHandler;
		private readonly MetadataStatistics _metadataStatistics;
		private readonly ILog _logger;

		public LongTextDownloadTapiBridgeFactory(
			TapiBridgeParametersFactory tapiBridgeParametersFactory,
			LongTextEncodingConverterFactory converterFactory,
			DownloadProgressManager downloadProgressManager,
			IMessagesHandler messageHandler,
			MetadataStatistics metadataStatistics,
			ILog logger)
		{
			_tapiBridgeParametersFactory = tapiBridgeParametersFactory.ThrowIfNull(nameof(tapiBridgeParametersFactory));
			_converterFactory = converterFactory.ThrowIfNull(nameof(converterFactory));
			_downloadProgressManager = downloadProgressManager.ThrowIfNull(nameof(downloadProgressManager));
			_messageHandler = messageHandler.ThrowIfNull(nameof(messageHandler));
			_metadataStatistics = metadataStatistics.ThrowIfNull(nameof(metadataStatistics));
			_logger = logger.ThrowIfNull(nameof(logger));
		}

		public IDownloadTapiBridge Create(CancellationToken token)
		{
			ITapiBridgeFactory tapiBridgeFactory = new LongTextTapiBridgeFactory(_tapiBridgeParametersFactory, _logger, token);
			ITapiBridge tapiBridge = tapiBridgeFactory.Create();
			return new DownloadTapiBridgeWithEncodingConversion(
				tapiBridge,
				new LongTextProgressHandler(_downloadProgressManager, _logger),
				_messageHandler,
				_metadataStatistics,
				_logger);
		}
	}
}