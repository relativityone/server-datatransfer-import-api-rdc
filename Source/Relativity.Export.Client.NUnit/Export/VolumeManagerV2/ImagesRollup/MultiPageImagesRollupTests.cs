using System;
using System.Collections;
using System.IO;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.ImagesRollup
{
	[TestFixture]
	public abstract class MultiPageImagesRollupTests
	{
		private MultiPageImagesRollup _instance;

		private Mock<IImage> _imageConverter;
		private Mock<IStatus> _status;
		private Mock<IFileHelper> _fileHelper;
		private ExportFile _exportSettings;

		[SetUp]
		public void SetUp()
		{
			_fileHelper = new Mock<IFileHelper>();
			_status = new Mock<IStatus>();
			_imageConverter = new Mock<IImage>();

			_exportSettings = new ExportFile(1)
			{
				FolderPath = "folder_path"
			};
			_instance = CreateInstance(_exportSettings, _fileHelper.Object, _status.Object, _imageConverter.Object);
		}

		protected abstract MultiPageImagesRollup CreateInstance(ExportFile exportSettings, IFileHelper fileHelper, IStatus status, IImage imageConverter);

		protected abstract void AssertImageConverterCall(Mock<IImage> imageConverter, ObjectExportInfo artifact);

		protected abstract string Extension();

		[Test]
		public void ItShouldSkipOnEmptyList()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList()
			};

			//ACT & ASSERT
			Assert.DoesNotThrow(() => _instance.RollupImages(artifact));
		}

		[Test]
		public void ItShouldRemoveImageFilesAfterRollup()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList()
			};

			string image1Location = "image_temp_location_1";
			string image2Location = "image_temp_location_2";

			ImageExportInfo image1 = new ImageExportInfo
			{
				TempLocation = image1Location
			};
			ImageExportInfo image2 = new ImageExportInfo
			{
				TempLocation = image2Location
			};
			artifact.Images.Add(image1);
			artifact.Images.Add(image2);

			//ACT
			_instance.RollupImages(artifact);

			//ASSERT
			_fileHelper.Verify(x => x.Delete(image1Location), Times.Once);
			_fileHelper.Verify(x => x.Delete(image2Location), Times.Once);
		}

		[Test]
		public void ItShouldMoveTemporaryImageToNewLocationAndUpdateFileNameAndLocation()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList()
			};

			string image1Location = "image_temp_location_1";
			string image1FileName = "image_file_name_1";
			string image2Location = "image_temp_location_2";

			ImageExportInfo image1 = new ImageExportInfo
			{
				TempLocation = image1Location,
				FileName = image1FileName
			};
			ImageExportInfo image2 = new ImageExportInfo
			{
				TempLocation = image2Location
			};
			artifact.Images.Add(image1);
			artifact.Images.Add(image2);

			//ACT
			_instance.RollupImages(artifact);

			//ASSERT
			string expectedLocation = Path.ChangeExtension(image1Location, Extension());
			_fileHelper.Verify(x => x.Move(It.IsAny<string>(), expectedLocation));
			Assert.That(image1.TempLocation, Is.EqualTo(expectedLocation));

			string expectedFileName = Path.ChangeExtension(image1FileName, Extension());
			Assert.That(image1.FileName, Is.EqualTo(expectedFileName));
		}

		[Test]
		public void ItShouldNotMoveTemporaryImageAndCleanTemporaryFileWhenImageExists()
		{
			const bool overwrite = false;
			_exportSettings.Overwrite = overwrite;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList()
			};

			string image1Location = "image_temp_location_1";
			ImageExportInfo image1 = new ImageExportInfo
			{
				TempLocation = image1Location
			};
			artifact.Images.Add(image1);

			string newImagePath = Path.ChangeExtension(image1Location, Extension());

			_fileHelper.Setup(x => x.Exists(newImagePath)).Returns(true);

			//ACT
			_instance.RollupImages(artifact);

			//ASSERT
			_fileHelper.Verify(x => x.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			_fileHelper.Verify(x => x.Delete(newImagePath), Times.Never);
			_fileHelper.Verify(x => x.Delete(It.Is<string>(y => y.EndsWith(".tmp"))), Times.Once);

			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void ItShouldMoveTemporaryImageWhenImageExists_Overwrite()
		{
			const bool overwrite = true;
			_exportSettings.Overwrite = overwrite;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList()
			};

			string image1Location = "image_temp_location_1";
			ImageExportInfo image1 = new ImageExportInfo
			{
				TempLocation = image1Location
			};
			artifact.Images.Add(image1);

			string newImagePath = Path.ChangeExtension(image1Location, Extension());

			_fileHelper.Setup(x => x.Exists(newImagePath)).Returns(true);

			//ACT
			_instance.RollupImages(artifact);

			//ASSERT
			_fileHelper.Verify(x => x.Delete(newImagePath), Times.Once);
			_fileHelper.Verify(x => x.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}

		[Test]
		[TestCase(true, ExpectedResult = false)]
		[TestCase(false, ExpectedResult = true)]
		public bool ItShouldSetRollupResultAccordingly(bool exceptionDuringRollup)
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList()
			};

			string image1Location = "image_temp_location_1";
			ImageExportInfo image1 = new ImageExportInfo
			{
				TempLocation = image1Location
			};
			artifact.Images.Add(image1);

			if (exceptionDuringRollup)
			{
				_imageConverter.Setup(x => x.ConvertImagesToMultiPagePdf(It.IsAny<string[]>(), It.IsAny<string>())).Throws<ImageException>();
				_imageConverter.Setup(x => x.ConvertTIFFsToMultiPage(It.IsAny<string[]>(), It.IsAny<string>())).Throws<ImageException>();
			}

			//ACT
			_instance.RollupImages(artifact);

			//ASSERT
			return image1.SuccessfulRollup;
		}

		[Test]
		public void ItShouldHandleImageRollupError()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList()
			};

			string image1Location = "image_temp_location_1";
			ImageExportInfo image1 = new ImageExportInfo
			{
				TempLocation = image1Location
			};
			artifact.Images.Add(image1);

			_imageConverter.Setup(x => x.ConvertImagesToMultiPagePdf(It.IsAny<string[]>(), It.IsAny<string>())).Throws<ImageException>();
			_imageConverter.Setup(x => x.ConvertTIFFsToMultiPage(It.IsAny<string[]>(), It.IsAny<string>())).Throws<ImageException>();

			_fileHelper.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

			//ACT
			_instance.RollupImages(artifact);

			//ASSERT
			_fileHelper.Verify(x => x.Delete(It.Is<string>(y => y.EndsWith(".tmp"))), Times.Once);
			_status.Verify(x => x.WriteImgProgressError(artifact, It.IsAny<int>(), It.IsAny<Exception>(), It.IsAny<string>()));
		}
	}
}