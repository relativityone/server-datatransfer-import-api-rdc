namespace Relativity.Export.VolumeManagerV2.Settings
{
	using kCura.WinEDDS;

	public interface IColumnsFactory
	{
		kCura.WinEDDS.ViewFieldInfo[] CreateColumns(ExportFile exportSettings);
	}
}