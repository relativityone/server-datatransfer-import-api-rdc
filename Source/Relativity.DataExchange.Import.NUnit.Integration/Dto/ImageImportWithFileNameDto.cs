// ----------------------------------------------------------------------------
// <copyright file="ImageImportWithFileNameDto.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;

	using Relativity.DataExchange.Media;
	using Relativity.DataExchange.TestFramework;

	public class ImageImportWithFileNameDto : ImageImportDto
	{
		public ImageImportWithFileNameDto(
			string batesNumber,
			string documentIdentifier,
			string fileLocation,
			string fileName)
			: base(batesNumber, documentIdentifier, fileLocation)
		{
			this.FileName = fileName;
		}

		[DisplayName(DefaultImageFieldNames.FileName)]
		public string FileName { get; }

		public static IEnumerable<ImageImportWithFileNameDto> GetRandomImageFiles(string directory, int numberOfDocumentsToImport, int numberOfImagesPerDocument, ImageFormat imageFormat)
		{
			int imageWidth = 200;
			int imageHeight = 200;

			for (int documentIndex = 1; documentIndex <= numberOfDocumentsToImport; documentIndex++)
			{
				string documentIdentifier = documentIndex.ToString();
				for (int imageIndex = 1; imageIndex <= numberOfImagesPerDocument; imageIndex++)
				{
					string batesNumber = $"{documentIdentifier}_{imageIndex}";
					string fileName = batesNumber;
					string fileLocation = RandomHelper.NextImageFile(imageFormat, directory, imageWidth, imageHeight, fileName);
					string fileNameToImport = AddSpecialCharacters(Path.GetFileName(fileLocation));
					yield return new ImageImportWithFileNameDto(batesNumber, documentIdentifier, fileLocation, fileNameToImport);
				}
			}
		}

		public static string AddSpecialCharacters(string text)
		{
			return $"ႝ\\ /:*?{text}";
		}
	}
}
