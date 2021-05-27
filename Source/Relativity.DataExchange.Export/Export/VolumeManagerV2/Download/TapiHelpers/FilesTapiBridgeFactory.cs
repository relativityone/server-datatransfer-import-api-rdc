namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
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
			DownloadTapiBridgeParameters2 parameters = _tapiBridgeParametersFactory.CreateTapiBridgeParametersFromConfiguration();
			if (_fileshareSettings == null)
			{
				_tapiObjectService.ApplyUnmappedFileRepositoryParameters(parameters);
				_logger.LogWarning(
					"Applying Transfer API bridge parameter changes because the file share settings are unmapped. ForceClientCandidates={ForceClientCandidates}, ForceAsperaClient={ForceAsperaClient}, ForceFileShareClient={ForceFileShareClient}, ForceHttpClient={ForceHttpClient}",
					parameters.ForceClientCandidates,
					parameters.ForceAsperaClient,
					parameters.ForceFileShareClient,
					parameters.ForceHttpClient);
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