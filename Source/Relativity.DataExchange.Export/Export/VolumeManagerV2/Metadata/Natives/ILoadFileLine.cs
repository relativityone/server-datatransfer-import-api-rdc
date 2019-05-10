namespace Relativity.Export.VolumeManagerV2.Metadata.Natives
{
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	public interface ILoadFileLine
	{
		ILoadFileEntry CreateLine(ObjectExportInfo artifact);
	}
}