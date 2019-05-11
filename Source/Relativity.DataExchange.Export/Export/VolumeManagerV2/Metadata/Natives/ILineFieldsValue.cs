namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives
{
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	public interface ILineFieldsValue
	{
		void AddFieldsValue(DeferredEntry loadFileEntry, ObjectExportInfo artifact);
	}
}