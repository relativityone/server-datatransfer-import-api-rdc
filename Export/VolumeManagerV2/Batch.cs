using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class Batch : IBatch
	{
		private readonly IBatchExporter _batchExporter;
		private readonly IBatchInitialization _batchInitialization;
		private readonly IBatchCleanUp _batchCleanUp;
		private readonly IBatchValidator _batchValidator;
		private readonly IBatchState _batchState;
		private readonly IMessenger _messenger;

		public Batch(IBatchExporter batchExporter, IBatchInitialization batchInitialization, IBatchCleanUp batchCleanUp, IBatchValidator batchValidator, IBatchState batchState, IMessenger messenger)
		{
			_batchExporter = batchExporter;
			_batchInitialization = batchInitialization;
			_batchCleanUp = batchCleanUp;
			_batchValidator = batchValidator;
			_batchState = batchState;
			_messenger = messenger;
		}

		public void Export(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			try
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_messenger.PreparingBatchForExport();

				_batchInitialization.PrepareBatch(artifacts, volumePredictions, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_messenger.DownloadingBatch();

				_batchExporter.Export(artifacts, volumePredictions, cancellationToken);

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

				_messenger.BatchCompleted();
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