namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text
{
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	public interface ILongTextHandler
	{
		void HandleLongText(ObjectExportInfo artifact, kCura.WinEDDS.ViewFieldInfo field, DeferredEntry lineEntry);
	}
}