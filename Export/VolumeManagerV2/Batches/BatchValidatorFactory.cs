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
			BatchValidator batchValidator = new BatchValidator(_logger);

			if (exportSettings.ExportFullTextAsFile)
			{
				_logger.LogVerbose("Adding {validator}.", nameof(LongTextBatchValidator));
				batchValidator.AddBatchValidator(container.Resolve<LongTextBatchValidator>());
			}
			if (exportSettings.ExportNative && exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
			{
				_logger.LogVerbose("Adding {validator}.", nameof(NativeFileBatchValidator));
				batchValidator.AddBatchValidator(container.Resolve<NativeFileBatchValidator>());
			}
			if (exportSettings.ExportImages && exportSettings.VolumeInfo.CopyImageFilesFromRepository)
			{
				_logger.LogVerbose("Adding {validator}.", nameof(ImageFileBatchValidator));
				batchValidator.AddBatchValidator(container.Resolve<ImageFileBatchValidator>());
			}
			if (exportSettings.ExportImages)
			{
				_logger.LogVerbose("Adding {validator}.", nameof(ImageLoadFileBatchValidator));
				batchValidator.AddBatchValidator(container.Resolve<ImageLoadFileBatchValidator>());
			}
			if (exportSettings.ExportNative)
			{
				_logger.LogVerbose("Adding {validator}.", nameof(LoadFileBatchValidator));
				batchValidator.AddBatchValidator(container.Resolve<LoadFileBatchValidator>());
			}

			return batchValidator;
		}
	}
}