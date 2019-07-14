namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System.Linq;
	using System.Threading;

	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public class FilesTapiBridgeFactory : ITapiBridgeFactory
	{
		private readonly IRelativityFileShareSettings _fileshareSettings;
		private readonly ILog _logger;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly ITapiObjectService _tapiObjectService;
		private readonly CancellationToken _token;

		public FilesTapiBridgeFactory(
			TapiBridgeParametersFactory factory,
			ITapiObjectService tapiObjectService,
			ILog logger,
			IRelativityFileShareSettings settings,
			CancellationToken token)
		{
			// Note: the setting can be null (see below).
			_tapiBridgeParametersFactory = factory.ThrowIfNull(nameof(factory));
			_tapiObjectService = tapiObjectService.ThrowIfNull(nameof(tapiObjectService));
			_logger = logger.ThrowIfNull(nameof(logger));
			_fileshareSettings = settings;
			_token = token;
		}

		public ITapiBridge Create()
		{
			TapiBridgeParameters2 parameters = _tapiBridgeParametersFactory.CreateTapiBridgeParametersFromConfiguration();
			if (_fileshareSettings == null)
			{
				string candidates = _tapiObjectService.GetUnmappedFileRepositoryClients();
				_logger.LogWarning(
					"Forcing the client candidates using {ForceClientCandidates} because the file share settings couldn't be determined.",
					candidates);
				parameters.ForceClientCandidates = candidates;
			}
			else
			{
				parameters.FileShare = _fileshareSettings.UncPath;
				parameters.TransferCredential = _fileshareSettings.TransferCredential;
			}

			DownloadTapiBridge2 tapiBridge = TapiBridgeFactory.CreateDownloadBridge(parameters, _logger, _token);
			tapiBridge.LogTransferParameters();
			return tapiBridge;
		}
	}
}