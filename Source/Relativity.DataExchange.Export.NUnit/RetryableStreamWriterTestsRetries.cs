﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="RetryableStreamWriterTestsRetries.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;

    using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exceptions;

	using Moq;

    using Polly;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.Logging;

	[TestFixture]
	public class RetryableStreamWriterTestsRetries
	{
		private const string _FILE_PATH = "file_path";
		private RetryableStreamWriter _instance;
		private StreamWriter _streamWriter;
		private Mock<IWritersRetryPolicy> _writersRetryPolicy;
		private Mock<IStreamFactory> _streamFactory;
		private Mock<IProcessingStatistics> _processingStatistics;
		private Action<Exception, TimeSpan, int, Context> _retryAction;
		private bool _hasRetryBeenExecuted;

		[SetUp]
		public void SetUp()
		{
			Policy policy = Policy.Handle<ExportBaseException>().Retry(OnRetry);

			_writersRetryPolicy = new Mock<IWritersRetryPolicy>();
			_writersRetryPolicy.Setup(x => x.CreateRetryPolicy(It.IsAny<Action<Exception, TimeSpan, int, Context>>())).Returns((Action<Exception, TimeSpan, int, Context> action) =>
			{
				_retryAction = action;
				return policy;
			});

			_streamWriter = new StreamWriter(new StreamStub(1));
			_streamFactory = new Mock<IStreamFactory>();
			_streamFactory.Setup(x => x.Create(It.IsAny<StreamWriter>(), It.IsAny<long>(), _FILE_PATH, Encoding.Default, It.IsAny<bool>())).Returns(_streamWriter);

			Mock<IDestinationPath> destinationPath = new Mock<IDestinationPath>();
			destinationPath.Setup(x => x.Path).Returns(_FILE_PATH);
			destinationPath.Setup(x => x.Encoding).Returns(Encoding.Default);

			_processingStatistics = new Mock<IProcessingStatistics>();
			Mock<IStatus> status = new Mock<IStatus>();

			_hasRetryBeenExecuted = false;

			_instance = new RetryableStreamWriter(_writersRetryPolicy.Object, _streamFactory.Object, destinationPath.Object, _processingStatistics.Object, status.Object, new NullLogger());
		}

		private void OnRetry(Exception arg1, int arg2)
		{
			_retryAction.Invoke(new Exception(), TimeSpan.Zero, 1, new Context(string.Empty));
			_hasRetryBeenExecuted = true;
		}

		[Test]
		public void ItShouldReinitializeBrokenStream()
		{
			const string loadFileEntry = "loadFileEntry";

			// ACT
			_instance.WriteEntry(loadFileEntry, CancellationToken.None);
			_instance.WriteEntry(loadFileEntry, CancellationToken.None);

			// ASSERT
			Assert.That(_hasRetryBeenExecuted, Is.True);
			_streamFactory.Verify(x => x.Create(_streamWriter, loadFileEntry.Length, _FILE_PATH, Encoding.Default, true), Times.Once);
		}
	}
}