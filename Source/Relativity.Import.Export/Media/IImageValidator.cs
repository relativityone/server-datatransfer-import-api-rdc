// ----------------------------------------------------------------------------
// <copyright file="IImageValidator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Media
{
	/// <summary>
	/// Represents an abstract object to validate an image file.
	/// </summary>
	public interface IImageValidator
	{
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
		void Validate(string file);
	}
}