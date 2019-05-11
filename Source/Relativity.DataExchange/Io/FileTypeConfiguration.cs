// ----------------------------------------------------------------------------
// <copyright file="FileTypeConfiguration.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System;

	/// <summary>
	/// Represents a class object to configure <see cref="IFileTypeIdentifier"/>. This class cannot be inherited.
	/// </summary>
	internal sealed class FileTypeConfiguration : IFileTypeConfiguration
	{
		/// <inheritdoc />
		public Exception Exception
		{
			get;
			internal set;
		}

		/// <inheritdoc />
		public bool HasError
		{
			get;
			internal set;
		}

		/// <inheritdoc />
		public string InstallDirectory
		{
			get;
			internal set;
		}

		/// <inheritdoc />
		public int Timeout
		{
			get;
			internal set;
		}

		/// <inheritdoc />
		public string Version
		{
			get;
			internal set;
		}
	}
}