using System;

namespace Relativity.Import.Export.Services
{
	public class RepositoryPathManager
	{
		private Int32 _maxVolumeSize;
		private string _currentSubDirectory;
		private string _lastSubdirectory;
		private Int32 _currentFileNumber = 0;
		public const Int32 MINIMUM_VOLUME_SIZE = 500;

		public RepositoryPathManager(Int32 maximumVolumeSize)
		{
			_currentSubDirectory = GetNewSubdirectory();
			_lastSubdirectory = string.Copy(_currentSubDirectory);
			_maxVolumeSize = System.Math.Max(MINIMUM_VOLUME_SIZE, maximumVolumeSize);
		}

		public void Rollback()
		{
			if (_currentFileNumber == 1)
			{
				_currentFileNumber = _maxVolumeSize;
				_currentSubDirectory = string.Copy(_lastSubdirectory);
			}
			else
				_currentFileNumber -= 1;
		}

		public string GetNextDestinationDirectory(string repositoryPath)
		{
			_currentFileNumber += 1;
			if (_currentFileNumber > _maxVolumeSize)
			{
				_currentFileNumber = 1;
				_lastSubdirectory = string.Copy(_currentSubDirectory);
				_currentSubDirectory = GetNewSubdirectory();
			}
			return System.IO.Path.Combine(repositoryPath, _currentSubDirectory) + @"\";
		}

		public string CurrentDestinationDirectory
		{
			get
			{
				return _currentSubDirectory;
			}
		}

		private static string GetNewSubdirectory()
		{
			return "RV_" + System.Guid.NewGuid().ToString();
		}

		public Int32 MaxVolumeSize
		{
			get
			{
				return _maxVolumeSize;
			}
		}

		public static string GetNewSubDirectory(string path)
		{
			return System.IO.Path.Combine(path, GetNewSubdirectory()) + @"\";
		}
	}

}
