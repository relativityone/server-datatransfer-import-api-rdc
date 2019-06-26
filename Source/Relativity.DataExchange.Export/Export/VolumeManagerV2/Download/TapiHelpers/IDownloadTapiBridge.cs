namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;

	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	public interface IDownloadTapiBridge : IDisposable
	{
		TapiClient Client { get; }
		string QueueDownload(TransferPath transferPath);
		void WaitForTransfers();
		void Disconnect();
	}
}