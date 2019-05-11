// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageFileBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
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
			ErrorFileWriter = new Mock<IErrorFileWriter>();
			FileHelper = new Mock<IFile>();
			_status = new Mock<IStatus>();

			_instance = CreateSut();
		}

		protected virtual IBatchValidator CreateSut()
		{
			return new ImageFileBatchValidator(ErrorFileWriter.Object, FileHelper.Object, _status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldPassForEmptyImageList()
		{
			ObjectExportInfo[] artifacts = { new ObjectExportInfo() };

			// ACT
			_instance.ValidateExportedBatch(artifacts, new VolumePredictions[1], CancellationToken.None);

			// ASSERT
			ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldPassForEmptyImageWithEmptyGuid(bool successfulRollup)
		{
			ObjectExportInfo[] artifacts = { CreateWithEmptyGuid(successfulRollup) };
			VolumePredictions[] volumePredictions = { new VolumePredictions() };

			// ACT
			_instance.ValidateExportedBatch(artifacts, volumePredictions, CancellationToken.None);

			// ASSERT
			ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(false, 1)]
		[TestCase(true, 0)]
		public void ItShouldWriteErrorForMissingOrEmptySingleImage(bool exists, long size)
		{
			string location = "file_location";

			ObjectExportInfo[] artifacts = { CreateSingleWithLocation(location) };

			FileHelper.Setup(x => x.Exists(location)).Returns(exists);
			FileHelper.Setup(x => x.GetFileSize(location)).Returns(size);

			// ACT
			_instance.ValidateExportedBatch(artifacts, new VolumePredictions[1], CancellationToken.None);

			// ASSERT
			ErrorFileWriter.Verify(x => x.Write(Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType.Image, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteUpdateForImageWithSmallerSize()
		{
			string location = "file_location";
			const int fileSize = 407612;

			ObjectExportInfo[] artifacts = { CreateSingleWithLocation(location) };
			VolumePredictions[] volumePredictions =
			{
				new VolumePredictions
				{
					ImageFilesSize = fileSize
				}
			};

			FileHelper.Setup(x => x.Exists(location)).Returns(true);
			FileHelper.Setup(x => x.GetFileSize(location)).Returns(fileSize - 1);

			// ACT
			_instance.ValidateExportedBatch(artifacts, volumePredictions, CancellationToken.None);

			// ASSERT
			ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			_status.Verify(x => x.WriteUpdate(It.IsAny<string>(), true), Times.Never);
		}

		[Test]
		public void ItShouldWriteWarningForImageWithBiggerSize()
		{
			string location = "file_location";
			const int fileSize = 407612;

			ObjectExportInfo[] artifacts = { CreateSingleWithLocation(location) };
			VolumePredictions[] volumePredictions =
			{
				new VolumePredictions
				{
					ImageFilesSize = fileSize
				}
			};

			FileHelper.Setup(x => x.Exists(location)).Returns(true);
			FileHelper.Setup(x => x.GetFileSize(location)).Returns(fileSize + 1);

			// ACT
			_instance.ValidateExportedBatch(artifacts, volumePredictions, CancellationToken.None);

			// ASSERT
			ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		[TestCase(false, 1)]
		[TestCase(true, 0)]
		public void ItShouldWriteErrorForMissingOrEmptyOneOfImages(bool exists, long size)
		{
			string location1 = "file_location_1";
			string location2 = "file_location_2";

			ObjectExportInfo[] artifacts = { CreateTwoImagesWithLocations(location1, location2) };

			FileHelper.Setup(x => x.Exists(location1)).Returns(true);
			FileHelper.Setup(x => x.GetFileSize(location1)).Returns(1);

			FileHelper.Setup(x => x.Exists(location2)).Returns(exists);
			FileHelper.Setup(x => x.GetFileSize(location2)).Returns(size);

			// ACT
			_instance.ValidateExportedBatch(artifacts, new VolumePredictions[1], CancellationToken.None);

			// ASSERT
			ErrorFileWriter.Verify(x => x.Write(Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers.ErrorFileWriter.ExportFileType.Image, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteWarningForInvalidSize()
		{
			string location1 = "file_location_1";
			string location2 = "file_location_2";

			const long size1 = 680296;
			const long size2 = 134113;

			ObjectExportInfo[] artifacts = { CreateTwoImagesWithLocations(location1, location2) };
			VolumePredictions[] volumePredictions =
			{
				new VolumePredictions
				{
					ImageFilesSize = size1 + size2 + 1
				}
			};

			FileHelper.Setup(x => x.Exists(location1)).Returns(true);
			FileHelper.Setup(x => x.GetFileSize(location1)).Returns(size1);

			FileHelper.Setup(x => x.Exists(location2)).Returns(true);
			FileHelper.Setup(x => x.GetFileSize(location2)).Returns(size2);

			// ACT
			_instance.ValidateExportedBatch(artifacts, volumePredictions, CancellationToken.None);

			// ASSERT
			ErrorFileWriter.Verify(x => x.Write(It.IsAny<ErrorFileWriter.ExportFileType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never);
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