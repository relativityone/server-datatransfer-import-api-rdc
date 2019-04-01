namespace Relativity.Export.VolumeManagerV2.Settings
{
	using kCura.WinEDDS;

	using ViewFieldInfo = Relativity.ViewFieldInfo;

	public interface IColumnsFactory
	{
		kCura.WinEDDS.ViewFieldInfo[] CreateColumns(ExportFile exportSettings);
	}
}