namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Threading;

	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;

	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public class LongTextDownloadTapiBridgeFactory : ILongTextDownloadTapiBridgeFactory
	{
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly DownloadProgressManager _downloadProgressManager;
		private readonly IMessagesHandler _messageHandler;
		private readonly MetadataStatistics _metadataStatistics;
		private readonly ILog _logger;
		private Func<string> _getCorrelationId;

		public LongTextDownloadTapiBridgeFactory(
			TapiBridgeParametersFactory tapiBridgeParametersFactory,
			DownloadProgressManager downloadProgressManager,
			IMessagesHandler messageHandler,
			MetadataStatistics metadataStatistics,
			ILog logger,
			Func<string> getCorrelationId)
		{
			_tapiBridgeParametersFactory = tapiBridgeParametersFactory.ThrowIfNull(nameof(tapiBridgeParametersFactory));
			_downloadProgressManager = downloadProgressManager.ThrowIfNull(nameof(downloadProgressManager));
			_messageHandler = messageHandler.ThrowIfNull(nameof(messageHandler));
			_metadataStatistics = metadataStatistics.ThrowIfNull(nameof(metadataStatistics));
			_logger = logger.ThrowIfNull(nameof(logger));
			_getCorrelationId = getCorrelationId;
		}

		public IDownloadTapiBridge Create(CancellationToken token)
		{
			ITapiBridgeFactory tapiBridgeFactory = new LongTextTapiBridgeFactory(_tapiBridgeParametersFactory, _logger, token, _getCorrelationId);
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