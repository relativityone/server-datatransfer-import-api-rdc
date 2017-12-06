using kCura.WinEDDS.TApi;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class DownloadTapiBridgeWrapper : IDownloadTapiBridge
	{
		private readonly DownloadTapiBridge _downloadTapiBridge;

		public DownloadTapiBridgeWrapper(DownloadTapiBridge downloadTapiBridge)
		{
			_downloadTapiBridge = downloadTapiBridge;
		}

		public void Dispose()
		{
			_downloadTapiBridge.Dispose();
		}

		public string AddPath(TransferPath transferPath)
		{
			return _downloadTapiBridge.AddPath(transferPath);
		}

		public void WaitForTransferJob()
		{
			_downloadTapiBridge.WaitForTransferJob();
		}
	}
}