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

		public BatchValidator(IList<IBatchValidator> validators, ILog logger)
		{
			_validators = validators;
			_logger = logger;
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