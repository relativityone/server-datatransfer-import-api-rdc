// <copyright file="ExportedFilesValidator.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Threading.Tasks;

	using global::NUnit.Framework;
	using kCura.WinEDDS;

	using Relativity.DataExchange.TestFramework.Validation;

	using FileInfo = System.IO.FileInfo;

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

		public static Task ValidateNativeFilesAsync(ExportFile exportFile)
		{
			exportFile = exportFile ?? throw new ArgumentNullException(nameof(exportFile));

			if (!exportFile.ExportNative)
			{
				return Task.CompletedTask;
			}

			List<FileInfo> actualNativeFiles = GetFilesFromSubFolder(exportFile.FolderPath, NativesSubFolder);
			return ValidateFilesAsync(actualNativeFiles, new NativeFileHashValidator());
		}

		public static Task ValidateImageFilesAsync(ExportFile exportFile)
		{
			exportFile = exportFile ?? throw new ArgumentNullException(nameof(exportFile));

			if (!exportFile.ExportImages)
			{
				return Task.CompletedTask;
			}

			List<FileInfo> actualImageFiles = GetFilesFromSubFolder(exportFile.FolderPath, ImagesSubFolder);
			IFileValidator fileValidator;
			switch (exportFile.TypeOfImage)
			{
				case ExportFile.ImageType.SinglePage:
					fileValidator = new ImageSinglePageFileHashValidator();
					break;
				case ExportFile.ImageType.MultiPageTiff:
					fileValidator = new ImageMultiPageFileHashValidator();
					break;
				case ExportFile.ImageType.Pdf:
					{
						const int NumberOfDifferentBytesInPdfFiles = 69;
						fileValidator = new PdfFileValidator(NumberOfDifferentBytesInPdfFiles);
						break;
					}

				default:
					throw new ArgumentException($"incorrect image type: {exportFile.TypeOfImage}");
			}

			return ValidateFilesAsync(actualImageFiles, fileValidator);
		}

		public static void ValidateFileContent(string filePath, string expectedContent)
		{
			CultureInfo cultureInfo = CultureInfo.CurrentCulture;
			string currentContent = GetFileContent(filePath).ToLower(cultureInfo).TrimEnd();
			string formattedExpectedContent = expectedContent == null ? string.Empty : expectedContent.ToLower(cultureInfo).TrimEnd();

			if (formattedExpectedContent != currentContent)
			{
				// Files should be copied to TestReports folder to be stored with other tests results from pipeline
				string[] storedFiles = StoreCurrentAndExpectedFileInExportTestReportFolder(filePath, formattedExpectedContent);
				Assert.AreEqual(formattedExpectedContent, currentContent, $"File '{filePath}' content differs from expected. Both exported and expected files saved in '{storedFiles[0]}' and {storedFiles[1]}");
			}
		}

		public static void ValidateFileEncoding(string filePath, Encoding expectedEncoding)
		{
			Encoding currentEncoding = GetFileEncoding(filePath);
			Assert.AreEqual(expectedEncoding, currentEncoding, $"File '{filePath}' encoding is '{currentEncoding}', should be '{expectedEncoding}'");
		}

		public static void ValidateFileExtension(string filePath, string expectedExtension)
		{
			string currentExtension = GetFileExtension(filePath);
			Assert.AreEqual(expectedExtension, currentExtension, $"File '{filePath}' extension is '{currentExtension}', should be '{expectedExtension}'");
		}

		private static string GetFileExtension(string filePath)
		{
			return Path.GetExtension(filePath).Replace(".", string.Empty);
		}

		private static string GetFileContent(string filePath)
		{
			return File.ReadAllText(filePath);
		}

		private static Encoding GetFileEncoding(string filePath)
		{
			using (StreamReader streamReader = new StreamReader(filePath, Encoding.Default, true))
			{
				streamReader.Peek();
				return streamReader.CurrentEncoding;
			}
		}

		private static string[] StoreCurrentAndExpectedFileInExportTestReportFolder(string currentFilePath, string expectedFileContent)
		{
			string reportFolder = GetPathToExportTestReportFolder();
			Directory.CreateDirectory(reportFolder);

			string fileExtension = GetFileExtension(currentFilePath);
			Encoding fileEncoding = GetFileEncoding(currentFilePath);

			string fileName = Path.GetFileNameWithoutExtension(currentFilePath) + "_" + DateTime.Now.Hour + "_"
			                  + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_current." + fileExtension;

			string storedCurrentFilePath = Path.Combine(reportFolder, fileName);
			string storedExpectedFilePath = Path.Combine(reportFolder, fileName).Replace("current", "expected");

			File.Move(currentFilePath, storedCurrentFilePath);
			File.WriteAllText(storedExpectedFilePath, expectedFileContent, fileEncoding);

			return new string[] { storedCurrentFilePath, storedExpectedFilePath };
		}

		private static string GetPathToExportTestReportFolder()
		{
			string parentFolder = new System.IO.FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent.Parent.Parent.FullName;
			string reportFolder = Path.Combine(parentFolder, "TestReports", "integration-tests", "ExportTests");
			return reportFolder;
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

		private static async Task ValidateFilesAsync(List<FileInfo> actualFiles, IFileValidator fileValidator)
		{
			var results = actualFiles.Select(x => fileValidator.IsValidAsync(x.FullName))
				.ToList();
			await Task.WhenAll(results).ConfigureAwait(false);
			var zip = actualFiles.Zip(results, (fileInfo, task) => new { fileInfo.FullName, task.Result });

			foreach (var file in zip)
			{
				Assert.True(file.Result, $"Actual file {file.FullName} is different than expected");
			}
		}

		private static List<FileInfo> GetFilesFromSubFolder(string path, string subFolderName)
		{
			List<FileInfo> files = new List<FileInfo>();

			DirectoryInfo[] exportDirectories = new DirectoryInfo(path)
				.GetDirectories("*", SearchOption.TopDirectoryOnly);
			foreach (var folder in exportDirectories)
			{
				var nativeDir = new DirectoryInfo(Path.Combine(folder.FullName, subFolderName));
				if (nativeDir.Exists)
				{
					files.AddRange(nativeDir.GetFiles("*", SearchOption.AllDirectories));
				}
			}

			return files;
		}
	}
}
