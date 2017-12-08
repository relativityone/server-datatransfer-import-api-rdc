using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.TApi;
using Relativity.Transfer;
using ITransferStatistics = kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics.ITransferStatistics;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public abstract class DownloadTapiBridgeAdapter : IDownloadTapiBridge
	{
		private readonly IProgressHandler _progressHandler;
		private readonly IMessagesHandler _messageHandler;
		private readonly ITransferStatistics _transferStatistics;

		protected DownloadTapiBridge TapiBridge { get; }

		protected DownloadTapiBridgeAdapter(DownloadTapiBridge tapiBridge, IProgressHandler progressHandler, IMessagesHandler messageHandler, ITransferStatistics transferStatistics)
		{
			TapiBridge = tapiBridge;
			_progressHandler = progressHandler;
			_messageHandler = messageHandler;
			_transferStatistics = transferStatistics;

			_messageHandler.Attach(TapiBridge);
			_progressHandler.Attach(tapiBridge);
			_transferStatistics.Attach(tapiBridge);
		}

		public virtual void Dispose()
		{
			_progressHandler.Detach();
			_messageHandler.Detach();
			_transferStatistics.Detach();
			TapiBridge.Dispose();
		}

		public abstract string AddPath(TransferPath transferPath);
		public abstract void WaitForTransferJob();
	}
}