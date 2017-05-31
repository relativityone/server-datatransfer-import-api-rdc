using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public class FilesTransferredEventArgs : EventArgs
	{
		public FilesTransferredEventArgs(int filesTransferred)
		{
			FilesTransferred = filesTransferred;
		}

		public int FilesTransferred { get; }
	}
}