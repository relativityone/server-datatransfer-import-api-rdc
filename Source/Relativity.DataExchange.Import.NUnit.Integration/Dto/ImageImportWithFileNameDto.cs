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
			return GetRandomImageFiles(directory, numberOfDocumentsToImport, numberOfImagesPerDocument, imageFormat, fileSize => { });
		}

		public static IEnumerable<ImageImportWithFileNameDto> GetRandomImageFiles(string directory, int numberOfDocumentsToImport, int numberOfImagesPerDocument, ImageFormat imageFormat, Action<long> aggregateFileSizeBytes)
		{
			return GetRandomImageFiles(directory, numberOfDocumentsToImport, numberOfImagesPerDocument, imageFormat, false, aggregateFileSizeBytes);
		}

		public static IEnumerable<ImageImportWithFileNameDto> GetRandomImageFiles(string directory, int numberOfDocumentsToImport, int numberOfImagesPerDocument, ImageFormat imageFormat, bool useInvalidIdentifier, Action<long> aggregateFileSizeBytes)
		{
			const int imageWidth = 200;
			const int imageHeight = 200;

			for (int documentIndex = 1; documentIndex <= numberOfDocumentsToImport; documentIndex++)
			{
				string documentIdentifier = useInvalidIdentifier ? $"{documentIndex}," : $"{documentIndex}";
				for (int imageIndex = 1; imageIndex <= numberOfImagesPerDocument; imageIndex++)
				{
					string batesNumber = $"{documentIdentifier}_{imageIndex}";
					string fileName = batesNumber;
					FileInfo imageFile = RandomHelper.NextImageFile(imageFormat, directory, imageWidth, imageHeight, fileName);
					aggregateFileSizeBytes(imageFile.Length);
					string fileNameToImport = AddSpecialCharacters(Path.GetFileName(imageFile.FullName));
					yield return new ImageImportWithFileNameDto(batesNumber, documentIdentifier, imageFile.FullName, fileNameToImport);
				}
			}
		}

		public static string AddSpecialCharacters(string text)
		{
			return $"ႝ\\ /:*?{text}";
		}
	}
}
