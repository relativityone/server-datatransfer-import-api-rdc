using System.Threading;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class LongTextTapiBridgeWrapperFactory : ITapiBridgeWrapperFactory
	{
		private readonly ILog _logger;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly CancellationToken _token;

		public LongTextTapiBridgeWrapperFactory(TapiBridgeParametersFactory tapiBridgeParametersFactory, ILog logger,
			CancellationToken token)
		{
			_tapiBridgeParametersFactory = tapiBridgeParametersFactory;
			_logger = logger;
			_token = token;
		}


		public ITapiBridgeWrapper Create()
		{
			TapiBridgeParameters parameters = _tapiBridgeParametersFactory.CreateTapiBridgeParametersFromConfiguration();

			parameters.ForceAsperaClient = false;
			parameters.ForceClientCandidates = string.Empty;
			parameters.ForceFileShareClient = false;
			parameters.ForceHttpClient = true;

			DownloadTapiBridge downloadTapiBridge = TapiBridgeFactory.CreateDownloadBridge(parameters, _logger, _token);
			return new TapiBridgeWrapper(downloadTapiBridge);
		}
	}
}