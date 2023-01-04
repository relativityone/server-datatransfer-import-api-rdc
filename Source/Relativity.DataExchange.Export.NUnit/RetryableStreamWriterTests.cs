// -----------------------------------------------------------------------------------------------------
// <copyright file="RetryableStreamWriterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.IO;
	using System.Text;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Polly;
	using Polly.NoOp;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class RetryableStreamWriterTests
	{
		private const string _FILE_PATH = "file_path";
		private RetryableStreamWriter _instance;
		private StreamWriter _streamWriter;
		private Mock<IWritersRetryPolicy> _writersRetryPolicy;
		private Mock<IStreamFactory> _streamFactory;
		private Mock<IProcessingStatistics> _processingStatistics;

		[SetUp]
		public void SetUp()
		{
			this._writersRetryPolicy = new Mock<IWritersRetryPolicy>();
			NoOpPolicy noOpPolicy = Policy.NoOp();
			this._writersRetryPolicy.Setup(x => x.CreateRetryPolicy(It.IsAny<Action<Exception, TimeSpan, int, Context>>())).Returns(noOpPolicy);

			this._streamWriter = new StreamWriter(new MemoryStream());
			this._streamFactory = new Mock<IStreamFactory>();
			this._streamFactory.Setup(x => x.Create(null, 0, _FILE_PATH, Encoding.Default, false)).Returns(this._streamWriter);

			Mock<IDestinationPath> destinationPath = new Mock<IDestinationPath>();
			destinationPath.Setup(x => x.Path).Returns(_FILE_PATH);
			destinationPath.Setup(x => x.Encoding).Returns(Encoding.Default);

			this._processingStatistics = new Mock<IProcessingStatistics>();
			Mock<IStatus> status = new Mock<IStatus>();

			this._instance = new RetryableStreamWriter(this._writersRetryPolicy.Object, this._streamFactory.Object, destinationPath.Object, this._processingStatistics.Object, status.Object, new TestNullLogger());
		}

		[Test]
		public void ItShouldSkipRestoringWhenStreamHasNotBeenInitialized()
		{
			// ACT
			this._instance.SaveState();
			this._instance.RestoreLastState();

			// ASSERT
			this._streamFactory.Verify(x => x.Create(It.IsAny<StreamWriter>(), It.IsAny<long>(), _FILE_PATH, Encoding.Default, true), Times.Never);
		}

		[Test]
		public void ItShouldSaveAndRestoreLastPosition()
		{
			const string loadFileEntry = "loadFileEntry";

			// ACT
			this._instance.WriteEntry(loadFileEntry, CancellationToken.None);
			this._instance.SaveState();
			this._instance.WriteEntry("additional_entry", CancellationToken.None);
			this._instance.RestoreLastState();

			// ASSERT
			this._streamFactory.Verify(x => x.Create(this._streamWriter, loadFileEntry.Length, _FILE_PATH, Encoding.Default, true), Times.Once);
		}

		[Test]
		public void ItShouldUpdateStatistics()
		{
			// ACT
			this._instance.WriteEntry("load_file_entry", CancellationToken.None);

			// ASSERT
			this._processingStatistics.Verify(x => x.UpdateStatisticsForFile(_FILE_PATH), Times.Once);
		}
	}
}