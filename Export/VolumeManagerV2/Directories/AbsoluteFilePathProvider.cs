namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class AbsoluteFilePathProvider : IFilePathProvider
	{
		public string GetPathForLoadFile(string filePath)
		{
			return filePath;
		}
	}
}