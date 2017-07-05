using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public class JobFinishStatisticsHandler : IJobFinishStatisticsHandler
	{
		public event EventHandler<string> JobFinished;

		public void RaiseJobFinishedEvent(string message)
		{
			JobFinished?.Invoke(this, message);
		}
	}
}