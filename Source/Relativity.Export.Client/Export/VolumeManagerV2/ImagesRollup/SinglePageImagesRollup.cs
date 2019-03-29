namespace Relativity.Export.VolumeManagerV2.ImagesRollup
{
	using kCura.WinEDDS.Exporters;

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