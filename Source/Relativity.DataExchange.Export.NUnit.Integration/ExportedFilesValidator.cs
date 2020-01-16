// <copyright file="ExportedFilesValidator.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System.IO;
	using System.Linq;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	public static class ExportedFilesValidator
	{
		private const string NativesSubFolder = "NATIVES";
		private const string ImagesSubFolder = "IMAGES";

		public static void ValidateImagesCount(ExtendedExportFile exportFile, int originalImagesCount)
		{
			int expectedImageCount = 0;

			if (exportFile.ExportImages)
			{
				expectedImageCount = originalImagesCount;
				if (exportFile.TypeOfImage != ExportFile.ImageType.SinglePage)
				{
					// the export tests assume we import images only for one document (originalImagesCount).
					// For Pdf and Multi-Tiff export we eventually get only 1 file as the image files merge process outcome
					expectedImageCount = 1;
				}
			}

			ValidateFileCount(exportFile, expectedImageCount, ImagesSubFolder);
		}

		public static void ValidateNativesCount(ExtendedExportFile exportFile, int originalNativesCount)
		{
			int expectedNativesCount = 0;

			if (exportFile.ExportNative)
			{
				expectedNativesCount = originalNativesCount;
			}

			ValidateFileCount(exportFile, expectedNativesCount, NativesSubFolder);
		}

		private static void ValidateFileCount(ExtendedExportFile exportFile, int expectedFileCount, string subFolderName)
		{
			DirectoryInfo[] exportDirectories = new DirectoryInfo(exportFile.FolderPath)
				.GetDirectories("*", SearchOption.AllDirectories);

			int actualFileCount = 0;
			foreach (var folder in exportDirectories.Where(subDir => string.Equals(subDir.Name, subFolderName)))
			{
				actualFileCount = folder.GetFiles("*.*", SearchOption.AllDirectories).Length;
			}

			Assert.That(actualFileCount, Is.EqualTo(expectedFileCount), $"Actual number of files in '{subFolderName}' folder is {actualFileCount} and expected number of files is {expectedFileCount} ");
		}
	}
}
