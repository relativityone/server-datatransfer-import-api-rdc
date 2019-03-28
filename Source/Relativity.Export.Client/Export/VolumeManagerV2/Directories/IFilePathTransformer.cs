namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public interface IFilePathTransformer
	{
		string TransformPath(string filePath);
	}
}