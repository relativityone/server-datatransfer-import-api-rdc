using System;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public interface IDownloadTapiBridge : IDisposable
	{
		string AddPath(TransferPath transferPath);
		void WaitForTransferJob();
	}
}