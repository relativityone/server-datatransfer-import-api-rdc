using System;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class RelativityFileShareSettings : IRelativityFileShareSettings
	{

		private readonly RelativityFileShare _fileShare;

		public RelativityFileShareSettings(RelativityFileShare fileShare)
		{
			if (fileShare == null)
			{
				throw new ArgumentNullException(nameof(fileShare));
			}

			_fileShare = fileShare;
		}

		public AsperaCredential TransferCredential => _fileShare.TransferCredential;

		public string UncPath => _fileShare.Url;

		public bool IsBaseOf(string path)
		{
			bool result = _fileShare.IsBaseOf(path);
			return result;
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
			
			RelativityFileShareSettings other = obj as RelativityFileShareSettings;

			if (other == null)
			{
				return false;
			}

			return string.Equals(UncPath, other.UncPath, StringComparison.OrdinalIgnoreCase) && (TransferCredential?.Equals(other.TransferCredential) ?? false);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (!string.IsNullOrEmpty(UncPath) ? UncPath.GetHashCode() : 0) * 397 ^ TransferCredential?.GetHashCode() ?? 1;
			}
		}
	}
}