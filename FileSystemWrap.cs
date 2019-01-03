// ----------------------------------------------------------------------------
// <copyright file="FileSystemWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;

	/// <summary>
	/// Represents a <see cref="System.IO"/> class object wrapper to access and create file system related objects.
	/// </summary>
	[CLSCompliant(false)]
	internal class FileSystemWrap : IFileSystem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileSystemWrap" /> class.
		/// </summary>
		internal FileSystemWrap()
		{
			this.Path = new PathWrap();
			this.Directory = new DirectoryWrap(this.Path);
			this.File = new FileWrap(this.Path);
		}

		/// <inheritdoc />
		public IDirectory Directory
		{
			get;
		}

		/// <inheritdoc />
		public IFile File
		{
			get;
		}

		/// <inheritdoc />
		public IPath Path
		{
			get;
		}

		/// <inheritdoc />
		public IDirectoryInfo CreateDirectoryInfo(string path)
		{
			path = this.Path.NormalizePath(path);
			return new DirectoryInfoWrap(path);
		}

		/// <inheritdoc />
		public IFileInfo CreateFileInfo(string fileName)
		{
			string path = this.Path.NormalizePath(fileName);
			return new FileInfoWrap(path);
		}

		/// <inheritdoc />
		public IFileSystem DeepCopy()
		{
			return new FileSystemWrap();
		}
	}
}