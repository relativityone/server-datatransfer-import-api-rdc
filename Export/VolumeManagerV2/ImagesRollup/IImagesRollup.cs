using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
	public interface IImagesRollup
	{
		bool RollupImages(ObjectExportInfo artifact);
	}
}