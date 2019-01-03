// ----------------------------------------------------------------------------
// <copyright file="IDirectory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;

	/// <summary>
	/// Represents a wrapper for the <see cref="T:System.IO.Directory"/> class.
	/// </summary>
	[CLSCompliant(false)]
	public interface IDirectory
	{
		/// <summary>
		/// Creates all directories and subdirectories in the specified path unless they already exist.
		/// </summary>
		/// <param name="path">
		/// The directory to create.
		/// </param>
		/// <returns>
		/// An object that represents the directory at the specified path. This object is returned regardless of whether a directory at the specified path already exists.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">
		/// The directory specified by <paramref name="path" /> is a file.-or-The network name is not known.
		/// </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.-or-
		/// <paramref name="path" /> is prefixed with, or contains, only a colon character (:).
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="path" /> contains a colon character (:) that is not part of a drive label ("C:\").
		/// </exception>
		void CreateDirectory(string path);

		/// <summary>
		/// Retrieves the parent directory of the specified path, including both absolute and relative paths.
		/// </summary>
		/// <param name="path">
		/// The path for which to retrieve the parent directory.
		/// </param>
		/// <returns>
		/// The parent directory, or <see langword="null" /> if <paramref name="path" /> is the root directory, including the root of a UNC server or share name.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">
		/// The directory specified by <paramref name="path" /> is read-only.
		/// </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters with the  <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The specified path was not found.
		/// </exception>
		IDirectoryInfo GetParent(string path);

		/// <summary>
		/// Determines whether the given path refers to an existing directory on disk.
		/// </summary>
		/// <param name="path">The path to test. </param>
		/// <returns>
		/// <see langword="true" /> if <paramref name="path" /> refers to an existing directory; <see langword="false" /> if the directory does not exist or an error occurs when trying to determine if the specified directory exists.
		/// </returns>
		bool Exists(string path);
	}
}