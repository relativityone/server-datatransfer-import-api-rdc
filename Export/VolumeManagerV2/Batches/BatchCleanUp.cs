using System;
using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchCleanUp : IBatchCleanUp
	{
		private readonly IList<IClearable> _repositories;

		private readonly ILog _logger;

		public BatchCleanUp(IList<IClearable> repositories, ILog logger)
		{
			_repositories = repositories;
			_logger = logger;
		}

		public void CleanUp()
		{
			_logger.LogVerbose("Clearing repositories after exporting batch.");
			foreach (var repository in _repositories)
			{
				try
				{
					repository.Clear();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred during repository cleanup. Continuing to the next repository.");
				}
			}

			_logger.LogVerbose("Repositories cleared.");
		}
	}
}