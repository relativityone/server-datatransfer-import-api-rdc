using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public interface IServerErrorStatisticsHandler
	{
		event EventHandler RetrievingServerErrors;
		event EventHandler<string> RetrievingServerErrorStatusUpdated; 

		void RaiseRetrievingServerErrorsEvent();
		void RaiseRetrievingServerErrorStatusUpdatedEvent(string message);
	}
}