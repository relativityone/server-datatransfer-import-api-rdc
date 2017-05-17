
using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import
{
	public class FileMetadata
	{
		public string FileName { get; set; }
		public string FileFullPathName { get; set; }
		public string FileGuid { get; set; }
		public bool FileExists { get; set; }
		public int LineNumber { get; set; }
		public OI.FileID.FileIDData FileIdData { get; set; }
		public ArtifactFieldCollection ArtifactFieldCollection { get; set; }
		
		protected bool Equals(FileMetadata other)
		{
			return string.Equals(FileGuid, other.FileGuid);
		}
		
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((FileMetadata) obj);
		}

		public override int GetHashCode()
		{
			return FileGuid != null ? FileGuid.GetHashCode() : 0;
		}
	}
}
