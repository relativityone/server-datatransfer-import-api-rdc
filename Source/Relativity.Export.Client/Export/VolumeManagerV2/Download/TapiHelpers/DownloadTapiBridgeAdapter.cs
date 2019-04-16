﻿namespace Relativity.Export.VolumeManagerV2.Download.TapiHelpers
{
	using Relativity.Export.VolumeManagerV2.Statistics;
	using Relativity.Import.Export.Transfer;
	using Relativity.Transfer;

	using ITransferStatistics = Relativity.Export.VolumeManagerV2.Statistics.ITransferStatistics;

	public abstract class DownloadTapiBridgeAdapter : IDownloadTapiBridge
	{
		private readonly IProgressHandler _progressHandler;
		private readonly IMessagesHandler _messageHandler;
		private readonly ITransferStatistics _transferStatistics;

		protected ITapiBridge TapiBridge { get; }

		protected DownloadTapiBridgeAdapter(ITapiBridge tapiBridge, IProgressHandler progressHandler, IMessagesHandler messageHandler, ITransferStatistics transferStatistics)
		{
			TapiBridge = tapiBridge;
			_progressHandler = progressHandler;
			_messageHandler = messageHandler;
			_transferStatistics = transferStatistics;

			_messageHandler.Attach(TapiBridge);
			_progressHandler.Attach(TapiBridge);
			_transferStatistics.Attach(TapiBridge);
		}

		public virtual void Dispose()
		{
			_progressHandler.Detach();
			_messageHandler.Detach();
			_transferStatistics.Detach();
			TapiBridge.Dispose();
		}

		public TapiClient ClientType => TapiBridge.ClientType;
		public abstract string QueueDownload(TransferPath transferPath);
		public abstract void WaitForTransferJob();
		public void Disconnect()
		{
			TapiBridge.Disconnect();
		}
	}
}