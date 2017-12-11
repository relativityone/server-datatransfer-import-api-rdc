using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
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

		public Batch(IBatchExporter batchExporter, IBatchInitialization batchInitialization, IBatchCleanUp batchCleanUp, IBatchValidator batchValidator, IBatchState batchState)
		{
			_batchExporter = batchExporter;
			_batchInitialization = batchInitialization;
			_batchCleanUp = batchCleanUp;
			_batchValidator = batchValidator;
			_batchState = batchState;
		}

		public void Export(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			try
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_batchInitialization.PrepareBatch(artifacts, volumePredictions, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_batchExporter.Export(artifacts, volumePredictions, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_batchValidator.ValidateExportedBatch(artifacts, volumePredictions, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_batchState.SaveState();
			}
			finally
			{
				if (cancellationToken.IsCancellationRequested)
				{
					_batchState.RestoreState();
				}
				_batchCleanUp.CleanUp();
			}
		}
	}
}