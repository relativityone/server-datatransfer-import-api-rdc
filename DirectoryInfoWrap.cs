// ----------------------------------------------------------------------------
// <copyright file="DirectoryInfoWrap.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;

	/// <summary>
	/// Represents a class object wrapper for the <see cref="T:System.IO.DirectoryInfo"/> class.
	/// </summary>
	[CLSCompliant(false)]
	internal class DirectoryInfoWrap : IDirectoryInfo
	{
		/// <summary>
		/// The directory information instance backing.
		/// </summary>
		private readonly System.IO.DirectoryInfo instance;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.IO.DirectoryInfo" /> class on the specified path.
		/// </summary>
		/// <param name="path">
		/// A string specifying the path on which to create the <see langword="DirectoryInfo" />.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.Security.SecurityException">
		/// The caller does not have the required permission.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="path" /> contains invalid characters such as ", &lt;, &gt;, or |.
		/// </exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. The specified path, file name, or both are too long.
		/// </exception>
		internal DirectoryInfoWrap(string path)
		{
			this.instance = new System.IO.DirectoryInfo(path);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.IO.DirectoryInfo" /> class on the specified path.
		/// </summary>
		/// <param name="info">
		/// The directory info object.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="info" /> is <see langword="null" />.
		/// </exception>
		internal DirectoryInfoWrap(System.IO.DirectoryInfo info)
		{
			this.instance = info ?? throw new ArgumentNullException(nameof(info));
		}

		/// <inheritdoc />
		public bool Exists => this.instance.Exists;

		/// <inheritdoc />
		public string FullName => this.instance.FullName;

		/// <inheritdoc />
		public string Name => this.instance.Name;

		/// <inheritdoc />
		public void Create()
		{
			this.instance.Create();
		}

		/// <inheritdoc />
		public void Refresh()
		{
			this.instance.Refresh();
		}
	}
}