namespace Relativity.Export.VolumeManagerV2.Repository
{
	using Castle.Windsor;

	using kCura.WinEDDS;

	using Relativity.Export.VolumeManagerV2.Download;
	using Relativity.Import.Export.Service;
	using Relativity.Logging;

	public class ImageRepositoryBuilderFactory
	{
		private readonly ILog _logger;

		public ImageRepositoryBuilderFactory(ILog logger)
		{
			_logger = logger;
		}

		public ImageRepositoryBuilder Create(ExportFile exportSettings, IWindsorContainer container)
		{
			IExportRequestBuilder imageExportRequestBuilder;

			if (exportSettings.ArtifactTypeID != (int) ArtifactType.Document)
			{
				_logger.LogVerbose("Creating {requestBuilder} for images.", nameof(EmptyExportRequestBuilder));
				imageExportRequestBuilder = container.Resolve<EmptyExportRequestBuilder>();
			}
			else
			{
				if (exportSettings.ExportImages && exportSettings.VolumeInfo.CopyImageFilesFromRepository)
				{
					_logger.LogVerbose("Creating {requestBuilder} for images.", nameof(ImageExportRequestBuilder));
					imageExportRequestBuilder = container.Resolve<ImageExportRequestBuilder>();
				}
				else
				{
					_logger.LogVerbose("Creating {requestBuilder} for images.", nameof(EmptyExportRequestBuilder));
					imageExportRequestBuilder = container.Resolve<EmptyExportRequestBuilder>();
				}
			}

			return new ImageRepositoryBuilder(container.Resolve<ImageRepository>(), imageExportRequestBuilder, _logger);
		}
	}
}