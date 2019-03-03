// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrimOption.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Represents the supported file delimiter modes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	/// <summary>
	/// Represents the supported whitespace trip options.
	/// </summary>
	internal enum TrimOption
	{
		/// <summary>
		/// No whitespace is trimmed.
		/// </summary>
		None,

		/// <summary>
		/// Only the leading whitespace is trimmed.
		/// </summary>
		Leading,

		/// <summary>
		/// Only the trailing whitespace is trimmed.
		/// </summary>
		Trailing,

		/// <summary>
		/// Both leading and trailing whitespace is trimmed.
		/// </summary>
		Both
	}
}