// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageConversionServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="IImageConversionService"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.Import.Export;
	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents <see cref="IImageConversionService"/> tests.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class ImageConversionServiceTests
	{
		private static readonly List<string> CandidateJpegImages = new List<string>();
		private static readonly List<string> CandidatePngImages = new List<string>();
		private static readonly List<string> CandidateTiffImages = new List<string>();
		private IImageConversionService service;
		private TempDirectory tempDirectory;

		[SetUp]
		public void Setup()
		{
			this.tempDirectory = new TempDirectory();
			this.tempDirectory.Create();
			this.service = new ImageConversionService();
		}

		[TearDown]
		public void Teardown()
		{
			this.tempDirectory.Dispose();
		}

		[Test]
		public void ShouldThrowWhenTheConstructorArgsAreInvalid()
		{
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						this.service.ConvertImagesToMultiPagePdf(null, "a");
					});

			Assert.Throws<ArgumentNullException>(
				() =>
					{
						this.service.ConvertImagesToMultiPagePdf(new[] { "a" }, null);
					});
			Assert.Throws<ArgumentOutOfRangeException>(
				() =>
					{
						this.service.ConvertImagesToMultiPagePdf(new string[] { }, "a");
					});
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						this.service.ConvertTiffsToMultiPageTiff(null, "a");
					});

			Assert.Throws<ArgumentNullException>(
				() =>
					{
						this.service.ConvertTiffsToMultiPageTiff(new[] { "a" }, null);
					});
			Assert.Throws<ArgumentOutOfRangeException>(
				() =>
					{
						this.service.ConvertTiffsToMultiPageTiff(new string[] { }, "a");
					});
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						this.service.GetPdfPageCount(string.Empty);
					});
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						this.service.GetPdfPageCount(null);
					});
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						this.service.GetTiffImageCount(string.Empty);
					});
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						this.service.GetTiffImageCount(null);
					});
		}

		[Test]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		[Category(TestCategories.Framework)]
		public void ShouldConvertTheTiffImagesToMultiPageTiff(int frameCount)
		{
			string outputFile = this.GetTempFilePath(".tif");
			IList<string> inputFiles = GetTiffInputFiles(frameCount);
			this.service.ConvertTiffsToMultiPageTiff(inputFiles, outputFile);
			int actualCount = this.service.GetTiffImageCount(outputFile);
			Assert.That(actualCount, Is.EqualTo(frameCount));
		}

		[Test]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		[Category(TestCategories.Framework)]
		public void ShouldConvertTheImagesToMultiPagePdf(int pageCount)
		{
			string outputFile = this.GetTempFilePath(".pdf");
			IList<string> inputFiles = GetPdfInputFiles(pageCount);
			this.service.ConvertImagesToMultiPagePdf(inputFiles, outputFile);
			int actualCount = this.service.GetPdfPageCount(outputFile);
			Assert.That(actualCount, Is.EqualTo(pageCount));
		}

		private static void GetAllCandidateFiles()
		{
			if (CandidateJpegImages.Count == 0)
			{
				CandidateJpegImages.AddRange(ResourceFileHelper.GetResourceFolderFiles("Jpeg"));
			}

			if (CandidatePngImages.Count == 0)
			{
				CandidatePngImages.AddRange(ResourceFileHelper.GetResourceFolderFiles("Png"));
			}

			if (CandidateTiffImages.Count == 0)
			{
				CandidateTiffImages.AddRange(ResourceFileHelper.GetResourceFolderFiles("Tiff"));
			}
		}

		private static IList<string> GetPdfInputFiles(int pageCount)
		{
			GetAllCandidateFiles();
			List<string> candidateFiles = new List<string>();
			candidateFiles.AddRange(CandidateJpegImages);
			candidateFiles.AddRange(CandidatePngImages);
			candidateFiles.AddRange(CandidateTiffImages);
			IList<string> inputFileNames = new List<string>();
			for (int i = 0; i < pageCount; i++)
			{
				int index = RandomHelper.NextInt32(0, candidateFiles.Count - 1);
				inputFileNames.Add(candidateFiles[index]);
			}

			return inputFileNames;
		}

		private static IList<string> GetTiffInputFiles(int frameCount)
		{
			List<string> candidateFiles = new List<string>();
			candidateFiles.AddRange(CandidateTiffImages);
			IList<string> inputFileNames = new List<string>();
			for (int i = 0; i < frameCount; i++)
			{
				int index = RandomHelper.NextInt32(0, candidateFiles.Count - 1);
				inputFileNames.Add(candidateFiles[index]);
			}

			return GetInputFiles("Tiff", inputFileNames);
		}

		private static IList<string> GetInputFiles(string folder, IEnumerable<string> fileNames)
		{
			return fileNames.Select(x => ResourceFileHelper.GetResourceFilePath(folder, x)).ToList();
		}

		private string GetTempFilePath(string extension)
		{
			string filename = System.Guid.NewGuid().ToString("D").ToUpperInvariant();
			filename = System.IO.Path.ChangeExtension(filename, extension);
			return System.IO.Path.Combine(this.tempDirectory.Directory, filename);
		}
	}
}