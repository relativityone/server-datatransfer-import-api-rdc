using Castle.Windsor;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchValidatorFactory
	{
		public IBatchValidator Create(ExportFile exportSettings, IWindsorContainer container)
		{
			BatchValidator batchValidator = new BatchValidator();

			if (exportSettings.ExportFullTextAsFile)
			{
				batchValidator.AddBatchValidator(container.Resolve<LongTextBatchValidator>());
			}
			if (exportSettings.ExportNative && exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
			{
				batchValidator.AddBatchValidator(container.Resolve<NativeFileBatchValidator>());
			}
			if (exportSettings.ExportImages && exportSettings.VolumeInfo.CopyImageFilesFromRepository)
			{
				batchValidator.AddBatchValidator(container.Resolve<ImageFileBatchValidator>());
			}
			if (exportSettings.ExportImages)
			{
				batchValidator.AddBatchValidator(container.Resolve<ImageLoadFileBatchValidator>());
			}
			if (exportSettings.ExportNative)
			{
				batchValidator.AddBatchValidator(container.Resolve<LoadFileBatchValidator>());
			}

			return batchValidator;
		}
	}
}