using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public interface ILongTextHandler
	{
		void HandleLongText(ObjectExportInfo artifact, ViewFieldInfo field, DeferredEntry lineEntry);
	}
}