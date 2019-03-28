using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public interface ILoadFileLine
	{
		ILoadFileEntry CreateLine(ObjectExportInfo artifact);
	}
}