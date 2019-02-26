// ----------------------------------------------------------------------------
// <copyright file="DirectoryWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
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

		/// <inheritdoc />
		public bool Exists(string path)
		{
			path = this.instance.NormalizePath(path);
			return System.IO.Directory.Exists(path);
		}
	}
}