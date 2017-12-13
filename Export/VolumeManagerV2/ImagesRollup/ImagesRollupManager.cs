using System.Threading;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
	public class ImagesRollupManager : IImagesRollupManager
	{
		private readonly IImagesRollup _imagesRollup;
		private readonly ILog _logger;

		public ImagesRollupManager(IImagesRollup imagesRollup, ILog logger)
		{
			_imagesRollup = imagesRollup;
			_logger = logger;
		}

		public void RollupImagesForArtifacts(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			foreach (ObjectExportInfo artifact in artifacts)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
				_logger.LogVerbose("Attempting to rollup images for artifact {artifactId}.", artifact.ArtifactID);
				_imagesRollup.RollupImages(artifact);
			}
		}
	}
}