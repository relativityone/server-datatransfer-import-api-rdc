using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public class TransferRateEventArgs : EventArgs
	{
		public TransferRateEventArgs(long transferredBytes, long transferTime)
		{
			TransferredBytes = transferredBytes;
			TransferTime = transferTime;
		}

		public long TransferredBytes { get; }
		public long TransferTime { get; }
	}
}