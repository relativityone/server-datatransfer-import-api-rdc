// ----------------------------------------------------------------------------
// <copyright file="FileTypeIdentifyError.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	/// <summary>
	/// Represents the errors that can occur when attempting to identify a file type.
	/// </summary>
	public enum FileTypeIdentifyError
	{
		/// <summary>
		/// The error hasn't been assigned.
		/// </summary>
		None,

		/// <summary>
		/// The file cannot be identified because it doesn't exist.
		/// </summary>
		FileNotFound,

		/// <summary>
		/// The file cannot be identified because there aren't sufficient permissions.
		/// </summary>
		Permissions,

		/// <summary>
		/// The file cannot be identified because of an I/O error.
		/// </summary>
		Io,
	}
}