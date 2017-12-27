using System.Collections.Generic;
using System.Text;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Batches
{
	[TestFixture]
	public class LongTextBatchValidatorTests
	{
		private LongTextBatchValidator _instance;
		private Mock<ILongTextRepository> _longTextRepository;
		private Mock<IFileHelper> _fileHelper;
		private Mock<IStatus> _status;

		[SetUp]
		public void SetUp()
		{
			_longTextRepository = new Mock<ILongTextRepository>();
			_fileHelper = new Mock<IFileHelper>();
			_status = new Mock<IStatus>();

			_instance = new LongTextBatchValidator(_longTextRepository.Object, _fileHelper.Object, _status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldPassForFileRequiringDeletion()
		{
			LongText longText = Create("location", true, true);
			_longTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> {longText});

			//ACT
			_instance.ValidateExportedBatch(null, null, CancellationToken.None);

			//ASSERT
			_status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never());
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never());
		}

		[Test]
		public void ItShouldPassForFileWithoutExportRequest()
		{
			LongText longText = Create("location", true, false);
			_longTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> {longText});

			//ACT
			_instance.ValidateExportedBatch(null, null, CancellationToken.None);

			//ASSERT
			_status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never());
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never());
		}

		[Test]
		public void ItShouldWriteErrorForMissingFile()
		{
			string location = "location";
			LongText longText = Create(location, false, true);
			_longTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> {longText});

			_fileHelper.Setup(x => x.Exists(location)).Returns(false);

			//ACT
			_instance.ValidateExportedBatch(null, null, CancellationToken.None);

			//ASSERT
			_status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Once);
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Never());
		}

		[Test]
		public void ItShouldWriteWarningForEmptyFile()
		{
			string location = "location";
			LongText longText = Create(location, false, true);
			_longTextRepository.Setup(x => x.GetLongTexts()).Returns(new List<LongText> {longText});

			_fileHelper.Setup(x => x.Exists(location)).Returns(true);
			_fileHelper.Setup(x => x.GetFileSize(location)).Returns(0);

			//ACT
			_instance.ValidateExportedBatch(null, null, CancellationToken.None);

			//ASSERT
			_status.Verify(x => x.WriteError(It.IsAny<string>()), Times.Never());
			_status.Verify(x => x.WriteWarning(It.IsAny<string>()), Times.Once);
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