﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;

	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	public interface IDownloadTapiBridge : IFileTransferProducer, IDisposable
	{
		TapiClient Client { get; }
		TapiBridgeParameters2 Parameters { get; }
		string QueueDownload(TransferPath transferPath);
		void WaitForTransfers();
	}
}