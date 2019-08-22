namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Reactive.Concurrency;
	using System.Reactive.Linq;
	using System.Reactive.Subjects;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;
	using Relativity.Transfer;

	using ITransferStatistics = Relativity.DataExchange.Export.VolumeManagerV2.Statistics.ITransferStatistics;

	public interface IFileTransferProducer
	{
		IObservable<string> FileDownloaded { get; }

		Subject<int> FileDownloadCompleted { get; }
	}

	public class DownloadTapiBridgeWithEncodingConversion : DownloadTapiBridgeAdapter, IFileTransferProducer
	{
		private readonly object _syncRoot = new object();
		private readonly IFileDownloadSubscriber _fileDownloadSubscriber;
		private readonly ILog _logger;
		private bool _initialized;

		private int _fileDownloadCount;

		public DownloadTapiBridgeWithEncodingConversion(
			ITapiBridge downloadTapiBridge,
			IProgressHandler progressHandler,
			IMessagesHandler messagesHandler,
			ITransferStatistics transferStatistics,
			IFileDownloadSubscriber fileDownloadSubscriber,
			ILog logger)
			: base(downloadTapiBridge, progressHandler, messagesHandler, transferStatistics)
		{
			this._fileDownloadSubscriber = fileDownloadSubscriber;
			_logger = logger;
			_initialized = false;

			this.FileDownloadCompleted = new Subject<int>();
		}

		public Subject<int> FileDownloadCompleted { get; private set; }

		public IObservable<string> FileDownloaded
		{
			get
			{
				return Observable.FromEventPattern<TapiProgressEventArgs>(h =>
					{
						this._logger.LogVerbose(
							"Attached tapi bridge {TapiBridgeInstanceId} to the long text encoding converter.",
							TapiBridge.InstanceId);
						this.TapiBridge.TapiProgress += h;
					},
		h =>
					{
						this._logger.LogVerbose(
							"Detached tapi bridge {TapiBridgeInstanceId} from the long text encoding converter.",
							this.TapiBridge.InstanceId);
						this.TapiBridge.TapiProgress -= h;
					})
					// it will run on the Thread Pool 
					.ObserveOn(Scheduler.Default)
					.Select(x => x.EventArgs)
					.Where(this.IsTransferSuccessful)
					.Select(x => x.FileName);
			}
		}

		public override string QueueDownload(TransferPath transferPath)
		{
			lock (_syncRoot)
			{
				if (!_initialized)
				{
					_logger.LogVerbose("Initializing long text encoding converter...");
					this._fileDownloadSubscriber.SubscribeForDownloadEvents(this);
					
					_logger.LogVerbose("Initialized long text encoding converter.");
					_initialized = true;
				}
			}
			return this.TapiBridge.AddPath(transferPath);
		}

		public override void WaitForTransfers()
		{
			lock (_syncRoot)
			{
				if (!_initialized)
				{
					_logger.LogVerbose(
						"Long text encoding conversion bridge hasn't been initialized, so skipping waiting.");
					return;
				}
			}
			try
			{
				const bool KeepJobAlive = false;
				this.TapiBridge.WaitForTransfers(
					"Waiting for all long files to download...",
					"Long file downloads completed.",
					"Failed to wait for all pending long file downloads.",
					KeepJobAlive);
			}
			finally
			{
				try
				{
					this.FileDownloadCompleted.OnNext(this._fileDownloadCount);
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Error occurred when trying to stop LongText encoding conversion after TAPI client failure.");
				}
			}

			this._fileDownloadSubscriber.WaitForConversionCompletion().GetAwaiter().GetResult();
		}

		private bool IsTransferSuccessful(TapiProgressEventArgs arg)
		{
			_logger.LogVerbose(
				"Long text encoding conversion progress event for file {FileName} with status {Successful}.",
				arg.FileName,
				arg.Successful);
			return arg.Successful;
		}
	}
}