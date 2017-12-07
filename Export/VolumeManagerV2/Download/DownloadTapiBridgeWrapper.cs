using kCura.WinEDDS.TApi;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class DownloadTapiBridgeWrapper : IDownloadTapiBridge
	{
		private bool _isEmpty;

		private readonly DownloadTapiBridge _downloadTapiBridge;

		public DownloadTapiBridgeWrapper(DownloadTapiBridge downloadTapiBridge)
		{
			_downloadTapiBridge = downloadTapiBridge;
			_isEmpty = true;
		}

		public void Dispose()
		{
			_downloadTapiBridge.Dispose();
		}

		public string AddPath(TransferPath transferPath)
		{
			_isEmpty = false;
			return _downloadTapiBridge.AddPath(transferPath);
		}

		public void WaitForTransferJob()
		{
			if (_isEmpty)
			{
				return;
			}
			_downloadTapiBridge.WaitForTransferJob();
		}
	}
}