using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class ImageLoadFileMetadataBuilder : IImageLoadFileMetadataBuilder
	{
		private readonly IImageLoadFileMetadataForArtifactBuilder _forArtifactBuilder;
		private readonly IImageLoadFileMetadataForArtifactBuilder _unsuccessfulRollupForArtifactBuilder;
		private readonly IRetryableStreamWriter _writer;
		private readonly ILog _logger;

		public ImageLoadFileMetadataBuilder(IImageLoadFileMetadataForArtifactBuilder forArtifactBuilder, IImageLoadFileMetadataForArtifactBuilder unsuccessfulRollupForArtifactBuilder,
			IRetryableStreamWriter writer, ILog logger)
		{
			_forArtifactBuilder = forArtifactBuilder;
			_unsuccessfulRollupForArtifactBuilder = unsuccessfulRollupForArtifactBuilder;
			_logger = logger;
			_writer = writer;
		}

		public void CreateLoadFileEntries(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Creating metadata for image load file for current batch.");

			foreach (var artifact in artifacts)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_logger.LogVerbose("Creating image load file entry for artifact {artifactId}.", artifact.ArtifactID);

				if (artifact.Images.Count > 0)
				{
					var image = (ImageExportInfo) artifact.Images[0];
					if (image.SuccessfulRollup)
					{
						_logger.LogVerbose("Rollup successful for image {batesNumber}. Continuing with default metadata builder.", image.BatesNumber);
						_forArtifactBuilder.WriteLoadFileEntry(artifact, _writer, cancellationToken);
					}
					else
					{
						_logger.LogVerbose("Rollup unsuccessful for image {batesNumber}. Continuing with metadata builder for unsuccessful rollup.", image.BatesNumber);
						_unsuccessfulRollupForArtifactBuilder.WriteLoadFileEntry(artifact, _writer, cancellationToken);
					}
				}
				else
				{
					_logger.LogVerbose("No images for artifact {artifactId}.", artifact.ArtifactID);
				}
			}

			_logger.LogVerbose("Successfully create metadata for images.");
		}
	}
}