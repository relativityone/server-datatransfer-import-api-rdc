// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageExportableSizeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.DataSize;

	[TestFixture]
	public class ImageExportableSizeTests
	{
		private const long _IMAGE_FILE_SIZE = 505718;
		private const long _IMAGE_FILE_COUNT = 959625;
		private const double _PDF_MERGE_SIZE_ERROR_THRESHOLD = 1.03;
		private ExportFile _exportSettings;
		private VolumePredictions _volumePredictions;
		private ImageExportableSize _instance;

		[SetUp]
		public void SetUp()
		{
			this._exportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo()
			};

			this._volumePredictions = new VolumePredictions
			{
				ImageFileCount = _IMAGE_FILE_COUNT,
				ImageFilesSize = _IMAGE_FILE_SIZE
			};

			this._instance = new ImageExportableSize(this._exportSettings);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotExportingImages()
		{
			this._exportSettings.ExportImages = false;
			this._exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;

			// ACT
			this._instance.CalculateImagesSize(this._volumePredictions);

			// ASSERT
			Assert.That(this._volumePredictions.ImageFileCount, Is.Zero);
			Assert.That(this._volumePredictions.ImageFilesSize, Is.Zero);
		}

		[Test]
		public void ItShouldResetSizeAndCountWhenNotCopyingFiles()
		{
			this._exportSettings.ExportImages = true;
			this._exportSettings.VolumeInfo.CopyImageFilesFromRepository = false;

			// ACT
			this._instance.CalculateImagesSize(this._volumePredictions);

			// ASSERT
			Assert.That(this._volumePredictions.ImageFileCount, Is.Zero);
			Assert.That(this._volumePredictions.ImageFilesSize, Is.Zero);
		}

		[Test]
		[TestCase(ExportFile.ImageType.MultiPageTiff)]
		[TestCase(ExportFile.ImageType.SinglePage)]
		public void ItShouldNotChangeSizeWhenExportingTiffFiles(ExportFile.ImageType imageType)
		{
			this._exportSettings.ExportImages = true;
			this._exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			this._exportSettings.TypeOfImage = imageType;

			// ACT
			this._instance.CalculateImagesSize(this._volumePredictions);

			// ASSERT
			Assert.That(this._volumePredictions.ImageFilesSize, Is.EqualTo(_IMAGE_FILE_SIZE));
		}

		[Test]
		public void ItShouldIncreaseSizeWhenExportingImagesAsPdf()
		{
			this._exportSettings.ExportImages = true;
			this._exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			this._exportSettings.TypeOfImage = ExportFile.ImageType.Pdf;

			// ACT
			this._instance.CalculateImagesSize(this._volumePredictions);

			// ASSERT
			Assert.That(this._volumePredictions.ImageFilesSize, Is.EqualTo(Math.Ceiling(_IMAGE_FILE_SIZE * _PDF_MERGE_SIZE_ERROR_THRESHOLD)));
		}

		[Test]
		[TestCase(ExportFile.ImageType.MultiPageTiff)]
		[TestCase(ExportFile.ImageType.Pdf)]
		public void ItShouldChangeCountWhenMergingImages(ExportFile.ImageType imageType)
		{
			this._exportSettings.ExportImages = true;
			this._exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			this._exportSettings.TypeOfImage = imageType;

			// ACT
			this._instance.CalculateImagesSize(this._volumePredictions);

			// ASSERT
			Assert.That(this._volumePredictions.ImageFileCount, Is.EqualTo(1));
		}

		[Test]
		public void ItShouldNotChangeCountWhenNotMergingImages()
		{
			this._exportSettings.ExportImages = true;
			this._exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			this._exportSettings.TypeOfImage = ExportFile.ImageType.SinglePage;

			// ACT
			this._instance.CalculateImagesSize(this._volumePredictions);

			// ASSERT
			Assert.That(this._volumePredictions.ImageFileCount, Is.EqualTo(_IMAGE_FILE_COUNT));
		}

		[Test]
		[TestCase(ExportFile.ImageType.MultiPageTiff)]
		[TestCase(ExportFile.ImageType.Pdf)]
		public void ItShouldNotChangeCountWhenNoImagesToMerge(ExportFile.ImageType imageType)
		{
			this._exportSettings.ExportImages = true;
			this._exportSettings.VolumeInfo.CopyImageFilesFromRepository = true;
			this._exportSettings.TypeOfImage = imageType;
			this._volumePredictions.ImageFileCount = 0;

			// ACT
			this._instance.CalculateImagesSize(this._volumePredictions);

			// ASSERT
			Assert.That(this._volumePredictions.ImageFileCount, Is.Zero);
		}
	}
}