// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryOptions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Defines a set of configurable flags to control retry behavior.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System;

	/// <summary>
	/// Defines a set of configurable flags to control retry behavior.
	/// </summary>
	[Flags]
	public enum RetryOptions
	{
		/// <summary>
		/// No retry is performed.
		/// </summary>
		None = 0,

		/// <summary>
		/// Retry an operation that fails when the file does not exist.
		/// </summary>
		FileNotFound = 1,

		/// <summary>
		/// Retry an operation that fails when the directory does not exist.
		/// </summary>
		DirectoryNotFound = 2,

		/// <summary>
		/// Retry an operation that fails when the disk is full.
		/// </summary>
		DiskFull = 4,

		/// <summary>
		/// Retry an operation that fails when any other I/O error not already listed occurs.
		/// </summary>
		Io = 8,

		/// <summary>
		/// Retry an operation that fails when there are insufficient permissions.
		/// </summary>
		Permissions = 16,

		/// <summary>
		/// Retry all operations that fail.
		/// </summary>
		All = FileNotFound | DirectoryNotFound | DiskFull | Io | Permissions,
	}
}