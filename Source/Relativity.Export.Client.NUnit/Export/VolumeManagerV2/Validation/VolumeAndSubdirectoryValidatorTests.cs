using kCura.WinEDDS.Core.Export.VolumeManagerV2.Validation;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Exporters.Validator;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Validation
{
	using System.Collections.Generic;

	[TestFixture]
	public class VolumeAndSubdirectoryValidatorTests
	{
		private Mock<IUserNotification> _userNotificationMock;
		private VolumeAndSubdirectoryValidator _instance;

		private static List<TestCaseData> _volumePaddingValidationTestCaseData = new List<TestCaseData>()
		{
			new TestCaseData(0, 0, 0).Returns(false),
			new TestCaseData(0, 0, 1).Returns(true),
			new TestCaseData(0, 1, 1).Returns(true),
			new TestCaseData(50, 49, 2).Returns(true),
			new TestCaseData(50, 50, 2).Returns(false),
			new TestCaseData(100, 1, 1).Returns(false),
			new TestCaseData(99, 1, 2).Returns(false),
			new TestCaseData(99, 1, 3).Returns(true),
			new TestCaseData(100, 100, 3).Returns(true),
			new TestCaseData(100, 1000, 3).Returns(false),
			new TestCaseData(100, 899, 3).Returns(true),
			new TestCaseData(100, 900, 3).Returns(false),
			new TestCaseData(4000, 5000, 4).Returns(true),
			new TestCaseData(0, 99999, 5).Returns(true),
			new TestCaseData(1, 99999, 5).Returns(false),
			new TestCaseData(1, 99999, 6).Returns(true),
			new TestCaseData(99999, 0, 5).Returns(true),
			new TestCaseData(99999, 1, 5).Returns(false),
			new TestCaseData(99999, 1, 6).Returns(true),
		};

		private const int _MAX_DIGIT_PADDING = 10;
		private const int _DEFAULT_NUMBER_OF_FILES = 1000;

		[SetUp]
		public void SetUp()
		{
			_userNotificationMock = new Mock<IUserNotification>();
			_userNotificationMock.Setup(x => x.AlertWarningSkippable(It.IsAny<string>())).Returns(false);

			_instance = new VolumeAndSubdirectoryValidator(new PaddingWarningValidator(), _userNotificationMock.Object, new Mock<ILog>().Object);
		}

		[Test]
		[TestCaseSource(nameof(_volumePaddingValidationTestCaseData))]
		public bool ItShouldValidateVolumePadding(int volumeStartNumber, int totalFiles, int configuredPadding)
		{
			return ValidateVolumeAndSubdirectoryPadding(volumeStartNumber, 0, totalFiles, configuredPadding, _MAX_DIGIT_PADDING);
		}

		[Test]
		[TestCaseSource(nameof(_volumePaddingValidationTestCaseData))]
		public bool ItShouldValidateSubdirectoryPadding(int subdirectoryStartNumber, int totalFiles, int configuredPadding)
		{
			return ValidateVolumeAndSubdirectoryPadding(0, subdirectoryStartNumber, totalFiles, _MAX_DIGIT_PADDING, configuredPadding);
		}

		[Test]
		public void ItShouldNotValidatePaddingWhenNoPhysicalFilesAreExported()
		{
			ExportFile exportSettings = CreateExportSettings(false);

			bool result = _instance.Validate(exportSettings, _DEFAULT_NUMBER_OF_FILES);

			Assert.That(result, Is.True);
		}

		[Test]
		public void ItShouldNotValidatePaddingWhenWarningIsSkippable()
		{
			_userNotificationMock.Setup(x => x.AlertWarningSkippable(It.IsAny<string>())).Returns(true);

			ExportFile exportSettings = CreateExportSettings(true);

			bool result = _instance.Validate(exportSettings, _DEFAULT_NUMBER_OF_FILES);

			Assert.That(result, Is.True);
		}

		private static ExportFile CreateExportSettings(bool exportFiles, int volumeStartNumber = 0, int subdirectoryStartNumber = 0, int configuredVolumePadding = 0, int configuredSubdirectoryPadding = 0)
		{
			var exportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo
				{
					CopyNativeFilesFromRepository = exportFiles,
					VolumeStartNumber = volumeStartNumber,
					SubdirectoryStartNumber = subdirectoryStartNumber
				},

				ExportNative = exportFiles,
				VolumeDigitPadding = configuredVolumePadding,
				SubdirectoryDigitPadding = configuredSubdirectoryPadding
			};
			return exportSettings;
		}

		private bool ValidateVolumeAndSubdirectoryPadding(int volumeStartNumber, int subdirectoryStartNumber, int totalFiles, int configuredVolumeDigitPadding, int configuredSubdirectoryDigitPadding)
		{
			ExportFile exportSettings = CreateExportSettings(true, volumeStartNumber, subdirectoryStartNumber, configuredVolumeDigitPadding, configuredSubdirectoryDigitPadding);

			return _instance.Validate(exportSettings, totalFiles);
		}

	}
}