// ----------------------------------------------------------------------------
// <copyright file="FileTypeIdConfiguration.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;

	/// <summary>
	/// Represents a class object to identify file types. This class cannot be inherited.
	/// </summary>
	internal sealed class FileTypeIdConfiguration : IFileTypeIdConfiguration
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