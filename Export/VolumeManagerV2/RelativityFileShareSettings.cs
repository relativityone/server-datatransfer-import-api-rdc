using System;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class RelativityFileShareSettings
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
	}
}