using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.TApi;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public abstract class DownloadTapiBridgeAdapter : IDownloadTapiBridge
	{
		protected DownloadTapiBridge TapiBridge { get; }
		protected IProgressHandler ProgressHandler { get; }
		protected IMessagesHandler MessageHandler { get; }

		protected DownloadTapiBridgeAdapter(DownloadTapiBridge tapiBridge, IProgressHandler progressHandler, IMessagesHandler messageHandler)
		{
			TapiBridge = tapiBridge;
			ProgressHandler = progressHandler;
			MessageHandler = messageHandler;

			MessageHandler.Attach(TapiBridge);
			ProgressHandler.Attach(tapiBridge);
		}

		public virtual void Dispose()
		{
			ProgressHandler.Detach();
			MessageHandler.Detach();
			TapiBridge.Dispose();
		}

		public abstract string AddPath(TransferPath transferPath);
		public abstract void WaitForTransferJob();
	}
}