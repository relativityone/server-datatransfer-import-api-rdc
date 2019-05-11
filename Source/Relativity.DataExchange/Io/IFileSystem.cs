// ----------------------------------------------------------------------------
// <copyright file="IFileSystem.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System;

	/// <summary>
	/// Represents an abstract wrapper for the <see cref="System.IO"/> wrapper to access and create file system related objects.
	/// </summary>
	[CLSCompliant(false)]
	public interface IFileSystem
	{
		/// <summary>
		/// Gets an <see cref="IDirectory"/> instance that wraps the <see cref="System.IO.Directory"/> object.
		/// </summary>
		/// <value>
		/// The <see cref="IDirectory"/> instance.
		/// </value>
		IDirectory Directory
		{
			get;
		}

		/// <summary>
		/// Gets an <see cref="IFile"/> instance that wraps the <see cref="System.IO.File"/> object.
		/// </summary>
		/// <value>
		/// The <see cref="IFile"/> instance.
		/// </value>
		IFile File
		{
			get;
		}

		/// <summary>
		/// Gets an <see cref="IPath"/> instance that wraps the <see cref="System.IO.Path"/> object.
		/// </summary>
		/// <value>
		/// The <see cref="IPath"/> instance.
		/// </value>
		IPath Path
		{
			get;
		}

		/// <summary>
		/// Creates a new <see cref="IDirectoryInfo"/> instance for the specified directory.
		/// </summary>
		/// <param name="path">
		/// The fully qualified name of the directory.
		/// </param>
		/// <returns>
		/// The <see cref="IDirectoryInfo"/> instance.
		/// </returns>
		IDirectoryInfo CreateDirectoryInfo(string path);

		/// <summary>
		/// Creates a new <see cref="IFileInfo"/> instance for the specified file.
		/// </summary>
		/// <param name="fileName">
		/// The fully qualified name of the new file, or the relative file name. Do not end the path with the directory separator character.
		/// </param>
		/// <returns>
		/// The <see cref="IFileInfo"/> instance.
		/// </returns>
		IFileInfo CreateFileInfo(string fileName);

		/// <summary>
		/// Creates a new <see cref="IStreamWriter"/> instance for the specified file on the specified path, UTF-8 encoding, and default buffer size. If the file exists, it can be either overwritten or appended to. If the file does not exist, a new file is created.
		/// </summary>
		/// <param name="path">
		/// The complete file path to write to.
		/// </param>
		/// <param name="append">
		/// <see langword="true" /> to append data to the file; <see langword="false" /> to overwrite the file. If the specified file does not exist, this parameter has no effect, and the constructor creates a new file.
		/// </param>
		/// <returns>
		/// The <see cref="IStreamWriter"/> instance.
		/// </returns>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// Access is denied.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is empty. -or-
		/// <paramref name="path" /> contains the name of a system device (com1, com2, and so on).
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// <paramref name="path" /> includes an incorrect or invalid syntax for file name, directory name, or volume label syntax.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must not exceed 248 characters, and file names must not exceed 260 characters.
		/// </exception>
		/// <exception cref="T:System.Security.SecurityException">
		/// The caller does not have the required permission.
		/// </exception>
		IStreamWriter CreateStreamWriter(string path, bool append);

		/// <summary>
		/// Creates a new <see cref="IStreamWriter"/> instance for the specified file on the specified path, using the specified encoding and default buffer size. If the file exists, it can be either overwritten or appended to. If the file does not exist, a new file is created.
		/// </summary>
		/// <param name="path">
		/// The complete file path to write to.
		/// </param>
		/// <param name="append">
		/// <see langword="true" /> to append data to the file; <see langword="false" /> to overwrite the file. If the specified file does not exist, this parameter has no effect, and the constructor creates a new file.
		/// </param>
		/// <param name="encoding">
		/// The character encoding to use.
		/// </param>
		/// <returns>
		/// The <see cref="IStreamWriter"/> instance.
		/// </returns>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// Access is denied.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> is empty. -or-
		/// <paramref name="path" /> contains the name of a system device (com1, com2, and so on).
		/// </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">
		/// The specified path is invalid (for example, it is on an unmapped drive).
		/// </exception>
		/// <exception cref="T:System.IO.IOException">
		/// <paramref name="path" /> includes an incorrect or invalid syntax for file name, directory name, or volume label syntax.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must not exceed 248 characters, and file names must not exceed 260 characters.
		/// </exception>
		/// <exception cref="T:System.Security.SecurityException">
		/// The caller does not have the required permission.
		/// </exception>
		IStreamWriter CreateStreamWriter(string path, bool append, System.Text.Encoding encoding);

		/// <summary>
		/// Creates a deep copy of this instance.
		/// </summary>
		/// <returns>
		/// The <see cref="IFileSystem"/> instance.
		/// </returns>
		IFileSystem DeepCopy();
	}
}