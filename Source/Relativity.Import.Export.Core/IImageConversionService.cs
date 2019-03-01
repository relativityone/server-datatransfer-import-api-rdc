// ----------------------------------------------------------------------------
// <copyright file="IImageConversionService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System.Collections.Generic;

	/// <summary>
	/// Represents an abstract service to convert images to their multi-page representation.
	/// </summary>
	public interface IImageConversionService
	{
		/// <summary>
		/// Converts the collection of TIFF images to their multi-page representation.
		/// </summary>
		/// <param name="inputFiles">
		/// The list of TIFF input file paths to convert.
		/// </param>
		/// <param name="outputFile">
		/// The path of the resulting output file.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when <paramref name="inputFiles" /> or <paramref name="outputFile" /> is <see langword="null" /> or empty.
		/// </exception>
		/// <exception cref="ConvertToMultiPageTiffException">
		/// Thrown when an error occurs during conversion.
		/// </exception>
		/// <exception cref="ImageRollupException">
		/// Thrown when a generic image error occurs during conversion.
		/// </exception>
		void ConvertTiffsToMultiPageTiff(IEnumerable<string> inputFiles, string outputFile);

		/// <summary>
		/// Converts the collection of images to their multi-page PDF representation.
		/// </summary>
		/// <param name="inputFiles">
		/// The list of image input file paths to convert.
		/// </param>
		/// <param name="outputFile">
		/// The path of the resulting output file.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when <paramref name="inputFiles" /> or <paramref name="outputFile" /> is <see langword="null" /> or empty.
		/// </exception>
		/// <exception cref="ConvertToMultiPagePdfException">
		/// Thrown when an error occurs during conversion.
		/// </exception>
		/// <exception cref="ImageRollupException">
		/// Thrown when a generic image error occurs during conversion.
		/// </exception>
		void ConvertImagesToMultiPagePdf(IEnumerable<string> inputFiles, string outputFile);

		/// <summary>
		/// Gets the number of images contained within a single TIFF image.
		/// </summary>
		/// <param name="file">
		/// The full path to the TIFF image.
		/// </param>
		/// <returns>
		/// The total number of images.
		/// </returns>
		int GetTiffImageCount(string file);

		/// <summary>
		/// Gets the number of pages contained within a single PDF file.
		/// </summary>
		/// <param name="file">
		/// The full path to the PDF file.
		/// </param>
		/// <returns>
		/// The total number of images.
		/// </returns>
		int GetPdfPageCount(string file);
	}
}