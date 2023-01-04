// ----------------------------------------------------------------------------
// <copyright file="ImageFormat.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Media
{
	/// <summary>
	/// Specifies the recognized image formats.
	/// </summary>
	/// <remarks>
	/// These values are aligned with the FreeImage library.
	/// </remarks>
	public enum ImageFormat
	{
		/// <summary>
		/// The image format is unsupported.
		/// </summary>
		Unsupported,

		/// <summary>
		/// The JPEG image format.
		/// </summary>
		Jpeg,

		/// <summary>
		/// The TIFF image format.
		/// </summary>
		Tiff,
	}
}