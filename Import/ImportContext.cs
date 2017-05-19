namespace kCura.WinEDDS.Core.Import
{
	public class ImportContext
	{
		public IImporterSettings Settings { get; set; }

		public long TotalRecordCount { get; set; }
		public int ParentFolderId { get; set; }
		public string FolderPath { get; set; }
		public MetadataFilesInfo MetadataFilesInfo { get; set; }
	}
}