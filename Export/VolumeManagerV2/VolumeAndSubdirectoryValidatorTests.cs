using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Exporters.Validator;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2
{
	[TestFixture]
	public class VolumeAndSubdirectoryValidatorTests
	{
		private VolumeAndSubdirectoryValidator _instance;

		private Mock<IUserNotification> _userNotificationMock;

		[SetUp]
		public void SetUp()
		{
			_userNotificationMock = new Mock<IUserNotification>();

			_instance = new VolumeAndSubdirectoryValidator(new PaddingWarningValidator(), _userNotificationMock.Object, new Mock<ILog>().Object);
		}

		[Test]
		public void ItShouldValidateVolumeAndSubdirectory()
		{
			_userNotificationMock.Setup(x => x.AlertWarningSkippable(It.IsAny<string>())).Returns(false);

			var exportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo
				{
					VolumeStartNumber = 1,
					SubdirectoryStartNumber = 1,
					CopyNativeFilesFromRepository = true
				},
				ExportNative = true
			};

			const int totalFiles = 1000;

			bool isValid = _instance.Validate(exportSettings, totalFiles);

			Assert.IsFalse(isValid);
		}

		[Test]
		public void Test()
		{
			//TODO
		}
	}
}