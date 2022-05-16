// ----------------------------------------------------------------------------
// <copyright file="ImageImportWithFileNameDtoBuilder.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Dto
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	using Relativity.DataExchange.Media;
	using Relativity.DataExchange.TestFramework;

	public class ImageImportWithFileNameDtoBuilder
	{
		private readonly string directory;
		private readonly int numberOfDocumentsToImport;
		private readonly int numberOfImagesPerDocument;
		private readonly ImageFormat imageFormat;

		private bool useInvalidDocumentIdentifier;
		private bool generateExtractedTextFiles;
		private int extractedTextLength;
		private Encoding extractedTextEncoding;
		private Action<long> aggregateFileSizeBytes = _ => { };

		public ImageImportWithFileNameDtoBuilder(string directory, int numberOfDocumentsToImport, int numberOfImagesPerDocument, ImageFormat imageFormat)
		{
			this.directory = directory;
			this.numberOfDocumentsToImport = numberOfDocumentsToImport;
			this.numberOfImagesPerDocument = numberOfImagesPerDocument;
			this.imageFormat = imageFormat;
		}

		public IEnumerable<ImageImportWithFileNameDto> Build()
		{
			const int ImageWidth = 200;
			const int ImageHeight = 200;

			for (int documentIndex = 1; documentIndex <= this.numberOfDocumentsToImport; documentIndex++)
			{
				string documentIdentifierPrefix = this.useInvalidDocumentIdentifier ? $"{documentIndex}," : $"{documentIndex}";
				string documentIdentifier = $"{documentIdentifierPrefix}_1";  // document identifier and bates number of the first image has to be equal
				for (int imageIndex = 1; imageIndex <= this.numberOfImagesPerDocument; imageIndex++)
				{
					string batesNumber = $"{documentIdentifierPrefix}_{imageIndex}";
					string fileName = batesNumber;
					FileInfo imageFile = RandomHelper.NextImageFile(this.imageFormat, this.directory, ImageWidth, ImageHeight, fileName);
					aggregateFileSizeBytes(imageFile.Length);
					string fileNameToImport = AddSpecialCharacters(Path.GetFileName(imageFile.FullName));
					if (this.generateExtractedTextFiles)
					{
						this.GenerateExtractedTextFileForImage(imageFile.FullName);
					}

					yield return new ImageImportWithFileNameDto(batesNumber, documentIdentifier, imageFile.FullName, fileNameToImport);
				}
			}
		}

		public ImageImportWithFileNameDtoBuilder WithInvalidDocumentIdentifier()
		{
			this.useInvalidDocumentIdentifier = true;
			return this;
		}

		public ImageImportWithFileNameDtoBuilder WithExtractedText(int length, Encoding encoding)
		{
			this.generateExtractedTextFiles = true;
			this.extractedTextLength = length;
			this.extractedTextEncoding = encoding;
			return this;
		}

		public ImageImportWithFileNameDtoBuilder WithFileSizeBytesAggregator(Action<long> aggregator)
		{
			this.aggregateFileSizeBytes = aggregator;
			return this;
		}

		private static string AddSpecialCharacters(string text)
		{
			return $"ႝ\\ /:*?{text}";
		}

		private void GenerateExtractedTextFileForImage(string imageFilePath)
		{
			const string ExtractedTextFileExtension = ".txt";
			string extractedTextFilePath = Path.ChangeExtension(imageFilePath, ExtractedTextFileExtension);
			RandomHelper.NextTextFile(this.extractedTextLength, this.extractedTextLength, extractedTextFilePath, this.extractedTextEncoding);
		}
	}
}