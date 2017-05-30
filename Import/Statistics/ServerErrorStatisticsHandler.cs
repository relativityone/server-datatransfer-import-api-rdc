using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public class ServerErrorStatisticsHandler : IServerErrorStatisticsHandler
	{
		public event EventHandler RetrievingServerErrors;

		public void RaiseRetrievingServerErrorsEvent()
		{
			RetrievingServerErrors?.Invoke(this, EventArgs.Empty);
		}
	}
}