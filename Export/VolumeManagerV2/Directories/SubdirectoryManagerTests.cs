using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Directories
{
	[TestFixture]
	public class SubdirectoryManagerTests
	{
		private SubdirectoryManager _instance;

		private void SetUpSubdirectoryManager(int startNumber, long maxFiles)
		{
			ExportFile exportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo
				{
					SubdirectoryMaxSize = maxFiles,
					SubdirectoryStartNumber = startNumber
				}
			};
			_instance = new SubdirectoryManager(exportSettings);
		}

		[Test]
		public void ItShouldRestartSubdirectoryNumberingToSpecifiedStartNumber()
		{
			const int maxFileCount = 3;
			const int startNumber = 5;

			SetUpSubdirectoryManager(startNumber, maxFileCount);

			VolumePredictions predictions = new VolumePredictions
			{
				NativeFileCount = 10
			};

			//ACT
			_instance.MoveNext(predictions);
			_instance.MoveNext(predictions);

			_instance.RestartSubdirectoryCounting();

			//ASSERT
			Assert.That(_instance.CurrentSubdirectoryNumber, Is.EqualTo(startNumber));
		}

		[Test]
		[TestCase(2, 0, 0)]
		[TestCase(0, 2, 0)]
		[TestCase(0, 0, 2)]
		public void ItShouldMoveToNextDirectoryWhenSpecificFilesCountExceedsLimit(int natives, int images, int text)
		{
			const int maxFileCount = 3;
			const int startNumber = 5;

			SetUpSubdirectoryManager(startNumber, maxFileCount);

			VolumePredictions predictions = new VolumePredictions
			{
				NativeFileCount = natives,
				ImageFileCount = images,
				TextFileCount = text
			};

			//ACT
			_instance.MoveNext(predictions);
			_instance.MoveNext(predictions);

			//ASSERT
			Assert.That(_instance.CurrentSubdirectoryNumber, Is.EqualTo(startNumber + 1));
		}

		[Test]
		public void ItShouldTreatDifferentFilesSeparately()
		{
			const int maxFileCount = 3;
			const int startNumber = 5;

			SetUpSubdirectoryManager(startNumber, maxFileCount);

			VolumePredictions predictionsNatives = new VolumePredictions
			{
				NativeFileCount = 2,
				ImageFileCount = 0,
				TextFileCount = 0
			};
			VolumePredictions predictionsImages = new VolumePredictions
			{
				NativeFileCount = 0,
				ImageFileCount = 2,
				TextFileCount = 0
			};
			VolumePredictions predictionsText = new VolumePredictions
			{
				NativeFileCount = 0,
				ImageFileCount = 0,
				TextFileCount = 2
			};

			//ACT
			_instance.MoveNext(predictionsNatives);
			_instance.MoveNext(predictionsImages);
			_instance.MoveNext(predictionsText);

			//ASSERT
			Assert.That(_instance.CurrentSubdirectoryNumber, Is.EqualTo(startNumber));
		}

		[Test]
		[TestCase(5, 0, 0)]
		[TestCase(0, 5, 0)]
		[TestCase(0, 0, 5)]
		public void ItShouldNotMoveToNextDirectoryForFirstPrediction(int natives, int images, int text)
		{
			const int maxFileCount = 1;
			const int startNumber = 5;

			SetUpSubdirectoryManager(startNumber, maxFileCount);

			VolumePredictions predictions = new VolumePredictions
			{
				NativeFileCount = natives,
				ImageFileCount = images,
				TextFileCount = text
			};

			//ACT
			_instance.MoveNext(predictions);

			//ASSERT
			Assert.That(_instance.CurrentSubdirectoryNumber, Is.EqualTo(startNumber));
		}

		[Test]
		[TestCase(5, 0, 0)]
		[TestCase(0, 5, 0)]
		[TestCase(0, 0, 5)]
		public void ItShouldNotMoveToNextDirectoryForFirstPredictionAfterRestart(int natives, int images, int text)
		{
			const int maxFileCount = 1;
			const int startNumber = 5;

			SetUpSubdirectoryManager(startNumber, maxFileCount);

			VolumePredictions predictions = new VolumePredictions
			{
				NativeFileCount = natives,
				ImageFileCount = images,
				TextFileCount = text
			};

			//ACT
			_instance.MoveNext(predictions);
			_instance.MoveNext(predictions);

			_instance.RestartSubdirectoryCounting();

			_instance.MoveNext(predictions);

			//ASSERT
			Assert.That(_instance.CurrentSubdirectoryNumber, Is.EqualTo(startNumber));
		}
	}
}