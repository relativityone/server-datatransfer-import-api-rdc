// <copyright file="TempFileBuilder.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Io
{
	using Relativity.DataExchange;

	/// <summary>
	/// Provides static methods to build temporary files that are easier to debug and sort than <see cref="System.IO.Path.GetTempFileName"/> and employ unique file names to avoid collisions.
	/// </summary>
	/// <remarks>
	/// REL-135101: avoid failures when Temp is full.
	/// </remarks>
	public static class TempFileBuilder
	{
		private static IFileSystem _fileSystem;

		private static IFileSystem FileSystem
		{
			get
			{
				// Limiting custom temp directory configuration to just this class.
				if (_fileSystem == null)
				{
					_fileSystem = Io.FileSystem.Instance.DeepCopy();
					_fileSystem.Path.CustomTempPath = AppSettings.Instance.TempDirectory;
				}

				return _fileSystem;
			}
		}

		/// <summary>
		/// Gets a uniquely named, zero-byte temporary file on disk and returns the full path of that file.
		/// </summary>
		/// <returns>
		/// The full path of the temporary file.
		/// </returns>
		public static string CreateEmptyFile()
		{
			return CreateEmptyFile(null);
		}

		/// <summary>
		/// Gets a uniquely named with a user-defined suffix, zero-byte temporary file on disk and returns the full path of that file.
		/// </summary>
		/// <param name="fileNameSuffix">
		/// Specify the suffix applied to the unique file name.
		/// </param>
		/// <returns>
		/// The full path of the temporary file.
		/// </returns>
		public static string CreateEmptyFile(string fileNameSuffix)
		{
			// The implementation has been relocated to the IPath object.
			return FileSystem.Path.CreateEmptyFile(fileNameSuffix);
		}

		/// <summary>
		/// Gets a uniquely named with a user-defined suffix, zero-byte temporary file on disk and returns the full path of that file.
		/// </summary>
		/// <param name="fileNameSuffix">
		/// Specify the suffix applied to the unique file name.
		/// </param>
		/// <returns>
		/// The full path of the temporary file.
		/// </returns>
		public static string TemporaryFileName(string fileNameSuffix)
		{
			// The implementation has been relocated to the IPath object.
			return FileSystem.Path.TemporaryFileName(fileNameSuffix);
		}
	}
}