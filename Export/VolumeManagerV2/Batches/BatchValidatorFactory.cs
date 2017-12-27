using System.Collections.Generic;
using Castle.Windsor;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchValidatorFactory
	{
		private readonly ILog _logger;

		public BatchValidatorFactory(ILog logger)
		{
			_logger = logger;
		}

		public IBatchValidator Create(ExportFile exportSettings, IWindsorContainer container)
		{
			_logger.LogVerbose("Creating BatchValidator.");
			List<IBatchValidator> batchValidators = new List<IBatchValidator>();

			if (exportSettings.ExportFullTextAsFile)
			{
				_logger.LogVerbose("Adding {validator}.", nameof(LongTextBatchValidator));
				batchValidators.Add(container.Resolve<LongTextBatchValidator>());
			}

			if (exportSettings.ExportNative && exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
			{
				_logger.LogVerbose("Adding {validator}.", nameof(NativeFileBatchValidator));
				batchValidators.Add(container.Resolve<NativeFileBatchValidator>());
			}

			if (exportSettings.ExportImages && exportSettings.VolumeInfo.CopyImageFilesFromRepository)
			{
				_logger.LogVerbose("Adding {validator}.", nameof(ImageFileBatchValidator));
				batchValidators.Add(container.Resolve<ImageFileBatchValidator>());
			}

			if (exportSettings.ExportImages)
			{
				_logger.LogVerbose("Adding {validator}.", nameof(ImageLoadFileBatchValidator));
				batchValidators.Add(container.Resolve<ImageLoadFileBatchValidator>());
			}

			if (exportSettings.ExportNative)
			{
				_logger.LogVerbose("Adding {validator}.", nameof(LoadFileBatchValidator));
				batchValidators.Add(container.Resolve<LoadFileBatchValidator>());
			}

			return new BatchValidator(batchValidators, _logger);
		}
	}
}