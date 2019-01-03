// ----------------------------------------------------------------------------
// <copyright file="FileSystem.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;

	/// <summary>
	/// Defines a singleton file system instance. This should be constructor injected where file system API usage is required.
	/// </summary>
	[CLSCompliant(false)]
	public static class FileSystem
	{
		/// <summary>
		/// Gets the single <see cref="IFileSystem"/> instance.
		/// </summary>
		/// <value>
		/// The <see cref="IFileSystem"/> instance.
		/// </value>
		public static IFileSystem Instance => new FileSystemWrap();
	}
}