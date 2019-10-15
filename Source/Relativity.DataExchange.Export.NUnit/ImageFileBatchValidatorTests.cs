// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageFileBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
    using System.Collections.Generic;
    using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Io;
	using Relativity.Logging;

	[TestFixture]
	public class ImageFileBatchValidatorTests
	{
		private IBatchValidator _instance;
		private Mock<IStatus> _status;
		private Mock<IAppSettings> _appSettings;
		private Mock<ILog> _logger;

		protected Mock<IErrorFileWriter> ErrorFileWriter { get; set; }

		protected Mock<IFile> FileHelper { get; set; }

		[SetUp]
		public void SetUp()
		{
			this.ErrorFileWriter = new Mock<IErrorFileWriter>();
			this.FileHelper = new Mock<IFile>();
			this._status = new Mock<IStatus>();
			this._appSettings = new Mock<IAppSettings>();
			this._logger = new Mock<ILog>();
			this._instance = this.CreateSut();
		}

		protected virtual IBatchValidator CreateSut()
		{
			return new ImageFileBatchValidator(
				this.ErrorFileWriter.Object,
				this.FileHelper.Object,
				this._appSettings.Object,
				this._logger.Object);
		}

		[Test]
		public void ItShouldPassForEmptyImageList()
		{
			ObjectExportInfo[] artifacts = { new ObjectExportInfo() };

			// ACT
			this._instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), It.IsAny<ObjectExportInfo>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldPassForEmptyImageWithEmptyGuid(bool successfulRollup)
		{
			ObjectExportInfo[] artifacts = { this.CreateWithEmptyGuid(successfulRollup) };

			// ACT
			this._instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), It.IsAny<ObjectExportInfo>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteErrorForMissingSingleImage()
		{
			string location = "file_location";

			ObjectExportInfo[] artifacts = { this.CreateSingleWithLocation(location) };

			this.FileHelper.Setup(x => x.Exists(location)).Returns(false);
			this.FileHelper.Setup(x => x.GetFileSize(location)).Returns(1);

			// ACT
			this._instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType
						.Image,
					It.IsAny<ObjectExportInfo>(),
					It.IsAny<string>(),
					It.IsAny<string>()),
				Times.Once());
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
			this._logger.Verify(x => x.LogVerbose(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
		}

		[Test]
		public void ItShouldWriteUpdateForImageWithSmallerSize()
		{
			string location = "file_location";
			const int fileSize = 407612;

			ObjectExportInfo[] artifacts = { this.CreateSingleWithLocation(location) };

			this.FileHelper.Setup(x => x.Exists(location)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location)).Returns(fileSize - 1);

			// ACT
			this._instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					It.IsAny<ErrorFileWriter.ExportFileType>(),
					It.IsAny<ObjectExportInfo>(),
					It.IsAny<string>(),
					It.IsAny<string>()),
				Times.Never);
			this._status.Verify(x => x.WriteUpdate(It.IsAny<string>(), true), Times.Never);
		}

		[Test]
		public void ItShouldWriteWarningForImageWithBiggerSize()
		{
			string location = "file_location";
			const int fileSize = 407612;

			ObjectExportInfo[] artifacts = { this.CreateSingleWithLocation(location) };

			this.FileHelper.Setup(x => x.Exists(location)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location)).Returns(fileSize + 1);

			// ACT
			this._instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					It.IsAny<ErrorFileWriter.ExportFileType>(),
					It.IsAny<ObjectExportInfo>(),
					It.IsAny<string>(),
					It.IsAny<string>()),
				Times.Never);
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteErrorForMissingOneOfImages()
		{
			string location1 = "file_location_1";
			string location2 = "file_location_2";

			ObjectExportInfo[] artifacts = { this.CreateTwoImagesWithLocations(location1, location2) };

			this.FileHelper.Setup(x => x.Exists(location1)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location1)).Returns(1);

			this.FileHelper.Setup(x => x.Exists(location2)).Returns(false);
			this.FileHelper.Setup(x => x.GetFileSize(location2)).Returns(1);

			// ACT
			this._instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType.Image,
					It.IsAny<ObjectExportInfo>(),
					It.IsAny<string>(),
					It.IsAny<string>()),
				Times.Once());
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
			this._logger.Verify(x => x.LogVerbose(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldNotWriteErrorForEmptyFileWhenConfigured(bool createErrorForEmptyNativeFile)
		{
			// ARRANGE
			string location = "file_location";
			ObjectExportInfo[] artifacts = { this.CreateSingleWithLocation(location) };
			this._appSettings.SetupGet(x => x.CreateErrorForEmptyNativeFile).Returns(createErrorForEmptyNativeFile);
			this.FileHelper.Setup(x => x.Exists(location)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location)).Returns(0);

			// ACT
			this._instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType
						.Image,
					It.IsAny<ObjectExportInfo>(),
					It.IsAny<string>(),
					It.IsAny<string>()),
				createErrorForEmptyNativeFile ? Times.Once() : Times.Never());
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteWarningForInvalidSize()
		{
			string location1 = "file_location_1";
			string location2 = "file_location_2";

			const long size1 = 680296;
			const long size2 = 134113;

			ObjectExportInfo[] artifacts = { this.CreateTwoImagesWithLocations(location1, location2) };

			this.FileHelper.Setup(x => x.Exists(location1)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location1)).Returns(size1);

			this.FileHelper.Setup(x => x.Exists(location2)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location2)).Returns(size2);

			// ACT
			this._instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					It.IsAny<ErrorFileWriter.ExportFileType>(),
					It.IsAny<ObjectExportInfo>(),
					It.IsAny<string>(),
					It.IsAny<string>()),
				Times.Never);
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		public void ItShouldWriteErrorForInvalidNativeSourceLocation(string location)
		{
			// ARRANGE
			ObjectExportInfo artifact = this.CreateSingleWithLocation(location);
			ObjectExportInfo[] artifacts = { artifact };
			this.FileHelper.Setup(x => x.Exists(location)).Returns(false);
			this.FileHelper.Setup(x => x.GetFileSize(location)).Returns(0);

			// ACT
			this._instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(
				x => x.Write(
					Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType
						.Image,
					artifact,
					location,
					It.IsAny<string>()),
				Times.Once);
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		private ObjectExportInfo CreateTwoImagesWithLocations(string location1, string location2)
		{
			ImageExportInfo image1 = new ImageExportInfo
			{
				FileGuid = Guid.NewGuid().ToString(),
				SuccessfulRollup = false,
				TempLocation = location1
			};
			ImageExportInfo image2 = new ImageExportInfo
			{
				FileGuid = Guid.NewGuid().ToString(),
				SuccessfulRollup = false,
				TempLocation = location2
			};
			var images = new List<ImageExportInfo>
			{
				image1,
				image2
			};
			return new ObjectExportInfo
			{
				Images = images
			};
		}

		private ObjectExportInfo CreateSingleWithLocation(string location)
		{
			ImageExportInfo image = new ImageExportInfo
				                        {
					                        FileGuid = Guid.NewGuid().ToString(),
					                        SourceLocation = @"\\files\T001\Files\EDDS1234567\temp.bin",
					                        SuccessfulRollup = true,
					                        TempLocation = location
				                        };
			var images = new List<ImageExportInfo> { image };
			return new ObjectExportInfo
			{
				Images = images
			};
		}

		private ObjectExportInfo CreateWithEmptyGuid(bool successfulRollup)
		{
			ImageExportInfo image = new ImageExportInfo
			{
				FileGuid = string.Empty,
				SuccessfulRollup = successfulRollup
			};
			var images = new List<ImageExportInfo>
			{
				image
			};
			return new ObjectExportInfo
			{
				Images = images
			};
		}
	}
}