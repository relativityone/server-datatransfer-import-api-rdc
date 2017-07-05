using System.IO;
using kCura.Utility;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Statistics;
using kCura.WinEDDS.Core.Import.Status;
using Moq;
using NUnit.Framework;
using Relativity;
using Relativity.Logging;
using File = System.IO.File;

namespace kCura.WinEDDS.Core.NUnit.Import.Errors
{
	[TestFixture]
	public class ServerErrorFileDownloaderTests
	{
		private string _filePath;
		private GenericCsvReader _reader;

		private ServerErrorFileDownloader _instance;

		private Mock<IErrorFileDownloader> _errorFileDownloader;
		private readonly Mock<ILog> _logMock = new Mock<ILog>();

		[SetUp]
		public void SetUp()
		{
			_filePath = Path.GetTempFileName();

			_errorFileDownloader = new Mock<IErrorFileDownloader>();

			var statisticsHandler = new Mock<IServerErrorStatisticsHandler>();

			var pathHelper = new Mock<IPathHelper>();
			pathHelper.Setup(x => x.GetTempFileName()).Returns(_filePath);

			var errorFileDownloaderFactory = new Mock<IErrorFileDownloaderFactory>();
			errorFileDownloaderFactory.Setup(x => x.Create(It.IsAny<CaseInfo>())).Returns(_errorFileDownloader.Object);

			_instance = new ServerErrorFileDownloader(pathHelper.Object, errorFileDownloaderFactory.Object, statisticsHandler.Object, _logMock.Object);
		}

		[TearDown]
		public void TearDown()
		{
			_reader?.Close();
			if (File.Exists(_filePath))
			{
				File.Delete(_filePath);
			}
		}

		[Test]
		public void ItShouldDownloadErrorFileFromServer()
		{
			var logKey = "log_key";
			var caseInfo = new CaseInfo();

			File.WriteAllText(_filePath, "anything");

			// ACT
			_reader = _instance.DownloadErrorFile(logKey, caseInfo);

			// ASSERT
			Assert.That(_reader, Is.Not.Null);

			_errorFileDownloader.Verify(x => x.MoveTempFileToLocal(_filePath, logKey, caseInfo, false), Times.Once);
			_errorFileDownloader.Verify(x => x.RemoveRemoteTempFile(logKey, caseInfo), Times.Once);
		}

		[Test]
		public void ItShouldRetryDownload()
		{
			const int retries = 3;

			var logKey = "log_key";
			var caseInfo = new CaseInfo();

			// ACT
			_reader = _instance.DownloadErrorFile(logKey, caseInfo);

			// ASSERT
			Assert.That(_reader, Is.Null);

			_errorFileDownloader.Verify(x => x.MoveTempFileToLocal(_filePath, logKey, caseInfo, false), Times.Exactly(retries));
			_errorFileDownloader.Verify(x => x.RemoveRemoteTempFile(logKey, caseInfo), Times.Once);
		}
	}
}