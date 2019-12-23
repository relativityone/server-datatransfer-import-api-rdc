// ----------------------------------------------------------------------------
// <copyright file="TestDirectoryManager.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.IO;

	/// <summary>
	/// Represents a class object used to manage test directories by limiting the number of files per directory.
	/// </summary>
	public class TestDirectoryManager
	{
		private readonly object syncRoot = new object();
		private readonly string parentDirectory;
		private int volumeNumber;
		private int subDirectoryNumber;
		private string currentVolumeDirectory;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestDirectoryManager" /> class.
		/// </summary>
		/// <param name="parentDirectory">
		/// The parent directory where all volumes are stored.
		/// </param>
		public TestDirectoryManager(string parentDirectory)
		{
			if (string.IsNullOrWhiteSpace(parentDirectory))
			{
				throw new ArgumentNullException(nameof(parentDirectory));
			}

			this.parentDirectory = parentDirectory;
			this.MaxFilesPerVolume = 500;
			this.volumeNumber = 0;
			this.subDirectoryNumber = 0;
			this.currentVolumeDirectory = null;
		}

		/// <summary>
		/// Gets or sets the maximum number of files per volume. This is set to 500 by default.
		/// </summary>
		/// <value>
		/// The maximum files per volume.
		/// </value>
		public int MaxFilesPerVolume
		{
			get;
			set;
		}

		/// <summary>
		/// Advances the sub-directory counter, creates the new volume directory if necessary, and returns the full directory path.
		/// </summary>
		/// <param name="fileName">
		/// The file name combined with the volume directory.
		/// </param>
		/// <returns>
		/// The full file path.
		/// </returns>
		public string MoveNext(string fileName)
		{
			lock (this.syncRoot)
			{
				this.subDirectoryNumber++;
				if (string.IsNullOrWhiteSpace(this.currentVolumeDirectory) || this.subDirectoryNumber > this.MaxFilesPerVolume)
				{
					this.subDirectoryNumber = 0;
					this.volumeNumber++;
					string folder = "VOL" + this.volumeNumber.ToString().PadLeft(4, '0');
					this.currentVolumeDirectory = System.IO.Path.Combine(this.parentDirectory, folder);
					Directory.CreateDirectory(this.currentVolumeDirectory);
				}

				return System.IO.Path.Combine(this.currentVolumeDirectory, fileName);
			}
		}
	}
}