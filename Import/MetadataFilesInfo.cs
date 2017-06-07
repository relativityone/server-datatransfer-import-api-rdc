namespace kCura.WinEDDS.Core.Import
{
	public class MetadataFilesInfo
	{
		public int BatchSize { get; set; }
		public FileMetadata CodeFilePath { get; set; }
		public FileMetadata ObjectFilePath { get; set; }
		public FileMetadata NativeFilePath { get; set; }
		public FileMetadata DataGridFilePath { get; set; }
	}
}