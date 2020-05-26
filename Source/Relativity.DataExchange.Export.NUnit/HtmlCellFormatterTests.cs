// -----------------------------------------------------------------------------------------------------
// <copyright file="HtmlCellFormatterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Service;

	using HtmlCellFormatter = Relativity.DataExchange.Export.HtmlCellFormatter;

    [TestFixture]
	public static class HtmlCellFormatterTests
	{
		[Test]
		public static void ItShouldProperlyTransformToCell()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document);
			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);

			var result = subject.TransformToCell("<>");
			Assert.AreEqual("<td>&lt;&gt;</td>", result);
		}

		[Test]
		public static void ItShouldReturnEmptyImageCellWhenDisabledExportImages()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document)
			{
				ExportImages = false
			};

			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);

			ObjectExportInfo arg = new ObjectExportInfo();
			var result = subject.CreateImageCell(arg);
			Assert.AreEqual(string.Empty, result);
		}

		[Test]
		public static void ItShouldReturnEmptyImageCellWhenArtifactIsNotDocument()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Field)
			{
				ExportImages = true
			};

			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);

			ObjectExportInfo arg = new ObjectExportInfo();
			var newResult = subject.CreateImageCell(arg);
			Assert.AreEqual(string.Empty, newResult);
		}

		[Test]
		public static void ItShouldReturnEmptyTdElementWhenNoImagesPresent()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document)
			{
				ExportImages = true
			};

			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);
			ObjectExportInfo arg = new ObjectExportInfo()
			{
				Images = new ArrayList()
			};
			var newResult = subject.CreateImageCell(arg);
			Assert.AreEqual("<td></td>", newResult);
		}

		[Test]
		public static void ItShouldReturnTdElementWithTransformedPathWhenTransformerIsUsed()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document)
			{
				ExportImages = true,
				VolumeInfo = new VolumeInfo()
				{
					CopyImageFilesFromRepository = true
				}
			};

			var filePathTransformer = new Mock<IFilePathTransformer>();
			filePathTransformer.Setup(x => x.TransformPath(It.IsAny<string>())).Returns(() => "./transformed_path.txt");
			HtmlCellFormatter subject = new HtmlCellFormatter(settings, filePathTransformer.Object);

			ObjectExportInfo arg = new ObjectExportInfo()
			{
				Images = new ArrayList()
				{
					new ImageExportInfo()
					{
						TempLocation = "./path.txt"
					}
				}
			};
			var result = subject.CreateImageCell(arg);
			Assert.AreEqual("<td><a style='display:block' href='./transformed_path.txt'></a></td>", result);
		}

		[Test]
		public static void ItShouldReturnTdElementWithOriginalPathWhenTransformerIsNotUsed()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document)
			{
				ExportImages = true,
				VolumeInfo = new VolumeInfo()
				{
					CopyImageFilesFromRepository = false
				}
			};

			var filePathTransformer = new Mock<IFilePathTransformer>();
			filePathTransformer.Setup(x => x.TransformPath(It.IsAny<string>())).Returns(() => "./fake/path.txt");
			HtmlCellFormatter subject = new HtmlCellFormatter(settings, filePathTransformer.Object);

			ObjectExportInfo arg = new ObjectExportInfo()
			{
				Images = new ArrayList()
				{
					new ImageExportInfo()
					{
						SourceLocation = "./path.txt"
					}
				}
			};
			var result = subject.CreateImageCell(arg);
			Assert.AreEqual("<td><a style='display:block' href='./path.txt'></a></td>", result);
		}

		[TestCase(ExportFile.ImageType.MultiPageTiff)]
		[TestCase(ExportFile.ImageType.Pdf)]
		public static void ItShouldReturnOnlyOneTdElement(ExportFile.ImageType imageType)
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document)
			{
				ExportImages = true,
				TypeOfImage = imageType,
				VolumeInfo = new VolumeInfo()
				{
					CopyImageFilesFromRepository = false
				}
			};

			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);

			ObjectExportInfo arg = new ObjectExportInfo()
			{
				Images = new ArrayList()
				{
					new ImageExportInfo()
					{
						SourceLocation = "./path1.txt"
					},
					new ImageExportInfo()
					{
						SourceLocation = "./path2.txt"
					}
				}
			};
			var result = subject.CreateImageCell(arg);
			Assert.AreEqual("<td><a style='display:block' href='./path1.txt'></a></td>", result);
		}

		[Test]
		public static void ItShouldReturnEmptyNativeCellWhenArtifactIsDocumentAndHasNoNatives()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document);

			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);

			ObjectExportInfo arg = new ObjectExportInfo();
			var result = subject.CreateNativeCell(string.Empty, arg);
			Assert.AreEqual("<td></td>", result);
		}

		[Test]
		public static void ItShouldReturnEmptyNativeCellWhenArtifactIsNotDocumentAndFileIsIsLessThanZero()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Field)
			{
				ExportImages = true
			};

			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);

			ObjectExportInfo arg = new ObjectExportInfo()
			{
				FileID = -1
			};
			var result = subject.CreateNativeCell(string.Empty, arg);
			Assert.AreEqual("<td></td>", result);
		}

		[Test]
		public static void ItShouldReturnValidNativeCell()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Field)
			{
				ExportImages = true
			};

			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);
			ObjectExportInfo arg = new ObjectExportInfo()
			{
				FileID = 1
			};
			var result = subject.CreateNativeCell("location", arg);
			Assert.AreEqual("<td><a style='display:block' href='location'></a></td>", result);
		}

		[Test]
		public static void ItShouldReturnEmptyPdfCellWhenArtifactIsNotDocument()
		{
			// ARRANGE
			ExportFile settings = new ExportFile((int)ArtifactType.Field);
			HtmlCellFormatter formatter = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);
			ObjectExportInfo artifact = new ObjectExportInfo();

			// ACT
			var actual = formatter.CreatePdfCell("location", artifact);

			// ASSERT
			Assert.AreEqual("<td></td>", actual);
		}

		[Test]
		public static void ItShouldReturnEmptyPdfCellWhenArtifactHasNoPdf()
		{
			// ARRANGE
			ExportFile settings = new ExportFile((int)ArtifactType.Document);
			HtmlCellFormatter formatter = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);
			ObjectExportInfo artifact = new ObjectExportInfo { PdfFileGuid = string.Empty };

			// ACT
			var actual = formatter.CreatePdfCell("location", artifact);

			// ASSERT
			Assert.AreEqual("<td></td>", actual);
		}

		[Test]
		public static void ItShouldReturnValidPdfCell()
		{
			// ARRANGE
			const string Location = "location";
			const string FileName = "file";
			ExportFile settings = new ExportFile((int)ArtifactType.Document) { AppendOriginalFileName = false };
			HtmlCellFormatter formatter = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				PdfFileGuid = Guid.NewGuid().ToString(),
				IdentifierValue = FileName
			};

			// ACT
			var actual = formatter.CreatePdfCell("location", artifact);

			// ASSERT
			Assert.AreEqual($"<td><a style='display:block' href='{Location}'>{FileName}.pdf</a></td>", actual);
		}
	}
}