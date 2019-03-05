// ----------------------------------------------------------------------------
// <copyright file="FileSystemWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;
	using System.IO;
	using System.Text;

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
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "This is an appropriate construction pattern.")]
		public IStreamWriter CreateStreamWriter(string path, bool append)
		{
			path = this.Path.NormalizePath(path);
			return new StreamWriterWrap(this.Path.NormalizePath(path), append, new UTF8Encoding(false, true));
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "This is an appropriate construction pattern.")]
		public IStreamWriter CreateStreamWriter(string path, bool append, System.Text.Encoding encoding)
		{
			path = this.Path.NormalizePath(path);
			return new StreamWriterWrap(this.Path.NormalizePath(path), append, encoding);
		}

		/// <inheritdoc />
		public IFileSystem DeepCopy()
		{
			return new FileSystemWrap();
		}
	}
}