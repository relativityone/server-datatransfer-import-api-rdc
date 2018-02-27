using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using Moq;
using NUnit.Framework;
using Relativity.Logging;
using Relativity.Transfer;

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

		public void ItShouldCreateTapiBridgesAccordinglyAndGroupFilesCorrectly()
		{
			string fileshareOne = "fileshare.one";
			string fileshareTwo = "fileshare.one";
			string fileshareThree = "fileshare.one";

			Credential credentialOne = new Credential();
			Credential credentialTwo = new Credential();
			Credential credentialThree = new Credential();

			_credentialsService.Setup(s => s.GetAsperaCredentialsForFileshare(It.Is<Uri>(u => new Uri(fileshareOne).IsBaseOf(u)))).Returns(credentialOne);
			_credentialsService.Setup(s => s.GetAsperaCredentialsForFileshare(It.Is<Uri>(u => new Uri(fileshareTwo).IsBaseOf(u)))).Returns(credentialTwo);
			_credentialsService.Setup(s => s.GetAsperaCredentialsForFileshare(It.Is<Uri>(u => new Uri(fileshareThree).IsBaseOf(u)))).Returns(credentialThree);

			
		}
	}
}
