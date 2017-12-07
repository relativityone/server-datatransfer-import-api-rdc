﻿using Castle.Windsor;
using Relativity;
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
			if (exportSettings.ArtifactTypeID != (int) ArtifactType.Document)
			{
				return new EmptyImageLoadFileMetadataBuilder();
			}
			ImageLoadFileMetadataForArtifactBuilderFactory factory = container.Resolve<ImageLoadFileMetadataForArtifactBuilderFactory>();
			IImageLoadFileMetadataForArtifactBuilder defaultMetadataBuilder = factory.Create(exportSettings);
			IImageLoadFileMetadataForArtifactBuilder unsuccessfulRollupMetadataBuilder = factory.CreateForUnsuccessfulRollup(exportSettings);

			return new ImageLoadFileMetadataBuilder(defaultMetadataBuilder, unsuccessfulRollupMetadataBuilder, _logger);
		}
	}
}