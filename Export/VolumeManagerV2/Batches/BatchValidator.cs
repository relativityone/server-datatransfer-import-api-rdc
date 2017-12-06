using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchValidator : IBatchValidator
	{
		private readonly IList<IBatchValidator> _validators;

		public BatchValidator()
		{
			_validators = new List<IBatchValidator>();
		}

		public void AddBatchValidator(IBatchValidator validator)
		{
			_validators.Add(validator);
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions, CancellationToken cancellationToken)
		{
			foreach (var batchValidator in _validators)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
				batchValidator.ValidateExportedBatch(artifacts, predictions, cancellationToken);
			}
		}
	}
}