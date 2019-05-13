// ----------------------------------------------------------------------------
// <copyright file="IDirectory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System;

	/// <summary>
	/// Represents an abstract wrapper for the <see cref="T:System.IO.Directory"/> class.
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
		/// Deletes an empty directory from a specified path.
		/// </summary>
		/// <param name="path">
		/// The name of the empty directory to remove. This directory must be writable and empty.
		/// </param>
		/// <exception cref="T:System.IO.IOException">
		/// A file with the same name and location specified by <paramref name="path" /> exists.-or-The directory is the application's current working directory.-or-The directory specified by <paramref name="path" /> is not empty.-or-The directory is read-only or contains a read-only file.-or-The directory is being used by another process.
		/// </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// <paramref name="path" /> does not exist or could not be found.-or-The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		void Delete(string path);

		/// <summary>
		/// Deletes the specified directory and, if indicated, any subdirectories and files in the directory.
		/// </summary>
		/// <param name="path">
		/// The name of the directory to remove.
		/// </param>
		/// <param name="recursive">
		/// <see langword="true" /> to remove directories, subdirectories, and files in <paramref name="path" />; otherwise, <see langword="false" />.
		/// </param>
		/// <exception cref="T:System.IO.IOException">
		/// A file with the same name and location specified by <paramref name="path" /> exists.-or-The directory specified by <paramref name="path" /> is read-only, or <paramref name="recursive" /> is <see langword="false" /> and <paramref name="path" /> is not an empty directory. -or-The directory is the application's current working directory. -or-The directory contains a read-only file.-or-The directory is being used by another process.
		/// </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// <paramref name="path" /> does not exist or could not be found.-or-The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		void Delete(string path, bool recursive);

		/// <summary>
		/// Deletes the specified directory and, if indicated, any subdirectories and files in the directory only when it exists and optionally throw when a failure occurs.
		/// </summary>
		/// <param name="path">
		/// The name of the directory to remove.
		/// </param>
		/// <param name="recursive">
		/// <see langword="true" /> to remove directories, subdirectories, and files in <paramref name="path" />; otherwise, <see langword="false" />.
		/// </param>
		/// <param name="throwOnExistsCheck">
		/// <see langword="true" /> to throw an exception if the exists check fails; otherwise, <see langword="false" />.
		/// </param>
		/// <exception cref="T:System.IO.IOException">
		/// A file with the same name and location specified by <paramref name="path" /> exists.-or-The directory specified by <paramref name="path" /> is read-only, or <paramref name="recursive" /> is <see langword="false" /> and <paramref name="path" /> is not an empty directory. -or-The directory is the application's current working directory. -or-The directory contains a read-only file.-or-The directory is being used by another process.
		/// </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// <paramref name="path" /> does not exist or could not be found.-or-The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		void DeleteIfExists(string path, bool recursive, bool throwOnExistsCheck);

		/// <summary>
		/// Determines whether the given path refers to an existing directory on disk.
		/// </summary>
		/// <param name="path">
		/// The path to test.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if <paramref name="path" /> refers to an existing directory; <see langword="false" /> if the directory does not exist or an error occurs when trying to determine if the specified directory exists.
		/// </returns>
		bool Exists(string path);

		/// <summary>
		/// Determines whether the given path refers to an existing directory on disk and optionally throw when a failure occurs.
		/// </summary>
		/// <param name="path">
		/// The path to test.
		/// </param>
		/// <param name="throwOnExistsCheck">
		/// <see langword="true" /> to throw an exception if the exists check fails; otherwise, <see langword="false" />.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if <paramref name="path" /> refers to an existing directory; <see langword="false" /> if the directory does not exist or an error occurs when trying to determine if the specified directory exists.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">
		/// A file with the same name and location specified by <paramref name="path" /> exists.-or-The directory specified by <paramref name="path" /> is read-only. -or-The directory is the application's current working directory. -or-The directory contains a read-only file.-or-The directory is being used by another process.
		/// </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// <paramref name="path" /> does not exist or could not be found.-or-The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		bool Exists(string path, bool throwOnExistsCheck);

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
	}
}