using System.Threading;

using Relativity.DataExchange.Transfer;
using Relativity.Logging;

namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;

	using kCura.WinEDDS.Service.Kepler;

	using Relativity.DataExchange.Service;

	public class LongTextTapiBridgeFactory : ITapiBridgeFactory
	{
		private readonly ILog _logger;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly IAppSettings _settings;
		private readonly CancellationToken _token;

		private Func<string> _getCorrelationId;

		public LongTextTapiBridgeFactory(
			TapiBridgeParametersFactory tapiBridgeParametersFactory,
			ILog logger,
			CancellationToken token,
			Func<string> getCorrelationId)
			: this(tapiBridgeParametersFactory, logger, AppSettings.Instance, token)
		{
			_getCorrelationId = getCorrelationId;
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
			TapiBridgeParameters2 parameters = _tapiBridgeParametersFactory.CreateTapiBridgeParametersFromConfiguration();
			parameters.ForceAsperaClient = false;
			parameters.ForceClientCandidates = string.Empty;
			parameters.ForceFileShareClient = false;
			parameters.ForceHttpClient = true;

			// REL-345129: For large extracted text files, override with a more specialized timeout.
			parameters.TimeoutSeconds = _settings.HttpExtractedTextTimeoutSeconds;

			DownloadTapiBridge2 downloadTapiBridge = TapiBridgeFactory.CreateDownloadBridge(
				parameters,
				this._logger,
				this._token,
				this._getCorrelationId,
				new WebApiVsKeplerFactory(this._logger),
				new RelativityManagerServiceFactory());
			return downloadTapiBridge;
		}
	}
}