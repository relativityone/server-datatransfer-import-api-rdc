// ----------------------------------------------------------------------------
// <copyright file="FileInfoWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;
	using System.IO;

	/// <summary>
	/// Represents a class object wrapper for <see cref="T:System.IO.FileInfo"/> class.
	/// </summary>
	[CLSCompliant(false)]
	internal class FileInfoWrap : IFileInfo
	{
		/// <summary>
		/// The file information instance backing.
		/// </summary>
		private readonly System.IO.FileInfo instance;

		/// <summary>
		/// The directory information instance backing.
		/// </summary>
		private IDirectoryInfo directoryInstance;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.IO.FileInfo" /> class, which acts as a wrapper for a file path.
		/// </summary>
		/// <param name="fileName">
		/// The fully qualified name of the new file, or the relative file name. Do not end the path with the directory separator character.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="fileName" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.Security.SecurityException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// The file name is empty, contains only white spaces, or contains invalid characters.
		/// </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		/// Access to <paramref name="fileName" /> is denied.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="fileName" /> contains a colon (:) in the middle of the string.
		/// </exception>
		internal FileInfoWrap(string fileName)
		{
			this.instance = new FileInfo(fileName);
		}

		/// <inheritdoc />
		public IDirectoryInfo Directory =>
			this.directoryInstance ??
			(this.directoryInstance = new DirectoryInfoWrap(this.instance.Directory));

		/// <inheritdoc />
		public bool Exists => this.instance.Exists;

		/// <inheritdoc />
		public string FullName => this.instance.FullName;

		/// <inheritdoc />
		public long Length => this.instance.Length;

		/// <inheritdoc />
		public string Name => this.instance.Name;

		/// <inheritdoc />
		public void Refresh()
		{
			this.instance.Refresh();
		}
	}
}