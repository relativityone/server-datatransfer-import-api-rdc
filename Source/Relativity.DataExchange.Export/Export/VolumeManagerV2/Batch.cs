namespace Relativity.DataExchange.Export.VolumeManagerV2
{
	using System;
	using System.Threading;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.Logging;
	using Relativity.Transfer;

	public class Batch : IBatch
	{
		private readonly IBatchExporter _batchExporter;
		private readonly IBatchInitialization _batchInitialization;
		private readonly IBatchCleanUp _batchCleanUp;
		private readonly IBatchValidator _batchValidator;
		private readonly IBatchState _batchState;
		private readonly IMessenger _messenger;
		private readonly ILog _logger;
		private int _batchNumber;

		public Batch(
			IBatchExporter batchExporter,
			IBatchInitialization batchInitialization,
			IBatchCleanUp batchCleanUp,
			IBatchValidator batchValidator,
			IBatchState batchState,
			IMessenger messenger,
			ILog logger)
		{
			_batchExporter = batchExporter.ThrowIfNull(nameof(batchExporter));
			_batchInitialization = batchInitialization.ThrowIfNull(nameof(batchInitialization));
			_batchCleanUp = batchCleanUp.ThrowIfNull(nameof(batchCleanUp));
			_batchValidator = batchValidator.ThrowIfNull(nameof(batchValidator));
			_batchState = batchState.ThrowIfNull(nameof(batchState));
			_messenger = messenger.ThrowIfNull(nameof(messenger));
			_logger = logger.ThrowIfNull(nameof(logger));
			_batchNumber = 0;
		}

		public void Export(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			try
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				int batchNumber = Interlocked.Increment(ref this._batchNumber);
				DateTime batchStartTime = DateTime.Now;
				this._logger.LogInformation("Starting export batch {BatchNumber}", batchNumber);
				_messenger.PreparingBatchForExport();

				_batchInitialization.PrepareBatch(artifacts, volumePredictions, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_messenger.DownloadingBatch();

				_batchExporter.Export(artifacts, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_messenger.ValidatingBatch();

				_batchValidator.ValidateExportedBatch(artifacts, volumePredictions, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_batchState.SaveState();
				TimeSpan elapsed = DateTime.Now - batchStartTime;
				this._logger.LogInformation(
					"Completed export batch {BatchNumber} - Elapsed: {BatchElapsedTime}",
					batchNumber,
					elapsed);
				_messenger.BatchCompleted();

			}
			catch (TransferException ex)
			{
				_logger.LogError(ex, "TransferException in TAPI occurred during batch export.");
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to export batch.");
				throw;
			}
			finally
			{
				if (cancellationToken.IsCancellationRequested)
				{
					_messenger.RestoringAfterCancel();
					_batchState.RestoreState();
					_messenger.StateRestored();
				}

				_batchCleanUp.CleanUp();
			}
		}
	}
}