// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageFileBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections;
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

		protected Mock<IErrorFileWriter> ErrorFileWriter { get; set; }

		protected Mock<IFile> FileHelper { get; set; }

		[SetUp]
		public void SetUp()
		{
			this.ErrorFileWriter = new Mock<IErrorFileWriter>();
			this.FileHelper = new Mock<IFile>();
			this._status = new Mock<IStatus>();

			this._instance = this.CreateSut();
		}

		protected virtual IBatchValidator CreateSut()
		{
			return new ImageFileBatchValidator(this.ErrorFileWriter.Object, this.FileHelper.Object, this._status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldPassForEmptyImageList()
		{
			ObjectExportInfo[] artifacts = { new ObjectExportInfo() };

			// ACT
			this._instance.ValidateExportedBatch(artifacts, new VolumePredictions[1], CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldPassForEmptyImageWithEmptyGuid(bool successfulRollup)
		{
			ObjectExportInfo[] artifacts = { this.CreateWithEmptyGuid(successfulRollup) };
			VolumePredictions[] volumePredictions = { new VolumePredictions() };

			// ACT
			this._instance.ValidateExportedBatch(artifacts, volumePredictions, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(false, 1)]
		[TestCase(true, 0)]
		public void ItShouldWriteErrorForMissingOrEmptySingleImage(bool exists, long size)
		{
			string location = "file_location";

			ObjectExportInfo[] artifacts = { this.CreateSingleWithLocation(location) };

			this.FileHelper.Setup(x => x.Exists(location)).Returns(exists);
			this.FileHelper.Setup(x => x.GetFileSize(location)).Returns(size);

			// ACT
			this._instance.ValidateExportedBatch(artifacts, new VolumePredictions[1], CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType.Image, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteUpdateForImageWithSmallerSize()
		{
			string location = "file_location";
			const int fileSize = 407612;

			ObjectExportInfo[] artifacts = { this.CreateSingleWithLocation(location) };
			VolumePredictions[] volumePredictions =
			{
				new VolumePredictions
				{
					ImageFilesSize = fileSize
				}
			};

			this.FileHelper.Setup(x => x.Exists(location)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location)).Returns(fileSize - 1);

			// ACT
			this._instance.ValidateExportedBatch(artifacts, volumePredictions, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			this._status.Verify(x => x.WriteUpdate(It.IsAny<string>(), true), Times.Never);
		}

		[Test]
		public void ItShouldWriteWarningForImageWithBiggerSize()
		{
			string location = "file_location";
			const int fileSize = 407612;

			ObjectExportInfo[] artifacts = { this.CreateSingleWithLocation(location) };
			VolumePredictions[] volumePredictions =
			{
				new VolumePredictions
				{
					ImageFilesSize = fileSize
				}
			};

			this.FileHelper.Setup(x => x.Exists(location)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location)).Returns(fileSize + 1);

			// ACT
			this._instance.ValidateExportedBatch(artifacts, volumePredictions, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(false, 1)]
		[TestCase(true, 0)]
		public void ItShouldWriteErrorForMissingOrEmptyOneOfImages(bool exists, long size)
		{
			string location1 = "file_location_1";
			string location2 = "file_location_2";

			ObjectExportInfo[] artifacts = { this.CreateTwoImagesWithLocations(location1, location2) };

			this.FileHelper.Setup(x => x.Exists(location1)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location1)).Returns(1);

			this.FileHelper.Setup(x => x.Exists(location2)).Returns(exists);
			this.FileHelper.Setup(x => x.GetFileSize(location2)).Returns(size);

			// ACT
			this._instance.ValidateExportedBatch(artifacts, new VolumePredictions[1], CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType.Image, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
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
			VolumePredictions[] volumePredictions =
			{
				new VolumePredictions
				{
					ImageFilesSize = size1 + size2 + 1
				}
			};

			this.FileHelper.Setup(x => x.Exists(location1)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location1)).Returns(size1);

			this.FileHelper.Setup(x => x.Exists(location2)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location2)).Returns(size2);

			// ACT
			this._instance.ValidateExportedBatch(artifacts, volumePredictions, CancellationToken.None);

			// ASSERT
			this.ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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
			ArrayList images = new ArrayList
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
				SuccessfulRollup = true,
				TempLocation = location
			};
			ArrayList images = new ArrayList
			{
				image
			};
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
			ArrayList images = new ArrayList
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