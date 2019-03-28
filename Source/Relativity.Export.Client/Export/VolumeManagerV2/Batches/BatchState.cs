using System;
using System.Collections.Generic;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchState : IBatchState
	{
		private readonly IList<IStateful> _statefulComponents;
		private readonly ILog _logger;

		public BatchState(IList<IStateful> statefulComponents, ILog logger)
		{
			_statefulComponents = statefulComponents;
			_logger = logger;
		}

		public void SaveState()
		{
			_logger.LogVerbose("Saving state after batch.");
			try
			{
				foreach (var statefulComponent in _statefulComponents)
				{
					statefulComponent.SaveState();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred during batch state saving.");
				throw;
			}

			_logger.LogVerbose("Batch state saved.");
		}

		public void RestoreState()
		{
			_logger.LogVerbose("Restoring previous batch state.");
			try
			{
				foreach (var statefulComponent in _statefulComponents)
				{
					statefulComponent.RestoreLastState();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred during batch state restoring.");
				throw;
			}

			_logger.LogVerbose("Batch state restored.");
		}
	}
}