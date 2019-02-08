using System;
using System.IO;
using System.Text;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using Moq;
using NUnit.Framework;
using Polly;
using Polly.NoOp;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Writers
{
	[TestFixture]
	public class RetryableStreamWriterTests
	{
		private RetryableStreamWriter _instance;

		private StreamWriter _streamWriter;

		private Mock<IWritersRetryPolicy> _writersRetryPolicy;
		private Mock<IStreamFactory> _streamFactory;
		private Mock<IProcessingStatistics> _processingStatistics;

		private const string _FILE_PATH = "file_path";

		[SetUp]
		public void SetUp()
		{
			_writersRetryPolicy = new Mock<IWritersRetryPolicy>();
			NoOpPolicy noOpPolicy = Policy.NoOp();
			_writersRetryPolicy.Setup(x => x.CreateRetryPolicy(It.IsAny<Action<Exception, TimeSpan, int, Context>>())).Returns(noOpPolicy);

			_streamWriter = new StreamWriter(new MemoryStream());
			_streamFactory = new Mock<IStreamFactory>();
			_streamFactory.Setup(x => x.Create(null, 0, _FILE_PATH, Encoding.Default, false)).Returns(_streamWriter);

			Mock<IDestinationPath> destinationPath = new Mock<IDestinationPath>();
			destinationPath.Setup(x => x.Path).Returns(_FILE_PATH);
			destinationPath.Setup(x => x.Encoding).Returns(Encoding.Default);

			_processingStatistics = new Mock<IProcessingStatistics>();
			Mock<IStatus> status = new Mock<IStatus>();


			_instance = new RetryableStreamWriter(_writersRetryPolicy.Object, _streamFactory.Object, destinationPath.Object, _processingStatistics.Object, status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldSkipRestoringWhenStreamHasNotBeenInitialized()
		{
			//ACT
			_instance.SaveState();
			_instance.RestoreLastState();

			//ASSERT
			_streamFactory.Verify(x => x.Create(It.IsAny<StreamWriter>(), It.IsAny<long>(), _FILE_PATH, Encoding.Default, true), Times.Never);
		}

		[Test]
		public void ItShouldSaveAndRestoreLastPosition()
		{
			const string loadFileEntry = "loadFileEntry";

			//ACT
			_instance.WriteEntry(loadFileEntry, CancellationToken.None);
			_instance.SaveState();
			_instance.WriteEntry("additional_entry", CancellationToken.None);
			_instance.RestoreLastState();

			//ASSERT
			_streamFactory.Verify(x => x.Create(_streamWriter, loadFileEntry.Length, _FILE_PATH, Encoding.Default, true), Times.Once);
		}

		[Test]
		public void ItShouldUpdateStatistics()
		{
			//ACT
			_instance.WriteEntry("load_file_entry", CancellationToken.None);

			//ASSERT
			_processingStatistics.Verify(x => x.UpdateStatisticsForFile(_FILE_PATH), Times.Once);
		}
	}
}