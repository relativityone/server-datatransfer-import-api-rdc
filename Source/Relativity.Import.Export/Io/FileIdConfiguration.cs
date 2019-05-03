// ----------------------------------------------------------------------------
// <copyright file="FileIdConfiguration.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;

	/// <summary>
	/// Represents the file identification configuration class object. This class cannot be inherited.
	/// </summary>
	public sealed class FileIdConfiguration
	{
		/// <summary>
		/// Gets the file identification configuration exception.
		/// </summary>
		/// <value>
		/// The <see cref="Exception"/> instance.
		/// </value>
		public Exception Exception
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets a value indicating whether a file identification configuration error has occurred.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when an file identification configuration error has occurred; otherwise, <see langword="false" />.
		/// </value>
		public bool HasError
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the file identification installation directory.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public string InstallDirectory
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the file identification timeout in seconds.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		public int Timeout
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the file identification library version.
		/// </summary>
		/// <value>
		/// The version.
		/// </value>
		public string Version
		{
			get;
			internal set;
		}
	}
}