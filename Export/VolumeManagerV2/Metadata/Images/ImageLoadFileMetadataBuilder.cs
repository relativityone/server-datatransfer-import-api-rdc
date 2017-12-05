using System.Collections.Generic;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class ImageLoadFileMetadataBuilder : IImageLoadFileMetadataBuilder
	{
		private readonly IImageLoadFileMetadataForArtifactBuilder _forArtifactBuilder;
		private readonly IImageLoadFileMetadataForArtifactBuilder _unsuccessfulRollupForArtifactBuilder;
		private readonly ILog _logger;

		public ImageLoadFileMetadataBuilder(IImageLoadFileMetadataForArtifactBuilder forArtifactBuilder, IImageLoadFileMetadataForArtifactBuilder unsuccessfulRollupForArtifactBuilder,
			ILog logger)
		{
			_forArtifactBuilder = forArtifactBuilder;
			_unsuccessfulRollupForArtifactBuilder = unsuccessfulRollupForArtifactBuilder;
			_logger = logger;
		}

		public IList<KeyValuePair<string, string>> CreateLoadFileEntries(ObjectExportInfo[] artifacts)
		{
			_logger.LogVerbose("Creating metadata for image load file for current batch.");

			var lines = new List<KeyValuePair<string, string>>();

			foreach (var artifact in artifacts)
			{
				_logger.LogVerbose("Creating image load file entry for artifact {artifactId}.", artifact.ArtifactID);

				if (artifact.Images.Count > 0)
				{
					var image = (ImageExportInfo) artifact.Images[0];
					if (image.SuccessfulRollup)
					{
						_logger.LogVerbose("Rollup successful for image {batesNumber}. Continuing with default metadata builder.", image.BatesNumber);
						_forArtifactBuilder.CreateLoadFileEntry(artifact, lines);
					}
					else
					{
						_logger.LogVerbose("Rollup unsuccessful for image {batesNumber}. Continuing with metadata builder for unsuccessful rollup.", image.BatesNumber);
						_unsuccessfulRollupForArtifactBuilder.CreateLoadFileEntry(artifact, lines);
					}
				}
				else
				{
					_logger.LogVerbose("No images for artifact {artifactId}.", artifact.ArtifactID);
				}
			}

			_logger.LogVerbose("Successfully create metadata for images.");
			return lines;
		}
	}
}