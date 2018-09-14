using System;
using kCura.WinEDDS.TApi;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public interface IDownloadTapiBridge : IDisposable
	{
		TapiClient ClientType { get; }
		string QueueDownload(TransferPath transferPath);
		void WaitForTransferJob();
	}
}