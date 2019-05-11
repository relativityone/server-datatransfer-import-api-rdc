namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives
{
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	public interface ILineNativeFilePath
	{
		void AddNativeFilePath(DeferredEntry loadFileEntry, ObjectExportInfo artifact);
	}
}