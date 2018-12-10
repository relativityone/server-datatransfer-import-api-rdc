using System.Collections;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity;
using HtmlCellFormatter = kCura.WinEDDS.Core.Export.VolumeManagerV2.HtmlCellFormatter;

namespace kCura.WinEDDS.Core.NUnit.Export
{
	[TestFixture]
	public class HtmlCellFormatterTests
	{
		[Test]
		public void ItShouldProperlyTransformToCell()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document);
			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);

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

			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);

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

			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);

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

			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);
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
		public void ItShouldReturnEmptyNativeCellWhenArtifactIsDocumentAndHasNoNatives()
		{
			ExportFile settings = new ExportFile((int)ArtifactType.Document)
			{
			};

			HtmlCellFormatter subject = new HtmlCellFormatter(settings, new Mock<IFilePathTransformer>().Object);

			ObjectExportInfo arg = new ObjectExportInfo();
			var result = subject.CreateNativeCell("", arg);
			Assert.AreEqual("<td></td>", result);
		}
		
		[Test]
		public void ItShouldReturnEmptyNativeCellWhenArtifactIsNotDocumentAndFileIDLessThan0()
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
			var result = subject.CreateNativeCell("", arg);
			Assert.AreEqual("<td></td>", result);
		}

		[Test]
		public void ItShouldReturnValidNativeCell()
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
	}
}