// ----------------------------------------------------------------------------
// <copyright file="IFileInfo.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;

	/// <summary>
	/// Represents a wrapper for the <see cref="T:System.IO.FileInfo"/> class.
	/// </summary>
	[CLSCompliant(false)]
	public interface IFileInfo
	{
		/// <summary>
		/// Gets a value indicating whether a file exists.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> if the file exists; <see langword="false" /> if the file does not exist or if the file is a directory.
		/// </returns>
		IDirectoryInfo Directory
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether a file exists.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> if the file exists; <see langword="false" /> if the file does not exist or if the file is a directory.
		/// </returns>
		bool Exists
		{
			get;
		}

		/// <summary>
		/// Gets the full path of the directory or file.
		/// </summary>
		/// <returns>
		/// A string containing the full path.
		/// </returns>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The fully qualified path and file name is 260 or more characters.
		/// </exception>
		/// <exception cref="T:System.Security.SecurityException">
		/// The caller does not have the required permission.
		/// </exception>
		string FullName
		{
			get;
		}

		/// <summary>
		/// Gets the size, in bytes, of the current file.
		/// </summary>
		/// <returns>
		/// The size of the current file in bytes.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">
		/// <see cref="M:System.IO.FileSystemInfo.Refresh" /> cannot update the state of the file or directory.
		/// </exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		/// The file does not exist.-or- The <see langword="Length" /> property is called for a directory.
		/// </exception>
		long Length
		{
			get;
		}

		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		/// <returns>
		/// The name of the file.
		/// </returns>
		string Name
		{
			get;
		}

		/// <summary>
		/// Refreshes the state of the object.
		/// </summary>
		/// <exception cref="T:System.IO.IOException">
		/// A device such as a disk drive is not ready.
		/// </exception>
		void Refresh();
	}
}