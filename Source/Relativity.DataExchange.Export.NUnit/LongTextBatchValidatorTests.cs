// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextBatchValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.Text;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Io;
	using Relativity.Logging;

	[TestFixture]
	public class LongTextBatchValidatorTests
	{
		protected IBatchValidator Instance { get; set; }

		protected Mock<ILongTextRepository> LongTextRepository { get; set; }

		protected Mock<IFile> FileHelper { get; set; }

		protected Mock<IStatus> Status { get; set; }

		[SetUp]
		public void SetUp()
		{
			this.LongTextRepository = new Mock<ILongTextRepository>();
			this.FileHelper = new Mock<IFile>();
			this.Status = new Mock<IStatus>();

			this.Instance = this.CreateValidator();
		}

		protected virtual IBatchValidator CreateValidator()
		{
			return new LongTextBatchValidator(this.LongTextRepository.Object, this.FileHelper.Object, this.Status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldPassForFileRequiringDeletion()
		{
			LongText longText = this.Create("location", true, true);
			this.LongTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> { longText });

			// ACT
			this.Instance.ValidateExportedBatch(null, null, CancellationToken.None);

			// ASSERT
			this.Status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never());
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never());
		}

		[Test]
		public void ItShouldPassForFileWithoutExportRequest()
		{
			string location = "location";
			LongText longText = this.Create(location, true, false);
			this.LongTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> { longText });
			this.FileHelper.Setup(x => x.Exists(location)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location)).Returns(1);

			// ACT
			this.Instance.ValidateExportedBatch(null, null, CancellationToken.None);

			// ASSERT
			this.Status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never());
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never());
		}

		[TestCase("")]
		[TestCase(null)]
		public void ItShouldPassForWhenLocationNotSet(string emptyLocation)
		{
			LongText longText = this.Create(emptyLocation, true, false);
			this.LongTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> { longText });

			// ACT
			this.Instance.ValidateExportedBatch(null, null, CancellationToken.None);

			// ASSERT
			this.Status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never());
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never());
			this.FileHelper.Verify(x => x.Exists(emptyLocation), Times.Never());
			this.FileHelper.Verify(x => x.GetFileSize(emptyLocation), Times.Never());
		}

		[Test]
		public void ItShouldWriteErrorForMissingFile()
		{
			string location = "location";
			LongText longText = this.Create(location, false, true);
			this.LongTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> { longText });

			this.FileHelper.Setup(x => x.Exists(location)).Returns(false);

			// ACT
			this.Instance.ValidateExportedBatch(null, null, CancellationToken.None);

			// ASSERT
			this.Status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Once());
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never());
		}

		[Test]
		public void ItShouldWriteWarningForEmptyFile()
		{
			string location = "location";
			LongText longText = this.Create(location, false, true);
			this.LongTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> { longText });

			this.FileHelper.Setup(x => x.Exists(location)).Returns(true);
			this.FileHelper.Setup(x => x.GetFileSize(location)).Returns(0);

			// ACT
			this.Instance.ValidateExportedBatch(null, null, CancellationToken.None);

			// ASSERT
			this.Status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never());
			this.Status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Once);
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