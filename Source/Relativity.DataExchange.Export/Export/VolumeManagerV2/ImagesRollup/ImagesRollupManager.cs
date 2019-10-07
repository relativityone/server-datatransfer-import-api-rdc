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
			DateTime imageRollupStartTime = DateTime.Now;
			this._logger.LogVerbose("Starting image rollup...");
			foreach (ObjectExportInfo artifact in artifacts)
			{
				try
				{
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}

					this._logger.LogVerbose("Preparing to rollup images for artifact {ArtifactId}.", artifact.ArtifactID);
					this._imagesRollup.RollupImages(artifact);
					this._logger.LogVerbose("Successfully rolled up images for artifact {ArtifactId}.", artifact.ArtifactID);
				}
				catch (Exception ex)
				{
					this._logger.LogError(ex, "Failed to perform image rollup for artifact {ArtifactId}", artifact.ArtifactID);
					artifact.DocumentError = true;
					this._status.WriteError($"Unexpected error occurred during image rollup for artifact {artifact.ArtifactID}");
				}
			}

			TimeSpan elapsed = DateTime.Now - imageRollupStartTime;
			this._logger.LogVerbose("Successfully rolled up images. Elapsed: {ImageRollupElapsedTime}", elapsed);
		}
	}
}