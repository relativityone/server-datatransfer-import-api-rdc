namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public interface IColumnsFactory
	{
		ViewFieldInfo[] CreateColumns(ExportFile exportSettings);
	}
}