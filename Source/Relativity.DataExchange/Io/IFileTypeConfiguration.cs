// ----------------------------------------------------------------------------
// <copyright file="IFileTypeConfiguration.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System;

	/// <summary>
	/// Represents an abstract object to configure <see cref="IFileTypeIdentifier"/>.
	/// </summary>
	public interface IFileTypeConfiguration
	{
		/// <summary>
		/// Gets the file identification configuration exception.
		/// </summary>
		/// <value>
		/// The <see cref="Exception"/> instance.
		/// </value>
		Exception Exception
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether a file identification configuration error has occurred.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when an file identification configuration error has occurred; otherwise, <see langword="false" />.
		/// </value>
		bool HasError
		{
			get;
		}

		/// <summary>
		/// Gets the file identification installation directory.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		string InstallDirectory
		{
			get;
		}

		/// <summary>
		/// Gets the file identification timeout in seconds.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		int Timeout
		{
			get;
		}

		/// <summary>
		/// Gets the file identification library version.
		/// </summary>
		/// <value>
		/// The version.
		/// </value>
		string Version
		{
			get;
		}
	}
}