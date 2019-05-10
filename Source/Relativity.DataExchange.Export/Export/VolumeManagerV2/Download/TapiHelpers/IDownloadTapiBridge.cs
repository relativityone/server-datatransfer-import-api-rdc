namespace Relativity.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;

	using Relativity.Import.Export.Transfer;
	using Relativity.Transfer;

	public interface IDownloadTapiBridge : IDisposable
	{
		TapiClient ClientType { get; }
		string QueueDownload(TransferPath transferPath);
		void WaitForTransferJob();
		void Disconnect();
	}
}