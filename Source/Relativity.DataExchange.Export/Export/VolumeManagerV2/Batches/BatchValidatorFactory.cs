namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Collections.Generic;

	using Castle.Windsor;

	using kCura.WinEDDS;

	using Relativity.Logging;

	public class BatchValidatorFactory
	{
		private readonly ILog _logger;

		public BatchValidatorFactory(ILog logger)
		{
			_logger = logger;
		}

		public IBatchValidator Create(ExportFile exportSettings, IExportConfig exportConfig, IWindsorContainer container)
		{
			_logger.LogVerbose("Creating BatchValidator.");
			List<IBatchValidator> batchValidators = new List<IBatchValidator>();

			if (exportSettings.ExportFullTextAsFile)
			{
				AddValidator<LongTextBatchValidator>(batchValidators, container);
			}

			if (exportSettings.ExportNative && exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
			{
				AddValidator<NativeFileBatchValidator>(batchValidators, container);
			}

			if (exportSettings.ExportImages && exportSettings.VolumeInfo.CopyImageFilesFromRepository)
			{
				AddValidator<ImageFileBatchValidator>(batchValidators, container);
			}

			if (exportSettings.ExportImages)
			{
				AddValidator<ImageLoadFileBatchValidator>(batchValidators, container);
			}

			if (exportSettings.ExportNative)
			{
				AddValidator<LoadFileBatchValidator>(batchValidators, container);
			}

			return new BatchValidator(batchValidators, _logger);
		}

		private void AddValidator<T>(List<IBatchValidator> batchValidators, IWindsorContainer container) where T : IBatchValidator
		{
			_logger.LogVerbose("Adding {validator}.", nameof(T));
			batchValidators.Add(container.Resolve<T>());
		}
	}
}