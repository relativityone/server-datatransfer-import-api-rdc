﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    using global::NUnit.Framework;

	using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
    using kCura.WinEDDS.Exporters;

    using Moq;

    using Relativity.Logging;

    [TestFixture]
	public class LongTextBatchValidatorTests
	{
		protected IBatchValidator Instance { get; set; }

		protected Mock<ILongTextRepository> LongTextRepository { get; set; }

		protected Mock<IFileHelper> FileHelper { get; set; }

		protected Mock<IStatus> Status { get; set; }

		[SetUp]
		public void SetUp()
		{
			LongTextRepository = new Mock<ILongTextRepository>();
			FileHelper = new Mock<IFileHelper>();
			Status = new Mock<IStatus>();

			Instance = CreateValidator();
		}

		protected virtual IBatchValidator CreateValidator()
		{
			return new LongTextBatchValidator(LongTextRepository.Object, FileHelper.Object, Status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldPassForFileRequiringDeletion()
		{
			LongText longText = Create("location", true, true);
			LongTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> { longText });

			// ACT
			Instance.ValidateExportedBatch(null, null, CancellationToken.None);

			// ASSERT
			Status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never());
			Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never());
		}

		[Test]
		public void ItShouldPassForFileWithoutExportRequest()
		{
			string location = "location";
			LongText longText = Create(location, true, false);
			LongTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> { longText });
			FileHelper.Setup(x => x.Exists(location)).Returns(true);
			FileHelper.Setup(x => x.GetFileSize(location)).Returns(1);

			// ACT
			Instance.ValidateExportedBatch(null, null, CancellationToken.None);

			// ASSERT
			Status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never());
			Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never());
		}

		[TestCase("")]
		[TestCase(null)]
		public void ItShouldPassForWhenLocationNotSet(string emptyLocation)
		{
			LongText longText = Create(emptyLocation, true, false);
			LongTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> { longText });

			// ACT
			Instance.ValidateExportedBatch(null, null, CancellationToken.None);

			// ASSERT
			Status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never());
			Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never());
			FileHelper.Verify(x => x.Exists(emptyLocation), Times.Never());
			FileHelper.Verify(x => x.GetFileSize(emptyLocation), Times.Never());
		}

		[Test]
		public void ItShouldWriteErrorForMissingFile()
		{
			string location = "location";
			LongText longText = Create(location, false, true);
			LongTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> { longText });

			FileHelper.Setup(x => x.Exists(location)).Returns(false);

			// ACT
			Instance.ValidateExportedBatch(null, null, CancellationToken.None);

			// ASSERT
			Status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Once());
			Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never());
		}

		[Test]
		public void ItShouldWriteWarningForEmptyFile()
		{
			string location = "location";
			LongText longText = Create(location, false, true);
			LongTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> { longText });

			FileHelper.Setup(x => x.Exists(location)).Returns(true);
			FileHelper.Setup(x => x.GetFileSize(location)).Returns(0);

			// ACT
			Instance.ValidateExportedBatch(null, null, CancellationToken.None);

			// ASSERT
			Status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never());
			Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Once);
		}

		private LongText Create(string location, bool requireDeletion, bool toDownload)
		{
			if (!toDownload)
			{
				return LongText.CreateFromExistingFile(1, 1, location, Encoding.ASCII);
			}

			LongTextExportRequest exportRequest = LongTextExportRequest.CreateRequestForFullText(new ObjectExportInfo(), 1, location);

			if (requireDeletion)
			{
				return LongText.CreateFromMissingValue(1, 1, exportRequest, Encoding.ASCII);
			}

			return LongText.CreateFromMissingFile(1, 1, exportRequest, Encoding.ASCII, Encoding.ASCII);
		}
	}
}