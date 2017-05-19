using System;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Helpers
{
	public class RepositoryFilePathHelper : IRepositoryFilePathHelper
	{
		private Relativity.RepositoryPathManager _repositoryPathManager;

		public RepositoryFilePathHelper()
		{
			_repositoryPathManager = new RepositoryPathManager(0);
		}

		public string GetNextDestinationDirectory()
		{
			// we don't need to handle repo root path as it will be combined in uploader class
			return _repositoryPathManager.GetNextDestinationDirectory(string.Empty);
		}

		public string CurrentDestinationDirectory => _repositoryPathManager.CurrentDestinationDirectory;
	}
}
