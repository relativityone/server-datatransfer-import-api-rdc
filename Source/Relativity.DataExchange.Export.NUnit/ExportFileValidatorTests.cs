// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class ExportFileValidatorTests
	{
		private ExportFileValidator _instance;

		private ExportFile _exportSettings;

		private Mock<IStatus> _status;
		private Mock<IFile> _fileHelper;
		private Mock<IExportRequestRepository> _exportRequestRepository;

		[SetUp]
		public void SetUp()
		{
			this._exportSettings = new ExportFile(1);

			this._status = new Mock<IStatus>();
			this._fileHelper = new Mock<IFile>();
			this._exportRequestRepository = new Mock<IExportRequestRepository>();

			this._instance = new ExportFileValidator(this._exportSettings, this._exportRequestRepository.Object, this._status.Object, this._fileHelper.Object, new TestNullLogger());
		}

		[Test]
		[TestCase(true, true, ExpectedResult = true)]
		[TestCase(false, true, ExpectedResult = true)]
		[TestCase(false, false, ExpectedResult = true)]
		[TestCase(true, false, ExpectedResult = false)]
		public bool ItShouldReturnAppropriateResult(bool fileExists, bool overwrite)
		{
			const string filePath = "file_path";

			this._fileHelper.Setup(x => x.Exists(filePath)).Returns(fileExists);
			this._exportSettings.Overwrite = overwrite;

			// ACT & ASSERT
			return this._instance.CanExport(filePath, string.Empty);
		}

		[Test]
		public void ItShouldWriteWarningWhenFileExistsAndNotOverwriting()
		{
			const string filePath = "file_path";

			this._fileHelper.Setup(x => x.Exists(filePath)).Returns(true);
			this._exportSettings.Overwrite = false;

			// ACT
			this._instance.CanExport(filePath, string.Empty);

			// ASSERT
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()));
		}

		[Test]
		public void ItShouldWriteStatusAndDeleteFileWhenExistsAndOverwriting()
		{
			const string filePath = "file_path";

			this._fileHelper.Setup(x => x.Exists(filePath)).Returns(true);
			this._exportSettings.Overwrite = true;

			// ACT
			this._instance.CanExport(filePath, string.Empty);

			// ASSERT
			this._status.Verify(x => x.WriteStatusLine(EventType2.Status, It.IsAny<string>(), false));
			this._fileHelper.Verify(x => x.Delete(filePath));
		}

		[Test]
		public void ItShouldWriteWarningWhenExportRequestExistsAndNotOverwriting()
		{
			const string filePath = "file_path";

			this._exportRequestRepository.Setup(repository => repository.AnyRequestForLocation(filePath)).Returns(true);
			this._exportSettings.Overwrite = false;

			// ACT
			bool canExport = this._instance.CanExport(filePath, string.Empty);

			// ASSERT
			Assert.That(canExport, Is.False);
			this._status.Verify(x => x.WriteWarning(It.IsAny<string>()));
		}

		[Test]
		public void ItShouldWriteStatusWhenExportRequestExistsAndOverwriting()
		{
			const string filePath = "file_path";

			this._exportRequestRepository.Setup(repository => repository.AnyRequestForLocation(filePath)).Returns(true);
			this._exportSettings.Overwrite = true;

			// ACT
			bool canExport = this._instance.CanExport(filePath, string.Empty);

			// ASSERT
			Assert.That(canExport, Is.False);
			this._status.Verify(x => x.WriteStatusLine(EventType2.Status, It.IsAny<string>(), false));
			this._fileHelper.Verify(x => x.Delete(filePath), Times.Never);
		}
	}
}