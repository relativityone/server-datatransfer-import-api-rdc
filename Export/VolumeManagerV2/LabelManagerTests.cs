using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2
{
	[TestFixture]
	public class LabelManagerTests
	{
		private Mock<IVolume> _volumeMock;
		private Mock<ISubdirectory> _subdirectoryMock;

		[SetUp]
		public void SetUp()
		{
			_volumeMock = new Mock<IVolume>();
			_subdirectoryMock = new Mock<ISubdirectory>();
		}

		[Test]
		[TestCase(1, "VOL", 3, ExpectedResult = "VOL001")]
		[TestCase(123, "VOL", 1, ExpectedResult = "VOL123")]
		[TestCase(1, "VOL", 1, ExpectedResult = "VOL1")]
		[TestCase(23, "VOL", 0, ExpectedResult = "VOL23")]
		[TestCase(0, "VOL", 3, ExpectedResult = "VOL000")]
		[TestCase(23, "", 0, ExpectedResult = "23")]
		[TestCase(3, "", 5, ExpectedResult = "00003")]
		[TestCase(1, "DifferentPrefix", 3, ExpectedResult = "DifferentPrefix001")]
		public string ItShouldReturnValidVolumeLabel(int volumeNumber, string prefix, int padding)
		{
			ExportFile exportSettings = new ExportFile(1)
			{
				VolumeDigitPadding = padding,
				VolumeInfo = new VolumeInfo
				{
					VolumePrefix = prefix
				}
			};
			_volumeMock.SetupGet(x => x.CurrentVolumeNumber).Returns(volumeNumber);
			var instance = new LabelManager(exportSettings, _volumeMock.Object, _subdirectoryMock.Object);

			//ACT
			return instance.GetCurrentVolumeLabel();
		}

		[Test]
		[TestCase(1, "IMG", 3, ExpectedResult = "IMAGES\\IMG001")]
		[TestCase(123, "IMG", 1, ExpectedResult = "IMAGES\\IMG123")]
		[TestCase(1, "IMG", 1, ExpectedResult = "IMAGES\\IMG1")]
		[TestCase(23, "IMG", 0, ExpectedResult = "IMAGES\\IMG23")]
		[TestCase(0, "IMG", 3, ExpectedResult = "IMAGES\\IMG000")]
		[TestCase(23, "", 0, ExpectedResult = "IMAGES\\23")]
		[TestCase(3, "", 5, ExpectedResult = "IMAGES\\00003")]
		[TestCase(1, "DifferentPrefix", 3, ExpectedResult = "IMAGES\\DifferentPrefix001")]
		public string ItShouldReturnValidImageLabel(int subdirectoryNumber, string prefix, int padding)
		{
			ExportFile exportSettings = new ExportFile(1)
			{
				SubdirectoryDigitPadding = padding,
				VolumeInfo = new VolumeInfo()
			};
			exportSettings.VolumeInfo.set_SubdirectoryImagePrefix(false, prefix);

			_subdirectoryMock.SetupGet(x => x.CurrentSubdirectoryNumber).Returns(subdirectoryNumber);
			var instance = new LabelManager(exportSettings, _volumeMock.Object, _subdirectoryMock.Object);

			//ACT
			return instance.GetCurrentImageSubdirectoryLabel();
		}

		[Test]
		[TestCase(1, "NAT", 3, ExpectedResult = "NATIVES\\NAT001")]
		[TestCase(123, "NAT", 1, ExpectedResult = "NATIVES\\NAT123")]
		[TestCase(1, "NAT", 1, ExpectedResult = "NATIVES\\NAT1")]
		[TestCase(23, "NAT", 0, ExpectedResult = "NATIVES\\NAT23")]
		[TestCase(0, "NAT", 3, ExpectedResult = "NATIVES\\NAT000")]
		[TestCase(23, "", 0, ExpectedResult = "NATIVES\\23")]
		[TestCase(3, "", 5, ExpectedResult = "NATIVES\\00003")]
		[TestCase(1, "DifferentPrefix", 3, ExpectedResult = "NATIVES\\DifferentPrefix001")]
		public string ItShouldReturnValidNativeLabel(int subdirectoryNumber, string prefix, int padding)
		{
			ExportFile exportSettings = new ExportFile(1)
			{
				SubdirectoryDigitPadding = padding,
				VolumeInfo = new VolumeInfo()
			};
			exportSettings.VolumeInfo.set_SubdirectoryNativePrefix(false, prefix);

			_subdirectoryMock.SetupGet(x => x.CurrentSubdirectoryNumber).Returns(subdirectoryNumber);
			var instance = new LabelManager(exportSettings, _volumeMock.Object, _subdirectoryMock.Object);

			//ACT
			return instance.GetCurrentNativeSubdirectoryLabel();
		}

		[Test]
		[TestCase(1, "TXT", 3, ExpectedResult = "TEXT\\TXT001")]
		[TestCase(123, "TXT", 1, ExpectedResult = "TEXT\\TXT123")]
		[TestCase(1, "TXT", 1, ExpectedResult = "TEXT\\TXT1")]
		[TestCase(23, "TXT", 0, ExpectedResult = "TEXT\\TXT23")]
		[TestCase(0, "TXT", 3, ExpectedResult = "TEXT\\TXT000")]
		[TestCase(23, "", 0, ExpectedResult = "TEXT\\23")]
		[TestCase(3, "", 5, ExpectedResult = "TEXT\\00003")]
		[TestCase(1, "DifferentPrefix", 3, ExpectedResult = "TEXT\\DifferentPrefix001")]
		public string ItShouldReturnValidTextLabel(int subdirectoryNumber, string prefix, int padding)
		{
			ExportFile exportSettings = new ExportFile(1)
			{
				SubdirectoryDigitPadding = padding,
				VolumeInfo = new VolumeInfo()
			};
			exportSettings.VolumeInfo.set_SubdirectoryFullTextPrefix(false, prefix);

			_subdirectoryMock.SetupGet(x => x.CurrentSubdirectoryNumber).Returns(subdirectoryNumber);
			var instance = new LabelManager(exportSettings, _volumeMock.Object, _subdirectoryMock.Object);

			//ACT
			return instance.GetCurrentTextSubdirectoryLabel();
		}
	}
}