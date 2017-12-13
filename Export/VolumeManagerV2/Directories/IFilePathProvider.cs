namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public interface IFilePathProvider
	{
		string GetPathForFile(string fileName);
	}
}