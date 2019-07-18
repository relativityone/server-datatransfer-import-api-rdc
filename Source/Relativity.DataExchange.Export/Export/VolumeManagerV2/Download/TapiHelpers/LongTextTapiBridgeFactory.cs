using System.Threading;

using Relativity.Logging;

namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using Relativity.DataExchange.Transfer;

	public class LongTextTapiBridgeFactory : ITapiBridgeFactory
	{
		private readonly ILog _logger;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly CancellationToken _token;

		public LongTextTapiBridgeFactory(TapiBridgeParametersFactory tapiBridgeParametersFactory, ILog logger,
			CancellationToken token)
		{
			_tapiBridgeParametersFactory = tapiBridgeParametersFactory;
			_logger = logger;
			_token = token;
		}


		public ITapiBridge Create()
		{
			TapiBridgeParameters2 parameters = _tapiBridgeParametersFactory.CreateTapiBridgeParametersFromConfiguration();

			parameters.ForceAsperaClient = false;
			parameters.ForceClientCandidates = string.Empty;
			parameters.ForceFileShareClient = false;
			parameters.ForceHttpClient = true;

			DownloadTapiBridge2 downloadTapiBridge = TapiBridgeFactory.CreateDownloadBridge(parameters, _logger, _token);
			return downloadTapiBridge;
		}
	}
}