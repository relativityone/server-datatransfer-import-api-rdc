namespace Relativity.DataExchange.Export.VolumeManagerV2.ImagesRollup
{
	using System;
	using System.Threading;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.Logging;

	public class ImagesRollupManager : IImagesRollupManager
	{
		private readonly IImagesRollup _imagesRollup;

		private readonly IStatus _status;

		private readonly ILog _logger;

		public ImagesRollupManager(IImagesRollup imagesRollup, IStatus status, ILog logger)
		{
			this._imagesRollup = imagesRollup;
			this._status = status;
			this._logger = logger;
		}

		public void RollupImagesForArtifacts(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			foreach (ObjectExportInfo artifact in artifacts)
			{
				try
				{
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}

					this._logger.LogVerbose("Attempting to rollup images for artifact {artifactId}.", artifact.ArtifactID);
					this._imagesRollup.RollupImages(artifact);
				}
				catch (Exception ex)
				{
					artifact.DocumentError = true;
					this._logger.LogError(ex, "Unexpected error occurred during image rollup for artifact {artifactId}", artifact.ArtifactID);
					this._status.WriteError($"Unexpected error occurred during image rollup for artifact {artifact.ArtifactID}");
				}
			}
		}
	}
}