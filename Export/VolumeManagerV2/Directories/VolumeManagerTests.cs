using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Directories
{
	[TestFixture]
	public class VolumeManagerTests
	{
		private Core.Export.VolumeManagerV2.Directories.VolumeManager _instance;
		private Mock<ISubdirectoryManager> _subdirectoryManager;

		private const int _MBS_TO_BYTES = 1024 * 1024;

		[SetUp]
		public void SetUp()
		{
			_subdirectoryManager = new Mock<ISubdirectoryManager>();
		}

		private void SetUpInstance(int volumeStartNumber, long maxSizeInMBs)
		{
			ExportFile exportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo
				{
					VolumeStartNumber = volumeStartNumber,
					VolumeMaxSize = maxSizeInMBs
				}
			};
			_instance = new Core.Export.VolumeManagerV2.Directories.VolumeManager(exportSettings, _subdirectoryManager.Object);
		}

		[Test]
		[TestCase(1)]
		[TestCase(123456)]
		public void ItShouldStartNumberingFromGivenNumber(int volumeStartNumber)
		{
			SetUpInstance(volumeStartNumber, 1);

			//ASSERT
			Assert.That(_instance.CurrentVolumeNumber, Is.EqualTo(volumeStartNumber));
		}

		[Test]
		public void ItShouldValidateStartNumberAndSize()
		{
			Assert.Throws<ArgumentException>(() => SetUpInstance(0, 1));
			Assert.Throws<ArgumentException>(() => SetUpInstance(1, 0));
		}

		[Test]
		public void ItShouldResetSubdirectoryAfterMovingToNextVolume()
		{
			const int sizeInMBs = 10;
			SetUpInstance(1, sizeInMBs);

			VolumePredictions predictions = new VolumePredictions
			{
				NativeFilesSize = sizeInMBs * _MBS_TO_BYTES,
				ImageFilesSize = sizeInMBs * _MBS_TO_BYTES,
				TextFilesSize = sizeInMBs * _MBS_TO_BYTES
			};

			//ACT
			_instance.MoveNext(predictions);
			_instance.MoveNext(predictions);

			//ASSERT
			_subdirectoryManager.Verify(x => x.RestartSubdirectoryCounting(), Times.Once);
		}

		[Test]
		public void ItShouldNotMoveToNextVolumeIfFirstFileExceedsSize()
		{
			const int startNumber = 1;
			const int sizeInMBs = 10;
			SetUpInstance(startNumber, sizeInMBs);

			VolumePredictions predictions = new VolumePredictions
			{
				NativeFilesSize = sizeInMBs * _MBS_TO_BYTES / 2,
				ImageFilesSize = sizeInMBs * _MBS_TO_BYTES / 2,
				TextFilesSize = sizeInMBs * _MBS_TO_BYTES / 2
			};

			//ACT & ASSERT
			Assert.That(_instance.CurrentVolumeNumber, Is.EqualTo(startNumber));

			_instance.MoveNext(predictions);

			Assert.That(_instance.CurrentVolumeNumber, Is.EqualTo(startNumber));
		}

		[Test]
		public void ItShouldUseTotalFileSizeForCalculations()
		{
			const int startNumber = 1;
			const int sizeInMBs = 10;
			SetUpInstance(startNumber, sizeInMBs);

			VolumePredictions predictionsNative = new VolumePredictions
			{
				NativeFilesSize = sizeInMBs * _MBS_TO_BYTES  + 1
			};

			VolumePredictions predictionsImage = new VolumePredictions
			{
				ImageFilesSize = sizeInMBs * _MBS_TO_BYTES  + 1
			};

			VolumePredictions predictionsText = new VolumePredictions
			{
				TextFilesSize = sizeInMBs * _MBS_TO_BYTES + 1
			};

			//ACT & ASSERT
			_instance.MoveNext(predictionsNative);
			Assert.That(_instance.CurrentVolumeNumber, Is.EqualTo(startNumber));

			_instance.MoveNext(predictionsImage);
			Assert.That(_instance.CurrentVolumeNumber, Is.EqualTo(startNumber + 1));
			
			_instance.MoveNext(predictionsText);
			Assert.That(_instance.CurrentVolumeNumber, Is.EqualTo(startNumber + 2));
		}

		[Test]
		public void ItShouldCalculateValidVolumeNumber()
		{
			const int startNumber = 1;
			const int sizeInMBs = 10;
			SetUpInstance(startNumber, sizeInMBs);

			VolumePredictions predictionsNotExceeding = new VolumePredictions
			{
				NativeFilesSize = sizeInMBs * _MBS_TO_BYTES / 3
			};

			VolumePredictions predictionsNotExceeding2 = new VolumePredictions
			{
				ImageFilesSize = sizeInMBs * _MBS_TO_BYTES / 3
			};

			VolumePredictions predictionsExceeding = new VolumePredictions
			{
				TextFilesSize = sizeInMBs * _MBS_TO_BYTES / 2
			};

			VolumePredictions predictionsNotExceeding3 = new VolumePredictions
			{
				TextFilesSize = sizeInMBs * _MBS_TO_BYTES / 5
			};

			VolumePredictions predictionsExceeding2 = new VolumePredictions
			{
				TextFilesSize = sizeInMBs * _MBS_TO_BYTES / 2
			};

			//ACT & ASSERT
			_instance.MoveNext(predictionsNotExceeding);
			Assert.That(_instance.CurrentVolumeNumber, Is.EqualTo(startNumber));

			_instance.MoveNext(predictionsNotExceeding2);
			Assert.That(_instance.CurrentVolumeNumber, Is.EqualTo(startNumber));

			_instance.MoveNext(predictionsExceeding);
			Assert.That(_instance.CurrentVolumeNumber, Is.EqualTo(startNumber + 1));

			_instance.MoveNext(predictionsNotExceeding3);
			Assert.That(_instance.CurrentVolumeNumber, Is.EqualTo(startNumber + 1));

			_instance.MoveNext(predictionsExceeding2);
			Assert.That(_instance.CurrentVolumeNumber, Is.EqualTo(startNumber + 2));
		}
	}
}