using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download
{
	[TestFixture]
	public class PhysicalFilesDownloaderTests
	{
		private Mock<IAsperaCredentialsService> _credentialsService;
		private Mock<IExportTapiBridgeFactory> _exportTapiBridgeFactory;
		private Mock<ILog> _logger;
		private SafeIncrement _safeIncrement;

		[SetUp]
		public void SetUp()
		{
			_credentialsService = new Mock<IAsperaCredentialsService>();
			_exportTapiBridgeFactory = new Mock<IExportTapiBridgeFactory>();
			_logger = new Mock<ILog>();
			_safeIncrement = new SafeIncrement();
		}

		public void ItShouldCreateThreeTapiBridgesAndGroupFilesCorrectly()
		{
			//_credentialsService.Setup(s => s.GetAsperaCredentialsForFileshare(It.Is<Uri>(u => )))
		}
	}
}
