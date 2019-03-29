﻿using System.Threading;
using Relativity.Import.Export.Transfer;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
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
			TapiBridgeParameters parameters = _tapiBridgeParametersFactory.CreateTapiBridgeParametersFromConfiguration();

			parameters.FileshareCredentials = _fileshareSettings?.TransferCredential;
			parameters.FileShare = _fileshareSettings?.UncPath;

			DownloadTapiBridge tapiBridge = TapiBridgeFactory.CreateDownloadBridge(parameters, _logger, _token);
			tapiBridge.DumpInfo();
			return new TapiBridgeWrapper(tapiBridge);
		}
	}
}