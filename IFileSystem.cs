// ----------------------------------------------------------------------------
// <copyright file="IFileSystem.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;

	/// <summary>
	/// Represents an abstract <see cref="System.IO"/> wrapper to access and create file system related objects.
	/// </summary>
	[CLSCompliant(false)]
	public interface IFileSystem
	{
		/// <summary>
		/// Get an <see cref="IDirectory"/> instance that wraps the <see cref="System.IO.Directory"/> object.
		/// </summary>
		/// <value>
		/// The <see cref="IDirectory"/> instance.
		/// </value>
		IDirectory Directory
		{
			get;
		}

		/// <summary>
		/// Get an <see cref="IFile"/> instance that wraps the <see cref="System.IO.File"/> object.
		/// </summary>
		/// <value>
		/// The <see cref="IFile"/> instance.
		/// </value>
		IFile File
		{
			get;
		}

		/// <summary>
		/// Get an <see cref="IPath"/> instance that wraps the <see cref="System.IO.Path"/> object.
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
		/// Creates a deep copy of this instance.
		/// </summary>
		/// <returns>
		/// The <see cref="IFileSystem"/> instance.
		/// </returns>
		IFileSystem DeepCopy();
	}
}