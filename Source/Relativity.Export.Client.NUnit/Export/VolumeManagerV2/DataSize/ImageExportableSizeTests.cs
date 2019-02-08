using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.DataSize;
using kCura.WinEDDS.Exporters;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.DataSize
{
	[TestFixture]
	public class ImageExportableSizeTests
	{
		private ExportFile _exportSettings;
		private VolumePredictions _volumePredictions;

		private ImageExportableSize _instance;

		private const long _IMAGE_FILE_SIZE = 505718;
		private const long _IMAGE_FILE_COUNT = 959625;
		private const double _PDF_MERGE_SIZE_ERROR_THRESHOLD = 1.03;

		[SetUp]
		public void SetUp()
		{
			_exportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo()
			};

			_volumePredictions = new VolumePredictions
			{
				ImageFileCount = _IMAGE_FILE_COUNT,
				ImageFilesSize = _IMAGE_FILE_SIZE
			};

			_instance = new ImageExportableSize(_exportSettings);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotExportingImages()
		{
			_exportSettings.ExportImages = false;
			_exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;

			//ACT
			_instance.CalculateImagesSize(_volumePredictions);

			//ASSERT
			Assert.That(_volumePredictions.ImageFileCount, Is.Zero);
			Assert.That(_volumePredictions.ImageFilesSize, Is.Zero);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotCopyingFiles()
		{
			_exportSettings.ExportImages = true;
			_exportSettings.VolumeInfo.CopyImageFilesFromRepository = false;

			//ACT
			_instance.CalculateImagesSize(_volumePredictions);

			//ASSERT
			Assert.That(_volumePredictions.ImageFileCount, Is.Zero);
			Assert.That(_volumePredictions.ImageFilesSize, Is.Zero);
		}

		[Test]
		[TestCase(ExportFile.ImageType.MultiPageTiff)]
		[TestCase(ExportFile.ImageType.SinglePage)]
		public void ItShouldNotChangeSizeWhenExportingTiffFiles(ExportFile.ImageType imageType)
		{
			_exportSettings.ExportImages = true;
			_exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			_exportSettings.TypeOfImage = imageType;

			//ACT
			_instance.CalculateImagesSize(_volumePredictions);

			//ASSERT
			Assert.That(_volumePredictions.ImageFilesSize, Is.EqualTo(_IMAGE_FILE_SIZE));
		}

		[Test]
		public void ItShouldIncreaseSizeWhenExportingImagesAsPdf()
		{
			_exportSettings.ExportImages = true;
			_exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			_exportSettings.TypeOfImage = ExportFile.ImageType.Pdf;

			//ACT
			_instance.CalculateImagesSize(_volumePredictions);

			//ASSERT
			Assert.That(_volumePredictions.ImageFilesSize, Is.EqualTo(Math.Ceiling(_IMAGE_FILE_SIZE * _PDF_MERGE_SIZE_ERROR_THRESHOLD)));
		}

		[Test]
		[TestCase(ExportFile.ImageType.MultiPageTiff)]
		[TestCase(ExportFile.ImageType.Pdf)]
		public void ItShouldChangeCountWhenMergingImages(ExportFile.ImageType imageType)
		{
			_exportSettings.ExportImages = true;
			_exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			_exportSettings.TypeOfImage = imageType;

			//ACT
			_instance.CalculateImagesSize(_volumePredictions);

			//ASSERT
			Assert.That(_volumePredictions.ImageFileCount, Is.EqualTo(1));
		}

		[Test]
		public void ItShouldNotChangeCountWhenNotMergingImages()
		{
			_exportSettings.ExportImages = true;
			_exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			_exportSettings.TypeOfImage = ExportFile.ImageType.SinglePage;

			//ACT
			_instance.CalculateImagesSize(_volumePredictions);

			//ASSERT
			Assert.That(_volumePredictions.ImageFileCount, Is.EqualTo(_IMAGE_FILE_COUNT));
		}

		[Test]
		[TestCase(ExportFile.ImageType.MultiPageTiff)]
		[TestCase(ExportFile.ImageType.Pdf)]
		public void ItShouldNotChangeCountWhenNoImagesToMerge(ExportFile.ImageType imageType)
		{
			_exportSettings.ExportImages = true;
			_exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			_exportSettings.TypeOfImage = imageType;
			_volumePredictions.ImageFileCount = 0;

			//ACT
			_instance.CalculateImagesSize(_volumePredictions);

			//ASSERT
			Assert.That(_volumePredictions.ImageFileCount, Is.Zero);
		}
	}
}