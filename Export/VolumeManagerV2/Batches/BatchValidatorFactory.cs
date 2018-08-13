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

		public IBatchValidator Create(ExportFile exportSettings, IExportConfig exportConfig, IWindsorContainer container)
		{
			_logger.LogVerbose("Creating BatchValidator.");
			List<IBatchValidator> batchValidators = new List<IBatchValidator>();

			if (exportSettings.ExportFullTextAsFile)
			{
				if (!exportConfig.ForceParallelismInNewExport)
				{
					AddValidator<LongTextBatchValidator>(batchValidators, container);
				}
				else
				{
					AddValidator<LongTextParallelBatchValidator>(batchValidators, container);
				}
			}

			if (exportSettings.ExportNative && exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
			{
				if (!exportConfig.ForceParallelismInNewExport)
				{
					AddValidator<NativeFileBatchValidator>(batchValidators, container);
				}
				else
				{
					AddValidator<NativeFileParallelBatchValidator>(batchValidators, container);
				}
			}

			if (exportSettings.ExportImages && exportSettings.VolumeInfo.CopyImageFilesFromRepository)
			{
				if (exportConfig.ForceParallelismInNewExport)
				{
					AddValidator<ImageFileParallelBatchValidator>(batchValidators, container);
				}
				else
				{
					AddValidator<ImageFileBatchValidator>(batchValidators, container);
				}
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