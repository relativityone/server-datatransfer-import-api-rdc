// ----------------------------------------------------------------------------
// <copyright file="IImageIdService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Media
{
	/// <summary>
	/// Represents an abstract service to identify an image file.
	/// </summary>
	public interface IImageIdService
	{
		/// <summary>
		/// Identifies the specified image file format.
		/// </summary>
		/// <param name="file">
		/// The full path to the image file.
		/// </param>
		/// <returns>
		/// The <see cref="ImageFormat"/> value.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when <paramref name="file" /> is <see langword="null" /> or empty.
		/// </exception>
		/// <exception cref="System.IO.FileNotFoundException">
		/// The exception thrown when the file doesn't exist.
		/// </exception>
		/// <exception cref="ImageValidationException">
		/// The exception thrown when the image file cannot be identified.
		/// </exception>
		ImageFormat Identify(string file);

		/// <summary>
		/// Validates the specified image file and throw an exception for any unsupported image format.
		/// </summary>
		/// <param name="file">
		/// The full path to the image file.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when <paramref name="file" /> is <see langword="null" /> or empty.
		/// </exception>
		/// <exception cref="System.IO.FileNotFoundException">
		/// The exception thrown when the file doesn't exist.
		/// </exception>
		/// <exception cref="ImageValidationException">
		/// The exception thrown when the image file cannot be validated.
		/// </exception>
		void Validate(string file);
	}
}