namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Threading;

	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public class FilesTapiBridgeFactory : ITapiBridgeFactory
	{
		private readonly IRelativityFileShareSettings _fileshareSettings;
		private readonly ILog _logger;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly CancellationToken _token;

		public FilesTapiBridgeFactory(
			TapiBridgeParametersFactory factory,
			ILog logger,
			IRelativityFileShareSettings settings,
			CancellationToken token)
		{
			factory.ThrowIfNull(nameof(factory));
			logger.ThrowIfNull(nameof(logger));
			settings.ThrowIfNull(nameof(settings));
			_tapiBridgeParametersFactory = factory;
			_logger = logger;
			_fileshareSettings = settings;
			_token = token;
		}

		public ITapiBridge Create()
		{
			TapiBridgeParameters2 parameters = _tapiBridgeParametersFactory.CreateTapiBridgeParametersFromConfiguration();
			parameters.FileshareCredentials = _fileshareSettings?.TransferCredential;
			parameters.FileShare = _fileshareSettings?.UncPath;

			DownloadTapiBridge2 tapiBridge = TapiBridgeFactory.CreateDownloadBridge(parameters, _logger, _token);
			tapiBridge.LogTransferParameters();
			return tapiBridge;
		}
	}
}