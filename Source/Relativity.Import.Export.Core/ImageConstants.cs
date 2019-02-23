// ----------------------------------------------------------------------------
// <copyright file="ImageConstants.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	/// <summary>
	/// Defines image related constants.
	/// </summary>
	public static class ImageConstants
	{
		/// <summary>
		/// The group 3 1-dimensional TIFF encoding.
		/// </summary>
		public const string TiffEncoding3 = "CCITT Group 3 1-Dimensional Modified Huffman";

		/// <summary>
		/// The group 3 FAX TIFF encoding.
		/// </summary>
		public const string TiffEncoding3Fax = "CCITT Group 3 FAX";

		/// <summary>
		/// The group 4 FAX TIFF encoding.
		/// </summary>
		public const string TiffEncoding4Fax = "CCITT Group 4 FAX";

		/// <summary>
		/// The LZW TIFF encoding.
		/// </summary>
		public const string TiffEncodingLzw = "LZW";

		/// <summary>
		/// The JPEG old-style TIFF encoding.
		/// </summary>
		public const string TiffEncodingJpegOld = "JPEG (old-style)";

		/// <summary>
		/// The JPEG new-style TIFF encoding.
		/// </summary>
		public const string TiffEncodingJpegNew = "JPEG (new-style)";

		/// <summary>
		/// The ZLIB TIFF encoding.
		/// </summary>
		public const string TiffEncodingZlib = "Deflate compression, using zlib data format";

		/// <summary>
		/// The pack bits TIFF encoding.
		/// </summary>
		public const string TiffEncodingPackBits = "PackBits";
	}
}