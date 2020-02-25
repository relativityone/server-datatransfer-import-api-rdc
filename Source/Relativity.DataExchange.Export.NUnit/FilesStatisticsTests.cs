// -----------------------------------------------------------------------------------------------------
// <copyright file="FilesStatisticsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;

	[TestFixture]
	public class FilesStatisticsTests
	{
		private FilesStatistics _instance;

		private kCura.WinEDDS.Statistics _statistics;
		private Mock<IFile> _fileHelper;
		private Mock<ITapiBridge> _tapiBridge;

		[SetUp]
		public void SetUp()
		{
			this._statistics = new kCura.WinEDDS.Statistics();
			this._fileHelper = new Mock<IFile>();
			this._tapiBridge = new Mock<ITapiBridge>();

			this._instance = new FilesStatistics(this._statistics, this._fileHelper.Object, new TestNullLogger());
		}

		[Test]
		public void ItShouldSaveAndRestoreState()
		{
			const long size1 = 525763;
			const long size2 = 627511;

			DateTime start = new DateTime(2017, 10, 10, 10, 10, 10);
			DateTime end = new DateTime(2017, 10, 10, 10, 10, 11);

			this._instance.Subscribe(this._tapiBridge.Object);

			// ACT
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, string.Empty, true, true, 0, size1, start, end));

			this._instance.SaveState();

			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, string.Empty, true, true, 0, size2, start, end));

			this._instance.RestoreLastState();

			// ASSERT
			Assert.That(this._statistics.FileTransferredBytes, Is.EqualTo(size1));
			Assert.That(this._statistics.FileTransferDuration.Ticks, Is.EqualTo(end.Ticks - start.Ticks));
		}

		[Test]
		public void ItShouldUpdateStatisticsForDownloadedFiles()
		{
			const long sizeDownload1 = 911681;
			const long sizeDownload2 = 586716;
			const long sizeNotDownload1 = 421857;

			DateTime start = new DateTime(2017, 10, 10, 10, 10, 10);
			DateTime end = new DateTime(2017, 10, 10, 10, 10, 11);

			this._instance.Subscribe(this._tapiBridge.Object);

			// ACT
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, string.Empty, true, true, 0, sizeDownload1, start, end));
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, string.Empty, false, false, 0, sizeNotDownload1, start, end));
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, string.Empty, true, true, 0, sizeDownload2, start, end));

			// ASSERT
			Assert.That(this._statistics.FileTransferredBytes, Is.EqualTo(sizeDownload1 + sizeDownload2));
			Assert.That(this._statistics.FileTransferDuration.Ticks, Is.EqualTo((end.Ticks - start.Ticks) * 2));
			Assert.That(this._statistics.NativeFilesTransferredCount, Is.EqualTo(2));
			Assert.That(this._statistics.MetadataFilesTransferredCount, Is.EqualTo(0));
		}

		[Test]
		public void ItShouldDetach()
		{
			const long size1 = 767403;
			const long size2 = 252593;

			DateTime start = new DateTime(2017, 10, 10, 10, 10, 10);
			DateTime end = new DateTime(2017, 10, 10, 10, 10, 11);

			this._instance.Subscribe(this._tapiBridge.Object);

			// ACT
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, string.Empty, true, true, 0, size1, start, end));

			this._instance.Unsubscribe(this._tapiBridge.Object);

			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, string.Empty, true, true, 0, size2, start, end));

			// ASSERT
			Assert.That(this._statistics.FileTransferredBytes, Is.EqualTo(size1));
			Assert.That(this._statistics.FileTransferDuration.Ticks, Is.EqualTo(end.Ticks - start.Ticks));
		}

		[Test]
		public void ItShouldAddSizeForFile()
		{
			const string fileName = "file_name";
			const long fileSize = 407452;

			this._fileHelper.Setup(x => x.Exists(fileName)).Returns(true);
			this._fileHelper.Setup(x => x.GetFileSize(fileName)).Returns(fileSize);

			// ACT
			this._instance.UpdateStatisticsForFile(fileName);

			// ASSERT
			Assert.That(this._statistics.FileTransferredBytes, Is.EqualTo(fileSize));
		}

		[Test]
		public void ItShouldHandleMissingFileWhenAddingStatistics()
		{
			const string fileName = "file_name";

			this._fileHelper.Setup(x => x.Exists(fileName)).Returns(false);

			// ACT & ASSERT
			Assert.DoesNotThrow(() => this._instance.UpdateStatisticsForFile(fileName));
		}
	}
}