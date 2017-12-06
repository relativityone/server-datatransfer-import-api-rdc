using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
	public class ImagesRollupManager : IImagesRollupManager
	{
		private readonly IImagesRollup _imagesRollup;

		public ImagesRollupManager(IImagesRollup imagesRollup)
		{
			_imagesRollup = imagesRollup;
		}

		public void RollupImagesForArtifacts(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			foreach (ObjectExportInfo artifact in artifacts)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
				_imagesRollup.RollupImages(artifact);
			}
		}
	}
}