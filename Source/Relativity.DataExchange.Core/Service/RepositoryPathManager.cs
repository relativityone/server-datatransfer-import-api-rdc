// ----------------------------------------------------------------------------
// <copyright file="RepositoryPathManager.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	/// <summary>
	/// Represents a class object to manage paths within a given repository.
	/// </summary>
	internal class RepositoryPathManager
	{
		/// <summary>
		/// The minimum volume size.
		/// </summary>
		public const int MinVolumeSize = 500;

		private readonly int maxVolumeSize;
		private string currentSubDirectory;
		private string lastSubdirectory;
		private int currentFileNumber;

		/// <summary>
		/// Initializes a new instance of the <see cref="RepositoryPathManager"/> class.
		/// </summary>
		/// <param name="maximumVolumeSize">
		/// The maximum size of the volume.
		/// </param>
		public RepositoryPathManager(int maximumVolumeSize)
		{
			this.currentSubDirectory = GetNewSubdirectory();
			this.lastSubdirectory = string.Copy(this.currentSubDirectory);
			this.maxVolumeSize = System.Math.Max(MinVolumeSize, maximumVolumeSize);
		}

		/// <summary>
		/// Gets the current destination directory.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public string CurrentDestinationDirectory
		{
			get
			{
				return this.currentSubDirectory;
			}
		}

		/// <summary>
		/// Gets the maximum number of files per volume.
		/// </summary>
		/// <value>
		/// The file count.
		/// </value>
		public int MaxVolumeSize
		{
			get
			{
				return this.maxVolumeSize;
			}
		}

		/// <summary>
		/// Retrieves a new sub-directory path.
		/// </summary>
		/// <param name="path">
		/// The path.
		/// </param>
		/// <returns>
		/// The new path.
		/// </returns>
		public static string GetNewSubDirectory(string path)
		{
			return System.IO.Path.Combine(path, GetNewSubdirectory()) + @"\";
		}

		/// <summary>
		/// Rollback the file count.
		/// </summary>
		public void Rollback()
		{
			if (this.currentFileNumber == 1)
			{
				this.currentFileNumber = this.maxVolumeSize;
				this.currentSubDirectory = string.Copy(this.lastSubdirectory);
			}
			else
			{
				this.currentFileNumber -= 1;
			}
		}

		/// <summary>
		/// Retrieve the next destination directory name.
		/// </summary>
		/// <param name="repositoryPath">
		/// The repository path.
		/// </param>
		/// <returns>
		/// The new path.
		/// </returns>
		public string GetNextDestinationDirectory(string repositoryPath)
		{
			this.currentFileNumber += 1;
			if (this.currentFileNumber > this.maxVolumeSize)
			{
				this.currentFileNumber = 1;
				this.lastSubdirectory = string.Copy(this.currentSubDirectory);
				this.currentSubDirectory = GetNewSubdirectory();
			}

			return System.IO.Path.Combine(repositoryPath, this.currentSubDirectory) + @"\";
		}

		private static string GetNewSubdirectory()
		{
			return "RV_" + System.Guid.NewGuid().ToString();
		}
	}
}