using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class ImageLoadFileMetadataBuilderFactory
	{
		private readonly ImageLoadFileMetadataForArtifactBuilderFactory _forArtifactBuilderFactory;
		private readonly ILog _logger;

		public ImageLoadFileMetadataBuilderFactory(ImageLoadFileMetadataForArtifactBuilderFactory forArtifactBuilderFactory, ILog logger)
		{
			_forArtifactBuilderFactory = forArtifactBuilderFactory;
			_logger = logger;
		}

		public IImageLoadFileMetadataBuilder Create(ExportFile exportSettings)
		{
			IImageLoadFileMetadataForArtifactBuilder defaultMetadataBuilder = _forArtifactBuilderFactory.Create(exportSettings);
			IImageLoadFileMetadataForArtifactBuilder unsuccessfulRollupMetadataBuilder = _forArtifactBuilderFactory.CreateForUnsuccessfulRollup(exportSettings);

			return new ImageLoadFileMetadataBuilder(defaultMetadataBuilder, unsuccessfulRollupMetadataBuilder, _logger);
		}
	}
}