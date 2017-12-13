using kCura.WinEDDS.Core.Export.VolumeManagerV2.DataSize;
using kCura.WinEDDS.Exporters;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.DataSize
{
	[TestFixture]
	public class TextExportableSizeTests
	{
		private ExportFile _exportSettings;
		private VolumePredictions _volumePredictions;

		private TextExportableSize _instance;

		private const long _TEXT_FILE_COUNT = 660488;
		private const long _TEXT_FILE_SIZE = 444442;

		[SetUp]
		public void SetUp()
		{
			_exportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo()
			};

			_volumePredictions = new VolumePredictions
			{
				TextFileCount = _TEXT_FILE_COUNT,
				TextFilesSize = _TEXT_FILE_SIZE
			};

			_instance = new TextExportableSize(_exportSettings, null);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotExportingText()
		{
			_exportSettings.ExportFullText = false;
			_exportSettings.ExportFullTextAsFile = true;

			//ACT
			_instance.CalculateTextSize(_volumePredictions, null);

			//ASSERT
			Assert.That(_volumePredictions.TextFileCount, Is.Zero);
			Assert.That(_volumePredictions.TextFilesSize, Is.Zero);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotExportingTextAsFile()
		{
			_exportSettings.ExportFullText = true;
			_exportSettings.ExportFullTextAsFile = false;

			//ACT
			_instance.CalculateTextSize(_volumePredictions, null);

			//ASSERT
			Assert.That(_volumePredictions.TextFileCount, Is.Zero);
			Assert.That(_volumePredictions.TextFilesSize, Is.Zero);
		}

		[Test]
		public void Todo()
		{
			//TODO
		}
	}
}