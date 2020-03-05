﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Reactive.Concurrency;
	using System.Reactive.Linq;
	using System.Reactive.Subjects;

	using Relativity.DataExchange.Transfer;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Logger;
	using Relativity.Logging;
	using Relativity.Transfer;

	using ITransferStatistics = Relativity.DataExchange.Export.VolumeManagerV2.Statistics.ITransferStatistics;

	public abstract class DownloadTapiBridgeAdapter : IDownloadTapiBridge
	{
		private readonly IProgressHandler _progressHandler;
		private readonly IMessagesHandler _messageHandler;
		private readonly ITransferStatistics _transferStatistics;

		protected  readonly ILog _logger;

		protected DownloadTapiBridgeAdapter(
			ITapiBridge bridge,
			IProgressHandler progressHandler,
			IMessagesHandler messageHandler,
			ITransferStatistics transferStatistics,
			ILog logger)
		{
			this.TapiBridge = bridge.ThrowIfNull(nameof(bridge));
			this._progressHandler = progressHandler.ThrowIfNull(nameof(progressHandler));
			this._messageHandler = messageHandler.ThrowIfNull(nameof(messageHandler));
			this._transferStatistics = transferStatistics.ThrowIfNull(nameof(transferStatistics));
			this._logger = logger.ThrowIfNull(nameof(logger));

			_messageHandler.Subscribe(this.TapiBridge);
			_progressHandler.Subscribe(this.TapiBridge);
			_transferStatistics.Subscribe(this.TapiBridge);
		}

		public TapiClient Client => this.TapiBridge.Client;

		public TapiBridgeParameters2 Parameters => this.TapiBridge.Parameters;

		protected ITapiBridge TapiBridge { get; }

		public virtual void Dispose()
		{
			_progressHandler.Unsubscribe(this.TapiBridge);
			_messageHandler.Unsubscribe(this.TapiBridge);
			_transferStatistics.Unsubscribe(this.TapiBridge);
			this.TapiBridge.Dispose();
		}

		public IObservable<string> FileDownloaded
		{
			get
			{
				return Observable.FromEventPattern<TapiProgressEventArgs>(h =>
							{
								this.TapiBridge.TapiProgress += h;
								this._logger.LogVerbose(
									"Attached tapi bridge {TapiBridgeInstanceId} to the events observer.",
									TapiBridge.InstanceId);
							},
						h =>
							{
								this.TapiBridge.TapiProgress -= h;
								this._logger.LogVerbose(
									"Detached tapi bridge {TapiBridgeInstanceId} from the events observer.",
									this.TapiBridge.InstanceId);
							})
					// it will run on the Thread Pool 
					.ObserveOn(Scheduler.Default)
					.Select(x => x.EventArgs)
					.Where(this.IsTransferSuccessful)
					.Select(x => x.FileName);
			}
		}

		public abstract string QueueDownload(TransferPath transferPath);

		public abstract void WaitForTransfers();

		private bool IsTransferSuccessful(TapiProgressEventArgs arg)
		{
			_logger.LogVerbose(
				"Long text encoding conversion progress event for file {FileName} with status {Successful}.",
				arg.FileName.Secure(),
				arg.Successful);
			return arg.Successful;
		}
	}
}