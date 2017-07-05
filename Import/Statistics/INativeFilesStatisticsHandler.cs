using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public interface INativeFilesStatisticsHandler : ITransferRateHandler
	{
		event EventHandler<FilesTransferredEventArgs> FilesTransferred;
	}
}