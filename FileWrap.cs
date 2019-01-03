// ----------------------------------------------------------------------------
// <copyright file="FileWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;

	/// <summary>
	/// Represents a class object wrapper for <see cref="T:System.IO.File"/> class.
	/// </summary>
	[CLSCompliant(false)]
	internal class FileWrap : IFile
	{
		/// <summary>
		/// The path instance backing.
		/// </summary>
		private readonly IPath instance;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileWrap" /> class.
		/// </summary>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="path"/> is <see langword="null"/>.
		/// </exception>
		internal FileWrap(IPath path)
		{
			this.instance = path ?? throw new ArgumentNullException(nameof(path));
		}

		/// <inheritdoc />
		public void Copy(string sourceFileName, string destFileName, bool overwrite)
		{
			sourceFileName = this.instance.NormalizePath(sourceFileName);
			destFileName = this.instance.NormalizePath(destFileName);
			System.IO.File.Copy(sourceFileName, destFileName, overwrite);
		}

		/// <inheritdoc />
		public System.IO.FileStream Create(string path)
		{
			path = this.instance.NormalizePath(path);
			return System.IO.File.Create(path);
		}

		/// <inheritdoc />
		public System.IO.StreamWriter CreateText(string path)
		{
			path = this.instance.NormalizePath(path);
			return System.IO.File.CreateText(path);
		}

		/// <inheritdoc />
		public void Delete(string path)
		{
			path = this.instance.NormalizePath(path);
			System.IO.File.Delete(path);
		}

		/// <inheritdoc />
		public bool Exists(string path)
		{
			path = this.instance.NormalizePath(path);
			return System.IO.File.Exists(path);
		}
	}
}