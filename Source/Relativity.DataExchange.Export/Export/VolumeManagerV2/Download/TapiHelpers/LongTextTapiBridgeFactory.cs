using System.Threading;

using Relativity.DataExchange.Transfer;
using Relativity.Logging;

namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class LongTextTapiBridgeFactory : ITapiBridgeFactory
	{
		private readonly ILog _logger;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly IAppSettings _settings;
		private readonly CancellationToken _token;

		public LongTextTapiBridgeFactory(
			TapiBridgeParametersFactory tapiBridgeParametersFactory,
			ILog logger,
			CancellationToken token)
			: this(tapiBridgeParametersFactory, logger, AppSettings.Instance, token)
		{
		}

		public LongTextTapiBridgeFactory(
			TapiBridgeParametersFactory tapiBridgeParametersFactory,
			ILog logger,
			IAppSettings settings,
			CancellationToken token)
		{
			_tapiBridgeParametersFactory = tapiBridgeParametersFactory.ThrowIfNull(nameof(tapiBridgeParametersFactory));
			_logger = logger.ThrowIfNull(nameof(logger));
			_settings = settings.ThrowIfNull(nameof(settings));
			_token = token;
		}


		public ITapiBridge Create()
		{
			DownloadTapiBridgeParameters2 parameters = _tapiBridgeParametersFactory.CreateTapiBridgeParametersFromConfiguration();
			parameters.ForceAsperaClient = false;
			parameters.ForceClientCandidates = string.Empty;
			parameters.ForceFileShareClient = false;
			parameters.ForceHttpClient = true;

			// REL-345129: For large extracted text files, override with a more specialized timeout.
			parameters.TimeoutSeconds = _settings.HttpExtractedTextTimeoutSeconds;

			DownloadTapiBridge2 downloadTapiBridge = TapiBridgeFactory.CreateDownloadBridge(parameters, _logger, _token);
			return downloadTapiBridge;
		}
	}
}