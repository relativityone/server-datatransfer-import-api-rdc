namespace Relativity.Export.VolumeManagerV2.ImagesRollup
{
	using kCura.WinEDDS.Exporters;

	public interface IImagesRollup
	{
		void RollupImages(ObjectExportInfo artifact);
	}
}