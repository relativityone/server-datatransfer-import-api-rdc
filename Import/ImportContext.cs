namespace kCura.WinEDDS.Core.Import
{
	public class ImportContext
	{
		public IImporterSettings Settings { get; set; }
		public long TotalRecordCount { get; set; }
		public MetadataFilesInfo MetadataFilesInfo { get; set; }
	}
}