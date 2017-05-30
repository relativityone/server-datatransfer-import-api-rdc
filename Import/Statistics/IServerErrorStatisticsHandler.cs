using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public interface IServerErrorStatisticsHandler
	{
		event EventHandler RetrievingServerErrors;

		void RaiseRetrievingServerErrorsEvent();
	}
}