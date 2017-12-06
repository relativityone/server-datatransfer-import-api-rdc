using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchInitialization : IBatchInitialization
	{
		private readonly LongTextRepositoryBuilder _longTextRepositoryBuilder;
		private readonly ILog _logger;

		public BatchInitialization(LongTextRepositoryBuilder longTextRepositoryBuilder, ILog logger)
		{
			_longTextRepositoryBuilder = longTextRepositoryBuilder;
			_logger = logger;
		}

		public void PrepareBatch(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			FillLongTextRepository(artifacts, cancellationToken);
		}

		private void FillLongTextRepository(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Building LongTextRepository for current batch.");
			_longTextRepositoryBuilder.AddLongTextForBatchToRepository(artifacts, cancellationToken);
		}
	}
}