using Castle.Windsor;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class ImageLoadFileMetadataBuilderFactory
	{
		private readonly ILog _logger;

		public ImageLoadFileMetadataBuilderFactory(ILog logger)
		{
			_logger = logger;
		}

		public IImageLoadFileMetadataBuilder Create(ExportFile exportSettings, IWindsorContainer container)
		{
			ImageLoadFileMetadataForArtifactBuilderFactory factory = container.Resolve<ImageLoadFileMetadataForArtifactBuilderFactory>();
			IImageLoadFileMetadataForArtifactBuilder defaultMetadataBuilder = factory.Create(exportSettings);
			IImageLoadFileMetadataForArtifactBuilder unsuccessfulRollupMetadataBuilder = factory.CreateForUnsuccessfulRollup(exportSettings);
			IRetryableStreamWriter streamWriter = container.Resolve<ImageLoadFileRetryableStreamWriter>();

			return new ImageLoadFileMetadataBuilder(defaultMetadataBuilder, unsuccessfulRollupMetadataBuilder, streamWriter, _logger);
		}
	}
}