﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System.Threading;

	using Relativity.DataExchange.Transfer;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;

	public class FilesTapiBridgeWrapperFactory : ITapiBridgeWrapperFactory
	{
		private readonly IRelativityFileShareSettings _fileshareSettings;
		private readonly ILog _logger;
		private readonly TapiBridgeParametersFactory _tapiBridgeParametersFactory;
		private readonly CancellationToken _token;

		public FilesTapiBridgeWrapperFactory(TapiBridgeParametersFactory tapiBridgeParametersFactory, ILog logger,
			IRelativityFileShareSettings fileshareSettings, CancellationToken token)
		{
			_tapiBridgeParametersFactory = tapiBridgeParametersFactory;
			_logger = logger;
			_fileshareSettings = fileshareSettings;
			_token = token;
		}

		public ITapiBridgeWrapper Create()
		{
			TapiBridgeParameters2 parameters = _tapiBridgeParametersFactory.CreateTapiBridgeParametersFromConfiguration();

			parameters.FileshareCredentials = _fileshareSettings?.TransferCredential;
			parameters.FileShare = _fileshareSettings?.UncPath;

			DownloadTapiBridge2 tapiBridge = TapiBridgeFactory.CreateDownloadBridge(parameters, _logger, _token);
			tapiBridge.DumpInfo();
			return new TapiBridgeWrapper(tapiBridge);
		}
	}
}