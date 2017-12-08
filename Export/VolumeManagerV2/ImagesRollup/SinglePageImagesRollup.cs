using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
	public class SinglePageImagesRollup : IImagesRollup
	{
		public void RollupImages(ObjectExportInfo artifact)
		{
			if (artifact.Images.Count > 0)
			{
				((ImageExportInfo) artifact.Images[0]).SuccessfulRollup = false;
			}
		}
	}
}