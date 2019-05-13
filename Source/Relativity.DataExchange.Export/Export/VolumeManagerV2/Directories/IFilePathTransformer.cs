namespace Relativity.DataExchange.Export.VolumeManagerV2.Directories
{
	public interface IFilePathTransformer
	{
		string TransformPath(string filePath);
	}
}