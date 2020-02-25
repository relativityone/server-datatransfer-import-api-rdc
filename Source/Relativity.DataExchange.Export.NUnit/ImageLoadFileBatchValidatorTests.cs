// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageLoadFileBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.TestFramework;

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
			this._destinationPath = new Mock<IDestinationPath>();
			this._destinationPath.Setup(x => x.Path).Returns("file_path");

			this._fileHelper = new Mock<IFile>();
			this._status = new Mock<IStatus>();
			this._instance = new ImageLoadFileBatchValidator(this._destinationPath.Object, this._fileHelper.Object, this._status.Object, new TestNullLogger());
		}

		[Test]
		public void ItShouldWriteErrorForMissingFile()
		{
			this._fileHelper.Setup(x => x.Exists(this._destinationPath.Object.Path)).Returns(false);

			// ACT
			this._instance.ValidateExportedBatch(null, CancellationToken.None);

			// ASSERT
			this._status.Verify(x => x.WriteError(It.IsAny<string>()));
		}

		[Test]
		public void ItShouldWriteErrorForEmptyFileWhenImagesExist()
		{
			this._fileHelper.Setup(x => x.Exists(this._destinationPath.Object.Path)).Returns(true);
			this._fileHelper.Setup(x => x.GetFileSize(this._destinationPath.Object.Path)).Returns(0);

			ArrayList images = new ArrayList { new ImageExportInfo() };
			ObjectExportInfo[] artifacts =
			{
				new ObjectExportInfo
				{
					Images = images
				}
			};

			// ACT
			this._instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this._status.Verify(x => x.WriteError(It.IsAny<string>()));
		}

		[Test]
		public void ItShouldPassForNonEmptyFile()
		{
			this._fileHelper.Setup(x => x.Exists(this._destinationPath.Object.Path)).Returns(true);
			this._fileHelper.Setup(x => x.GetFileSize(this._destinationPath.Object.Path)).Returns(1);

			// ACT
			this._instance.ValidateExportedBatch(null, CancellationToken.None);

			// ASSERT
			this._status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldPassForEmptyFileWhenNoImagesExist()
		{
			this._fileHelper.Setup(x => x.Exists(this._destinationPath.Object.Path)).Returns(true);
			this._fileHelper.Setup(x => x.GetFileSize(this._destinationPath.Object.Path)).Returns(0);

			ObjectExportInfo[] artifacts =
			{
				new ObjectExportInfo
				{
					Images = new ArrayList()
				}
			};

			// ACT
			this._instance.ValidateExportedBatch(artifacts, CancellationToken.None);

			// ASSERT
			this._status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never);
		}
	}
}