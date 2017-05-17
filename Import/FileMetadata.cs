
using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import
{
	public class FileMetadata
	{
		public string FileName { get; set; }
		public string FileFullPathName { get; set; }
		public string FileGuid { get; set; }
		public bool FileExists { get; set; }

		public OI.FileID.FileIDData FileIdData { get; set; }

		public int LineNumber { get; set; }

		public ArtifactFieldCollection ArtifactFieldCollection { get; set; }

	}
}
