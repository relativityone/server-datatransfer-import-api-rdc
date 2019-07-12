// ----------------------------------------------------------------------------
// <copyright file="DirectoryWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System;

	/// <summary>
	/// Represents a class object wrapper for the <see cref="T:System.IO.Directory"/> class.
	/// </summary>
	[CLSCompliant(false)]
	internal class DirectoryWrap : IDirectory
	{
		/// <summary>
		/// The path instance backing.
		/// </summary>
		private readonly IPath instance;

		/// <summary>
		/// Initializes a new instance of the <see cref="DirectoryWrap" /> class.
		/// </summary>
		/// <param name="path">
		/// The full path to the directory.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="path"/> is <see langword="null"/>.
		/// </exception>
		internal DirectoryWrap(IPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			this.instance = path;
		}

		/// <inheritdoc />
		public void CreateDirectory(string path)
		{
			path = this.instance.NormalizePath(path);
			System.IO.Directory.CreateDirectory(path);
		}

		/// <inheritdoc />
		public void Delete(string path)
		{
			path = this.instance.NormalizePath(path);
			System.IO.Directory.Delete(path);
		}

		/// <inheritdoc />
		public void Delete(string path, bool recursive)
		{
			path = this.instance.NormalizePath(path);
			System.IO.Directory.Delete(path, recursive);
		}

		/// <inheritdoc />
		public void DeleteIfExists(string path, bool recursive, bool throwOnExistsCheck)
		{
			if (this.Exists(path, throwOnExistsCheck))
			{
				this.Delete(path, recursive);
			}
		}

		/// <inheritdoc />
		public bool Exists(string path)
		{
			path = this.instance.NormalizePath(path);
			return System.IO.Directory.Exists(path);
		}

		/// <inheritdoc />
		public bool Exists(string path, bool throwOnExistsCheck)
		{
			path = this.instance.NormalizePath(path);
			if (!throwOnExistsCheck)
			{
				return System.IO.Directory.Exists(path);
			}

			// System.IO.Directory.Exists will eat any exception that occurs
			// when trying to check if a directory exists, which is not
			// helpful when there's a network problem. GetCreationTimeUtc
			// will except if there's an access issue to the directory in
			// question. If the directory legitimitely doesn't exist, it
			// will return January 1, 1601 (according to http://msdn.microsoft.com/en-us/library/system.io.directory.getcreationtimeutc.aspx)
			return System.IO.Directory.GetCreationTimeUtc(path) != new DateTime(1601, 1, 1);
		}

		/// <inheritdoc />
		public IDirectoryInfo GetParent(string path)
		{
			path = this.instance.NormalizePath(path);
			System.IO.DirectoryInfo directoryInfo = System.IO.Directory.GetParent(path);
			if (directoryInfo == null)
			{
				return null;
			}

			return new DirectoryInfoWrap(directoryInfo);
		}
	}
}