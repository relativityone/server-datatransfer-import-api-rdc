// -----------------------------------------------------------------------------------------------------
// <copyright file="MultiPageImagesRollupTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.ImagesRollup;
	using Relativity.DataExchange.Io;

	[TestFixture]
	public abstract class MultiPageImagesRollupTests
	{
		private MultiPageImagesRollup _instance;

		private Mock<IImage> _imageConverter;
		private Mock<IStatus> _status;
		private Mock<IFile> _fileHelper;
		private ExportFile _exportSettings;

		[SetUp]
		public void SetUp()
		{
			this._fileHelper = new Mock<IFile>();
			this._status = new Mock<IStatus>();
			this._imageConverter = new Mock<IImage>();

			this._exportSettings = new ExportFile(1)
			{
				FolderPath = "folder_path"
			};
			this._instance = this.CreateInstance(this._exportSettings, this._fileHelper.Object, this._status.Object, this._imageConverter.Object);
		}

		protected abstract MultiPageImagesRollup CreateInstance(ExportFile exportSettings, IFile fileHelper, IStatus status, IImage imageConverter);

		protected abstract void AssertImageConverterCall(Mock<IImage> imageConverter, ObjectExportInfo artifact);

		protected abstract string Extension();

		[Test]
		public void ItShouldSkipOnEmptyList()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new List<ImageExportInfo>()
			};

			// ACT & ASSERT
			Assert.DoesNotThrow(() => this._instance.RollupImages(artifact));
		}

		[Test]
		public void ItShouldRemoveImageFilesAfterRollup()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new List<ImageExportInfo>()
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

			// ACT
			this._instance.RollupImages(artifact);

			// ASSERT
			this._fileHelper.Verify(x => x.Delete(image1Location), Times.Once);
			this._fileHelper.Verify(x => x.Delete(image2Location), Times.Once);
		}

		[Test]
		public void ItShouldMoveTemporaryImageToNewLocationAndUpdateFileNameAndLocation()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new List<ImageExportInfo>()
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

			// ACT
			this._instance.RollupImages(artifact);

			// ASSERT
			string expectedLocation = Path.ChangeExtension(image1Location, this.Extension());
			this._fileHelper.Verify(x => x.Move(It.IsAny<string>(), expectedLocation));
			Assert.That(image1.TempLocation, Is.EqualTo(expectedLocation));

			string expectedFileName = Path.ChangeExtension(image1FileName, this.Extension());
			Assert.That(image1.FileName, Is.EqualTo(expectedFileName));
		}

		[Test]
		public void ItShouldNotMoveTemporaryImageAndCleanTemporaryFileWhenImageExists()
		{
			const bool overwrite = false;
			this._exportSettings.Overwrite = overwrite;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new List<ImageExportInfo>()
			};

			string image1Location = "image_temp_location_1";
			ImageExportInfo image1 = new ImageExportInfo
			{
				TempLocation = image1Location
			};
			artifact.Images.Add(image1);

			string newImagePath = Path.ChangeExtension(image1Location, this.Extension());

			this._fileHelper.Setup(x => x.Exists(newImagePath)).Returns(true);

			// ACT
			this._instance.RollupImages(artifact);

			// ASSERT
			this._fileHelper.Verify(x => x.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			this._fileHelper.Verify(x => x.Delete(newImagePath), Times.Never);
			this._fileHelper.Verify(x => x.Delete(It.Is<string>(y => y.EndsWith(".tmp"))), Times.Once);

			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void ItShouldMoveTemporaryImageWhenImageExists_Overwrite()
		{
			const bool overwrite = true;
			this._exportSettings.Overwrite = overwrite;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new List<ImageExportInfo>()
			};

			string image1Location = "image_temp_location_1";
			ImageExportInfo image1 = new ImageExportInfo
			{
				TempLocation = image1Location
			};
			artifact.Images.Add(image1);

			string newImagePath = Path.ChangeExtension(image1Location, this.Extension());

			this._fileHelper.Setup(x => x.Exists(newImagePath)).Returns(true);

			// ACT
			this._instance.RollupImages(artifact);

			// ASSERT
			this._fileHelper.Verify(x => x.Delete(newImagePath), Times.Once);
			this._fileHelper.Verify(x => x.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}

		[Test]
		[TestCase(true, ExpectedResult = false)]
		[TestCase(false, ExpectedResult = true)]
		public bool ItShouldSetRollupResultAccordingly(bool exceptionDuringRollup)
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new List<ImageExportInfo>()
			};

			string image1Location = "image_temp_location_1";
			ImageExportInfo image1 = new ImageExportInfo
			{
				TempLocation = image1Location
			};
			artifact.Images.Add(image1);

			if (exceptionDuringRollup)
			{
				this._imageConverter.Setup(x => x.ConvertImagesToMultiPagePdf(It.IsAny<string[]>(), It.IsAny<string>())).Throws<ImageException>();
				this._imageConverter.Setup(x => x.ConvertTIFFsToMultiPage(It.IsAny<string[]>(), It.IsAny<string>())).Throws<ImageException>();
			}

			// ACT
			this._instance.RollupImages(artifact);

			// ASSERT
			Assert.That(artifact.DocumentError, Is.EqualTo(exceptionDuringRollup));
			return image1.SuccessfulRollup;
		}

		[Test]
		public void ItShouldHandleImageRollupError()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new List<ImageExportInfo>()
			};

			string image1Location = "image_temp_location_1";
			ImageExportInfo image1 = new ImageExportInfo
			{
				TempLocation = image1Location
			};
			artifact.Images.Add(image1);

			this._imageConverter.Setup(x => x.ConvertImagesToMultiPagePdf(It.IsAny<string[]>(), It.IsAny<string>())).Throws<ImageException>();
			this._imageConverter.Setup(x => x.ConvertTIFFsToMultiPage(It.IsAny<string[]>(), It.IsAny<string>())).Throws<ImageException>();

			this._fileHelper.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

			// ACT
			this._instance.RollupImages(artifact);

			// ASSERT
			this._fileHelper.Verify(x => x.Delete(It.Is<string>(y => y.EndsWith(".tmp"))), Times.Once);
			this._status.Verify(x => x.WriteImgProgressError(artifact, It.IsAny<int>(), It.IsAny<Exception>(), It.IsAny<string>()));
		}
	}
}