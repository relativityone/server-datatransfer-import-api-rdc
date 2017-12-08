using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.TApi;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class DownloadTapiBridgeForFiles : DownloadTapiBridgeAdapter
	{
		private bool _isEmpty;

		private readonly ITransferClientHandler _transferClientHandler;

		public DownloadTapiBridgeForFiles(DownloadTapiBridge downloadTapiBridge, IProgressHandler progressHandler, IMessagesHandler messagesHandler,
			ITransferClientHandler transferClientHandler) : base(downloadTapiBridge, progressHandler, messagesHandler)
		{
			_transferClientHandler = transferClientHandler;
			_transferClientHandler.Attach(downloadTapiBridge);
			_isEmpty = true;
		}

		public override string AddPath(TransferPath transferPath)
		{
			_isEmpty = false;
			return TapiBridge.AddPath(transferPath);
		}

		public override void WaitForTransferJob()
		{
			if (_isEmpty)
			{
				return;
			}
			TapiBridge.WaitForTransferJob();
		}

		public override void Dispose()
		{
			_transferClientHandler.Detach();
			base.Dispose();
		}
	}
}