using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public interface IJobFinishStatisticsHandler
	{
		event EventHandler<string> JobFinished;

		void RaiseJobFinishedEvent(string message);
	}
}