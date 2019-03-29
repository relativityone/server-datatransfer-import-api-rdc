using System;
using Relativity.Import.Export.Transfer;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public interface IDownloadTapiBridge : IDisposable
	{
		TapiClient ClientType { get; }
		string QueueDownload(TransferPath transferPath);
		void WaitForTransferJob();
		void Disconnect();
	}
}