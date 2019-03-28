namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public interface IImageLoadFileEntry
	{
		string Create(string batesNumber, string filePath, string volume, int pageNumber, int numberOfImages);
	}
}