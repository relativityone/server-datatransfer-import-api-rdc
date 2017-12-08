using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.TApi;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class DownloadTapiBridgeForFiles : DownloadTapiBridgeAdapter
	{
		private bool _isEmpty;

		private readonly ITransferClientHandler _transferClientHandler;
		private readonly ILog _logger;

		public DownloadTapiBridgeForFiles(DownloadTapiBridge downloadTapiBridge, IProgressHandler progressHandler, IMessagesHandler messagesHandler,
			ITransferClientHandler transferClientHandler, ILog logger) : base(downloadTapiBridge, progressHandler, messagesHandler)
		{
			_transferClientHandler = transferClientHandler;
			_logger = logger;
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
				_logger.LogVerbose("Files transfer bridge is empty, so skipping waiting.");
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