using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.TApi;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Statistics
{
	[TestFixture]
	public class MetadataStatisticsTests
	{
		private MetadataStatistics _instance;

		private WinEDDS.Statistics _statistics;
		private Mock<IFileHelper> _fileHelper;
		private Mock<ITapiBridge> _tapiBridge;

		[SetUp]
		public void SetUp()
		{
			_statistics = new WinEDDS.Statistics();
			_fileHelper = new Mock<IFileHelper>();
			_tapiBridge = new Mock<ITapiBridge>();

			_instance = new MetadataStatistics(_statistics, _fileHelper.Object, new NullLogger());
		}

		[Test]
		public void ItShouldSaveAndRestoreState()
		{
			const long size1 = 808873;
			const long size2 = 690132;

			DateTime start = new DateTime(2017, 10, 10, 10, 10, 10);
			DateTime end = new DateTime(2017, 10, 10, 10, 10, 11);

			_instance.Attach(_tapiBridge.Object);

			//ACT
			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs("", true, 0, size1, start, end));

			_instance.SaveState();

			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs("", true, 0, size2, start, end));

			_instance.RestoreLastState();

			//ASSERT
			Assert.That(_statistics.MetadataBytes, Is.EqualTo(size1));
			Assert.That(_statistics.MetadataTime, Is.EqualTo(end.Ticks - start.Ticks));
		}

		[Test]
		public void ItShouldUpdateStatisticsForDownloadedFiles()
		{
			const long sizeDownload1 = 881259;
			const long sizeDownload2 = 403944;
			const long sizeNotDownload1 = 127777;

			DateTime start = new DateTime(2017, 10, 10, 10, 10, 10);
			DateTime end = new DateTime(2017, 10, 10, 10, 10, 11);

			_instance.Attach(_tapiBridge.Object);

			//ACT
			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs("", true, 0, sizeDownload1, start, end));
			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs("", false, 0, sizeNotDownload1, start, end));
			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs("", true, 0, sizeDownload2, start, end));

			//ASSERT
			Assert.That(_statistics.MetadataBytes, Is.EqualTo(sizeDownload1 + sizeDownload2));
			Assert.That(_statistics.MetadataTime, Is.EqualTo((end.Ticks - start.Ticks) * 2));
		}

		[Test]
		public void ItShouldDetach()
		{
			const long size1 = 431644;
			const long size2 = 181259;

			DateTime start = new DateTime(2017, 10, 10, 10, 10, 10);
			DateTime end = new DateTime(2017, 10, 10, 10, 10, 11);

			_instance.Attach(_tapiBridge.Object);

			//ACT
			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs("", true, 0, size1, start, end));

			_instance.Detach();

			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs("", true, 0, size2, start, end));

			//ASSERT
			Assert.That(_statistics.MetadataBytes, Is.EqualTo(size1));
			Assert.That(_statistics.MetadataTime, Is.EqualTo(end.Ticks - start.Ticks));
		}

		[Test]
		public void ItShouldAddSizeForFile()
		{
			const string fileName = "file_name";
			const long fileSize = 101437;

			_fileHelper.Setup(x => x.Exists(fileName)).Returns(true);
			_fileHelper.Setup(x => x.GetFileSize(fileName)).Returns(fileSize);

			//ACT
			_instance.AddStatisticsForFile(fileName);

			//ASSERT
			Assert.That(_statistics.MetadataBytes, Is.EqualTo(fileSize));
		}

		[Test]
		public void ItShouldHandleMissingFileWhenAddingStatistics()
		{
			const string fileName = "file_name";

			_fileHelper.Setup(x => x.Exists(fileName)).Returns(false);

			//ACT & ASSERT
			Assert.DoesNotThrow(() => _instance.AddStatisticsForFile(fileName));
		}
	}
}