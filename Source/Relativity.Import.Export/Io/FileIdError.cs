// ----------------------------------------------------------------------------
// <copyright file="FileIdError.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	/// <summary>
	/// Represents the file identification error enumeration.
	/// </summary>
	public enum FileIdError
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