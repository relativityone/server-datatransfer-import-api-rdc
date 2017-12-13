using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchValidator : IBatchValidator
	{
		private readonly IList<IBatchValidator> _validators;
		private readonly ILog _logger;

		public BatchValidator(ILog logger)
		{
			_logger = logger;
			_validators = new List<IBatchValidator>();
		}

		public void AddBatchValidator(IBatchValidator validator)
		{
			_validators.Add(validator);
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Verifying batch correctness.");
			try
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
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred during checking batch correctness.");
				throw;
			}
			_logger.LogVerbose("Batch verified.");
		}
	}
}