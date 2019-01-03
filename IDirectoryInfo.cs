// ----------------------------------------------------------------------------
// <copyright file="IDirectoryInfo.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;

	/// <summary>
	/// Represents a wrapper for the <see cref="T:System.IO.DirectoryInfo"/> class.
	/// </summary>
	[CLSCompliant(false)]
	public interface IDirectoryInfo
	{
		/// <summary>
		/// Gets a value indicating whether the directory exists.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> if the directory exists; otherwise, <see langword="false" />.
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
		/// Gets the name of this <see cref="T:System.IO.DirectoryInfo" /> instance.
		/// </summary>
		/// <returns>
		/// The directory name.
		/// </returns>
		string Name
		{
			get;
		}

		/// <summary>
		/// Creates a directory.
		/// </summary>
		/// <exception cref="T:System.IO.IOException">
		/// The directory cannot be created.
		/// </exception>
		void Create();

		/// <summary>
		/// Refreshes the state of the object.
		/// </summary>
		/// <exception cref="T:System.IO.IOException">
		/// A device such as a disk drive is not ready.
		/// </exception>
		void Refresh();
	}
}