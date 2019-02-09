﻿// ----------------------------------------------------------------------------
// <copyright file="LoadFileBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System.Threading;

    using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;

    using Moq;

    using global::NUnit.Framework;

    using Relativity.Logging;

    [TestFixture]
	public class LoadFileBatchValidatorTests
	{
		private LoadFileBatchValidator _instance;
		private Mock<IDestinationPath> _destinationPath;
		private Mock<IFileHelper> _fileHelper;
		private Mock<IStatus> _status;

		[SetUp]
		public void SetUp()
		{
			_destinationPath = new Mock<IDestinationPath>();
			_destinationPath.Setup(x => x.Path).Returns("file_path");

			_fileHelper = new Mock<IFileHelper>();
			_status = new Mock<IStatus>();

			_instance = new LoadFileBatchValidator(_destinationPath.Object, _fileHelper.Object, _status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldWriteErrorForMissingFile()
		{
			_fileHelper.Setup(x => x.Exists(_destinationPath.Object.Path)).Returns(false);

			//ACT
			_instance.ValidateExportedBatch(null, null, CancellationToken.None);

			//ASSERT
			_status.Verify(x => x.WriteError(It.IsAny<string>()));
		}

		[Test]
		public void ItShouldPassForNonEmptyFile()
		{
			_fileHelper.Setup(x => x.Exists(_destinationPath.Object.Path)).Returns(true);
			_fileHelper.Setup(x => x.GetFileSize(_destinationPath.Object.Path)).Returns(1);

			//ACT
			_instance.ValidateExportedBatch(null, null, CancellationToken.None);

			//ASSERT
			_status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void ItShouldWriteErrorForEmptyFile()
		{
			_fileHelper.Setup(x => x.Exists(_destinationPath.Object.Path)).Returns(true);
			_fileHelper.Setup(x => x.GetFileSize(_destinationPath.Object.Path)).Returns(0);

			//ACT
			_instance.ValidateExportedBatch(null, null, CancellationToken.None);

			//ASSERT
			_status.Verify(x => x.WriteError(It.IsAny<string>()));
		}
	}
}