namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public class ImageLoadFileEntryDataSet
	{
		public string BatesNumber { get; set; }
		public string FilePath { get; set; }
		public string Volume { get; set; }
		public int PageNumber { get; set; }
		public int NumberOfImages { get; set; }
		public ExportFile.ImageType ImageType { get; set; }
		public string ExpectedResult { get; set; }
	}
}