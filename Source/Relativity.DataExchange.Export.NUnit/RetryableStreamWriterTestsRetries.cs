// -----------------------------------------------------------------------------------------------------
// <copyright file="RetryableStreamWriterTestsRetries.cs" company="Relativity ODA LLC">
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
			Policy policy = Policy.Handle<ExportBaseException>().Retry(this.OnRetry);

			this._writersRetryPolicy = new Mock<IWritersRetryPolicy>();
			this._writersRetryPolicy.Setup(x => x.CreateRetryPolicy(It.IsAny<Action<Exception, TimeSpan, int, Context>>())).Returns((Action<Exception, TimeSpan, int, Context> action) =>
			{
				this._retryAction = action;
				return policy;
			});

			this._streamWriter = new StreamWriter(new StreamStub(1));
			this._streamFactory = new Mock<IStreamFactory>();
			this._streamFactory.Setup(x => x.Create(It.IsAny<StreamWriter>(), It.IsAny<long>(), _FILE_PATH, Encoding.Default, It.IsAny<bool>())).Returns(this._streamWriter);

			Mock<IDestinationPath> destinationPath = new Mock<IDestinationPath>();
			destinationPath.Setup(x => x.Path).Returns(_FILE_PATH);
			destinationPath.Setup(x => x.Encoding).Returns(Encoding.Default);

			this._processingStatistics = new Mock<IProcessingStatistics>();
			Mock<IStatus> status = new Mock<IStatus>();

			this._hasRetryBeenExecuted = false;

			this._instance = new RetryableStreamWriter(this._writersRetryPolicy.Object, this._streamFactory.Object, destinationPath.Object, this._processingStatistics.Object, status.Object, new NullLogger());
		}

		private void OnRetry(Exception arg1, int arg2)
		{
			this._retryAction.Invoke(new Exception(), TimeSpan.Zero, 1, new Context(string.Empty));
			this._hasRetryBeenExecuted = true;
		}

		[Test]
		public void ItShouldReinitializeBrokenStream()
		{
			const string loadFileEntry = "loadFileEntry";

			// ACT
			this._instance.WriteEntry(loadFileEntry, CancellationToken.None);
			this._instance.WriteEntry(loadFileEntry, CancellationToken.None);

			// ASSERT
			Assert.That(this._hasRetryBeenExecuted, Is.True);
			this._streamFactory.Verify(x => x.Create(this._streamWriter, loadFileEntry.Length, _FILE_PATH, Encoding.Default, true), Times.Once);
		}
	}
}