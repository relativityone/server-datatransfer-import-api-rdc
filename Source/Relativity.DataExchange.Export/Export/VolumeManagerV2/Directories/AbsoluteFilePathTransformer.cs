namespace Relativity.DataExchange.Export.VolumeManagerV2.Directories
{
	public class AbsoluteFilePathTransformer : IFilePathTransformer
	{
		public string TransformPath(string filePath)
		{
			return filePath;
		}
	}
}