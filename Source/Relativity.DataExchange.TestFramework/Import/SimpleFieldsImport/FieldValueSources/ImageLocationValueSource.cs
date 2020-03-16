// // ----------------------------------------------------------------------------
// <copyright file="ImageLocationValueSource.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources
{
	using System;
	using System.Collections;

	using Relativity.DataExchange.Media;

	[Serializable]
	public class ImageLocationValueSource : IFieldValueSourceWithPrefix
	{
		public ImageLocationValueSource(string directory, ImageFormat imageFormat)
			: this(directory, "IAPI-test", imageFormat, 1)
		{
		}

		public ImageLocationValueSource(string directory, ImageFormat imageFormat, int numberOfImagesPerDocument)
			: this(directory, "IAPI-test", imageFormat, numberOfImagesPerDocument)
		{
		}

		public ImageLocationValueSource(string directory, string fileNamePrefix, ImageFormat imageFormat, int numberOfImagesPerDocument)
		{
			this.Directory = directory;
			this.FileNamePrefix = fileNamePrefix;
			this.ImageFormat = imageFormat;
			this.NumberOfImagesPerDocument = numberOfImagesPerDocument;
		}

		public string Directory { get; }

		public string FileNamePrefix { get; }

		public ImageFormat ImageFormat { get; }

		public int NumberOfImagesPerDocument { get; }

		public IEnumerable CreateValuesEnumerator()
		{
			const int ImageWidth = 200;
			const int ImageHeight = 200;

			for (int documentIndex = 0; ; documentIndex++)
			{
				for (int imageIndex = 0; imageIndex < this.NumberOfImagesPerDocument; imageIndex++)
				{
					string fileName = $"{this.FileNamePrefix}_{documentIndex}_{imageIndex}";
					string imageLocation = RandomHelper.NextImageFile(this.ImageFormat, this.Directory, ImageWidth, ImageHeight, fileName);
					yield return imageLocation;
				}
			}
		}

		public IFieldValueSourceWithPrefix CreateFieldValueSourceWithPrefix(string prefix)
		{
			return new ImageLocationValueSource(this.Directory, prefix, this.ImageFormat, this.NumberOfImagesPerDocument);
		}
	}
}