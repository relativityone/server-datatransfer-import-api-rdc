using System;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public interface IDownloadTapiBridge : IDisposable
	{
		string QueueDownload(TransferPath transferPath);
		void WaitForTransferJob();
	}
}