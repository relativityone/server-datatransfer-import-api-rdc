// <copyright file="ExportedFilesValidator.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Collections.Generic;
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
		private const string SearchablePdfSubFolder = "PDF";

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

		public static void ValidateSearchablePdfsCount(ExtendedExportFile exportFile, int originalSearchablePdfsCount)
		{
			exportFile = exportFile ?? throw new ArgumentNullException(nameof(exportFile));
			int expectedSearchablePdfsCount = 0;

			if (exportFile.ExportPdf && exportFile.VolumeInfo.CopyPdfFilesFromRepository)
			{
				expectedSearchablePdfsCount = originalSearchablePdfsCount;
			}

			ValidateFileCount(exportFile, expectedSearchablePdfsCount, SearchablePdfSubFolder);
		}

		public static void ValidateSearchablePdfSubDirectoryPrefix(ExportFile exportFile)
		{
			exportFile = exportFile ?? throw new ArgumentNullException(nameof(exportFile));

			if (!exportFile.ExportPdf || !exportFile.VolumeInfo.CopyPdfFilesFromRepository)
			{
				return;
			}

			List<FileInfo> actualSearchablePdfFiles = GetFilesFromSubFolder(exportFile.FolderPath, SearchablePdfSubFolder);
			foreach (FileInfo pdfFile in actualSearchablePdfFiles)
			{
				StringAssert.StartsWith(exportFile.VolumeInfo.get_SubdirectoryPdfPrefix(false) ?? string.Empty, pdfFile.Directory?.Name);
			}
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

		public static Task ValidateSearchablePdfFilesAsync(ExportFile exportFile)
		{
			exportFile = exportFile ?? throw new ArgumentNullException(nameof(exportFile));

			if (!exportFile.ExportPdf || !exportFile.VolumeInfo.CopyPdfFilesFromRepository)
			{
				return Task.CompletedTask;
			}

			List<FileInfo> actualSearchablePdfFiles = GetFilesFromSubFolder(exportFile.FolderPath, SearchablePdfSubFolder);
			return ValidateFilesAsync(actualSearchablePdfFiles, new SearchablePdfFileHashValidator());
		}

		public static async Task ValidateFileStringContentAsync(string filePath, string expectedContent, string extension = null, Encoding loadFileEncoding = null)
		{
			var validator = new StringContentValidator(expectedContent, extension, loadFileEncoding);

			if (!await validator.IsValidAsync(filePath).ConfigureAwait(false))
			{
				// Files should be copied to TestReports folder to be stored with other tests results from pipeline
				string[] storedFiles = StoreCurrentAndExpectedFileInExportTestReportFolder(filePath, expectedContent);
				Assert.Fail($"File '{filePath}' content differs from expected. Both exported and expected files saved in '{storedFiles[0]}' and {storedFiles[1]}");
			}
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

			Encoding fileEncoding = GetFileEncoding(currentFilePath);

			string fileName = Path.GetFileNameWithoutExtension(currentFilePath) + "_" + DateTime.Now.Hour + "_"
			                  + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_current" + Path.GetExtension(currentFilePath);

			string storedCurrentFilePath = Path.Combine(reportFolder, fileName);
			string storedExpectedFilePath = Path.Combine(reportFolder, fileName).Replace("current", "expected");

			File.Move(currentFilePath, storedCurrentFilePath);
			File.WriteAllText(storedExpectedFilePath, expectedFileContent, fileEncoding);

			return new[] { storedCurrentFilePath, storedExpectedFilePath };
		}

		private static string GetPathToExportTestReportFolder()
		{
			string parentFolder = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent.Parent.Parent.FullName;
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
