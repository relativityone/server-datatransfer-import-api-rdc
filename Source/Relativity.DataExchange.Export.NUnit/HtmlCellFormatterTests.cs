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
	public class HtmlCellFormatterTests
	{
		private Mock<IFilePathTransformer> _filePathTransformerMock;
		private Mock<IFileNameProvider> _fileNameProviderMock;

		[SetUp]
		public void SetUp()
		{
			_filePathTransformerMock = new Mock<IFilePathTransformer>();
			_fileNameProviderMock = new Mock<IFileNameProvider>();
		}

		[Test]
		public void ItShouldProperlyTransformToCell()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document);
			HtmlCellFormatter subject = CreateSut(settings);

			var result = subject.TransformToCell("<>");
			Assert.AreEqual("<td>&lt;&gt;</td>", result);
		}

		[Test]
		public void ItShouldReturnEmptyImageCellWhenDisabledExportImages()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document)
			{
				ExportImages = false
			};

			HtmlCellFormatter subject = CreateSut(settings);

			ObjectExportInfo arg = new ObjectExportInfo();
			var result = subject.CreateImageCell(arg);
			Assert.AreEqual(string.Empty, result);
		}

		[Test]
		public void ItShouldReturnEmptyImageCellWhenArtifactIsNotDocument()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Field)
			{
				ExportImages = true
			};

			HtmlCellFormatter subject = CreateSut(settings);

			ObjectExportInfo arg = new ObjectExportInfo();
			var newResult = subject.CreateImageCell(arg);
			Assert.AreEqual(string.Empty, newResult);
		}

		[Test]
		public void ItShouldReturnEmptyTdElementWhenNoImagesPresent()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document)
			{
				ExportImages = true
			};

			HtmlCellFormatter subject = CreateSut(settings);
			ObjectExportInfo arg = new ObjectExportInfo()
			{
				Images = new ArrayList()
			};
			var newResult = subject.CreateImageCell(arg);
			Assert.AreEqual("<td></td>", newResult);
		}

		[Test]
		public void ItShouldReturnTdElementWithTransformedPathWhenTransformerIsUsed()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document)
			{
				ExportImages = true,
				VolumeInfo = new VolumeInfo()
				{
					CopyImageFilesFromRepository = true
				}
			};

			_filePathTransformerMock.Setup(x => x.TransformPath(It.IsAny<string>())).Returns(() => "./transformed_path.txt");
			HtmlCellFormatter subject = CreateSut(settings);

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
		public void ItShouldReturnTdElementWithOriginalPathWhenTransformerIsNotUsed()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document)
			{
				ExportImages = true,
				VolumeInfo = new VolumeInfo()
				{
					CopyImageFilesFromRepository = false
				}
			};

			_filePathTransformerMock.Setup(x => x.TransformPath(It.IsAny<string>())).Returns(() => "./fake/path.txt");
			HtmlCellFormatter subject = CreateSut(settings);

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
		public void ItShouldReturnOnlyOneTdElement(ExportFile.ImageType imageType)
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

			HtmlCellFormatter subject = CreateSut(settings);

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
		public void ItShouldReturnEmptyNativeCellWhenArtifactIsDocumentAndHasNoNatives()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document);

			HtmlCellFormatter subject = CreateSut(settings);

			ObjectExportInfo arg = new ObjectExportInfo();
			var result = subject.CreateNativeCell(string.Empty, arg);
			Assert.AreEqual("<td></td>", result);
		}

		[Test]
		public void ItShouldReturnEmptyNativeCellWhenArtifactIsNotDocumentAndFileIsIsLessThanZero()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Field)
			{
				ExportImages = true
			};

			HtmlCellFormatter subject = CreateSut(settings);

			ObjectExportInfo arg = new ObjectExportInfo()
			{
				FileID = -1
			};
			var result = subject.CreateNativeCell(string.Empty, arg);
			Assert.AreEqual("<td></td>", result);
		}

		[Test]
		public void ItShouldReturnValidNativeCell()
		{
			// arrange
			const string FileName = "test.docx";
			ExportFile settings = new ExportFile((int)ArtifactType.Field)
			{
				ExportImages = true
			};

			HtmlCellFormatter subject = CreateSut(settings);
			ObjectExportInfo arg = new ObjectExportInfo()
			{
				FileID = 1
			};
			this._fileNameProviderMock.Setup(x => x.GetName(arg)).Returns(FileName);

			// act
			var result = subject.CreateNativeCell("location", arg);

			// assert
			Assert.AreEqual("<td><a style='display:block' href='location'>test.docx</a></td>", result);
		}

		[Test]
		public void ItShouldReturnEmptyPdfCellWhenArtifactIsNotDocument()
		{
			// ARRANGE
			ExportFile settings = new ExportFile((int)ArtifactType.Field);
			HtmlCellFormatter formatter = CreateSut(settings);
			ObjectExportInfo artifact = new ObjectExportInfo();

			// ACT
			var actual = formatter.CreatePdfCell("location", artifact);

			// ASSERT
			Assert.AreEqual("<td></td>", actual);
		}

		[Test]
		public void ItShouldReturnEmptyPdfCellWhenArtifactHasNoPdf()
		{
			// ARRANGE
			ExportFile settings = new ExportFile((int)ArtifactType.Document);
			HtmlCellFormatter formatter = CreateSut(settings);
			ObjectExportInfo artifact = new ObjectExportInfo { PdfFileGuid = string.Empty };

			// ACT
			var actual = formatter.CreatePdfCell("location", artifact);

			// ASSERT
			Assert.AreEqual("<td></td>", actual);
		}

		[Test]
		public void ItShouldReturnValidPdfCell()
		{
			// ARRANGE
			const string Location = "location";
			const string FileName = "file.pdf";
			ExportFile settings = new ExportFile((int)ArtifactType.Document) { AppendOriginalFileName = false };
			HtmlCellFormatter formatter = CreateSut(settings);
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				PdfFileGuid = Guid.NewGuid().ToString(),
				IdentifierValue = "ItShouldNotBeUsed"
			};

			this._fileNameProviderMock.Setup(x => x.GetPdfName(artifact)).Returns(FileName);

			// ACT
			var actual = formatter.CreatePdfCell("location", artifact);

			// ASSERT
			Assert.AreEqual($"<td><a style='display:block' href='{Location}'>{FileName}</a></td>", actual);
		}

		private HtmlCellFormatter CreateSut(ExportFile settings)
		{
			return new HtmlCellFormatter(settings, _filePathTransformerMock.Object, _fileNameProviderMock.Object);
		}
	}
}