﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageLoadFileBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System.Collections;
    using System.Threading;

    using global::NUnit.Framework;

	using kCura.WinEDDS;
    using kCura.WinEDDS.Exporters;

    using Moq;

	using Relativity.Export.VolumeManagerV2.Batches;
	using Relativity.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.Import.Export.Io;
    using Relativity.Logging;

    [TestFixture]
	public class ImageLoadFileBatchValidatorTests
	{
		private ImageLoadFileBatchValidator _instance;
		private Mock<IDestinationPath> _destinationPath;
		private Mock<IFile> _fileHelper;
		private Mock<IStatus> _status;

		[SetUp]
		public void SetUp()
		{
			_destinationPath = new Mock<IDestinationPath>();
			_destinationPath.Setup(x => x.Path).Returns("file_path");

			_fileHelper = new Mock<IFile>();
			_status = new Mock<IStatus>();
			_instance = new ImageLoadFileBatchValidator(_destinationPath.Object, _fileHelper.Object, _status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldWriteErrorForMissingFile()
		{
			_fileHelper.Setup(x => x.Exists(_destinationPath.Object.Path)).Returns(false);

			// ACT
			_instance.ValidateExportedBatch(null, null, CancellationToken.None);

			// ASSERT
			_status.Verify(x => x.WriteError(It.IsAny<string>()));
		}

		[Test]
		public void ItShouldWriteErrorForEmptyFileWhenImagesExist()
		{
			_fileHelper.Setup(x => x.Exists(_destinationPath.Object.Path)).Returns(true);
			_fileHelper.Setup(x => x.GetFileSize(_destinationPath.Object.Path)).Returns(0);

			ArrayList images = new ArrayList { new ImageExportInfo() };
			ObjectExportInfo[] artifacts =
			{
				new ObjectExportInfo
				{
					Images = images
				}
			};

			// ACT
			_instance.ValidateExportedBatch(artifacts, null, CancellationToken.None);

			// ASSERT
			_status.Verify(x => x.WriteError(It.IsAny<string>()));
		}

		[Test]
		public void ItShouldPassForNonEmptyFile()
		{
			_fileHelper.Setup(x => x.Exists(_destinationPath.Object.Path)).Returns(true);
			_fileHelper.Setup(x => x.GetFileSize(_destinationPath.Object.Path)).Returns(1);

			// ACT
			_instance.ValidateExportedBatch(null, null, CancellationToken.None);

			// ASSERT
			_status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldPassForEmptyFileWhenNoImagesExist()
		{
			_fileHelper.Setup(x => x.Exists(_destinationPath.Object.Path)).Returns(true);
			_fileHelper.Setup(x => x.GetFileSize(_destinationPath.Object.Path)).Returns(0);

			ObjectExportInfo[] artifacts =
			{
				new ObjectExportInfo
				{
					Images = new ArrayList()
				}
			};

			// ACT
			_instance.ValidateExportedBatch(artifacts, null, CancellationToken.None);

			// ASSERT
			_status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never);
		}
	}
}