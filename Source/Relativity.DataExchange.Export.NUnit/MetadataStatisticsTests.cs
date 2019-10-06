// -----------------------------------------------------------------------------------------------------
// <copyright file="MetadataStatisticsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	[TestFixture]
	public class MetadataStatisticsTests
	{
		private MetadataStatistics _instance;

		private kCura.WinEDDS.Statistics _statistics;
		private Mock<IFile> _fileHelper;
		private Mock<ITapiBridge> _tapiBridge;

		[SetUp]
		public void SetUp()
		{
			this._statistics = new kCura.WinEDDS.Statistics();
			this._fileHelper = new Mock<IFile>();
			this._tapiBridge = new Mock<ITapiBridge>();

			this._instance = new MetadataStatistics(this._statistics, this._fileHelper.Object, new NullLogger());
		}

		[Test]
		public void ItShouldSaveAndRestoreState()
		{
			const long size1 = 808873;
			const long size2 = 690132;

			DateTime start = new DateTime(2017, 10, 10, 10, 10, 10);
			DateTime end = new DateTime(2017, 10, 10, 10, 10, 11);

			this._instance.Subscribe(this._tapiBridge.Object);

			// ACT
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, true, true, 0, size1, start, end));

			this._instance.SaveState();

			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, true, true, 0, size2, start, end));

			this._instance.RestoreLastState();

			// ASSERT
			Assert.That(this._statistics.MetadataBytes, Is.EqualTo(size1));
			Assert.That(this._statistics.MetadataTime, Is.EqualTo(end.Ticks - start.Ticks));
			Assert.That(this._statistics.TotalMetadataFilesTransferred, Is.EqualTo(2));
			Assert.That(this._statistics.TotalNativeFilesTransferred, Is.EqualTo(0));
		}

		[Test]
		public void ItShouldUpdateStatisticsForDownloadedFiles()
		{
			const long sizeDownload1 = 881259;
			const long sizeDownload2 = 403944;
			const long sizeNotDownload1 = 127777;

			DateTime start = new DateTime(2017, 10, 10, 10, 10, 10);
			DateTime end = new DateTime(2017, 10, 10, 10, 10, 11);

			this._instance.Subscribe(this._tapiBridge.Object);

			// ACT
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, true, true, 0, sizeDownload1, start, end));
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, false, false, 0, sizeNotDownload1, start, end));
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, true, true, 0, sizeDownload2, start, end));

			// ASSERT
			Assert.That(this._statistics.MetadataBytes, Is.EqualTo(sizeDownload1 + sizeDownload2));
			Assert.That(this._statistics.MetadataTime, Is.EqualTo((end.Ticks - start.Ticks) * 2));
		}

		[Test]
		public void ItShouldDetach()
		{
			const long size1 = 431644;
			const long size2 = 181259;

			DateTime start = new DateTime(2017, 10, 10, 10, 10, 10);
			DateTime end = new DateTime(2017, 10, 10, 10, 10, 11);

			this._instance.Subscribe(this._tapiBridge.Object);

			// ACT
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, true, true, 0, size1, start, end));

			this._instance.Unsubscribe(this._tapiBridge.Object);

			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(string.Empty, true, true, 0, size2, start, end));

			// ASSERT
			Assert.That(this._statistics.MetadataBytes, Is.EqualTo(size1));
			Assert.That(this._statistics.MetadataTime, Is.EqualTo(end.Ticks - start.Ticks));
		}

		[Test]
		public void ItShouldUpdateSizeForFile()
		{
			const string fileName = "file_name";
			const long fileSize = 101437;
			const long newFileSize = 803761;

			this._fileHelper.Setup(x => x.Exists(fileName)).Returns(true);
			this._fileHelper.Setup(x => x.GetFileSize(fileName))
				.Returns(new Queue<long>(new[] { fileSize, newFileSize }).Dequeue);

			// ACT
			this._instance.UpdateStatisticsForFile(fileName);
			this._instance.UpdateStatisticsForFile(fileName);

			// ASSERT
			Assert.That(this._statistics.MetadataBytes, Is.EqualTo(newFileSize));
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