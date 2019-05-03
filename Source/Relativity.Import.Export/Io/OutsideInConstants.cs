// ----------------------------------------------------------------------------
// <copyright file="OutsideInConstants.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	/// <summary>
	/// Defines Outside In constants.
	/// </summary>
	internal static class OutsideInConstants
	{
		/// <summary>
		/// The OI error code that doesn't represent an actual error.
		/// </summary>
		public const int NoErrorCode = 0;

		/// <summary>
		/// The OI error code that represents a file not found error.
		/// </summary>
		public const int FileNotFoundErrorCode = 58;

		/// <summary>
		/// The OI error code that represents a file permission error.
		/// </summary>
		public const int FilePermissionErrorCode = 7;
	}
}