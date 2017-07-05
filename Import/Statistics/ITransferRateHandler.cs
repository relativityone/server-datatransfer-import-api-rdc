using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public interface ITransferRateHandler
	{
		event EventHandler<TransferRateEventArgs> TransferRateChanged;
	}
}