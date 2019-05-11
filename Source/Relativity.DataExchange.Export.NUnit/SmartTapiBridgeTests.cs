// -----------------------------------------------------------------------------------------------------
// <copyright file="SmartTapiBridgeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using global::NUnit.Framework;

	using kCura.WinEDDS;

    using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Transfer;
    using Relativity.Logging;
    using Relativity.Transfer;

    [TestFixture]
	public class SmartTapiBridgeTests
	{
		private const long _FILE_BYTES = 512;
		private const int _LINE_NUMBER = 10;
		private Mock<IExportConfig> _config;
		private Mock<ITapiBridgeWrapperFactory> _wrapperFactory;
		private Mock<ITapiBridgeWrapper> _innerTapiBridge;
		private Mock<ILog> _log;
		private SmartTapiBridge _bridge;

		[SetUp]
		public void SetUp()
		{
			_config = new Mock<IExportConfig>();
			_innerTapiBridge = new Mock<ITapiBridgeWrapper>();
			_wrapperFactory = new Mock<ITapiBridgeWrapperFactory>();
			_wrapperFactory.Setup(factory => factory.Create()).Returns(_innerTapiBridge.Object);
			_log = new Mock<ILog>();
			_bridge = new SmartTapiBridge(_config.Object, _wrapperFactory.Object, _log.Object, CancellationToken.None);
		}

		[Test]
		public void ItShouldCreateInnerTapiBridgeOnFirstPath()
		{
			_bridge.AddPath(new TransferPath("Mock Path"));

			_wrapperFactory.Verify(factory => factory.Create(), Times.Once);
		}

		[Test]
		public void ItShouldCreateInnerTapiBridgeOnce()
		{
			_bridge.AddPath(new TransferPath("Mock Path"));
			_bridge.AddPath(new TransferPath("Mock Path 2"));

			_wrapperFactory.Verify(factory => factory.Create(), Times.Once);
		}

		[Test]
		public void ItShouldDisposeInnerTapiBridgeOnDispose()
		{
			_bridge.AddPath(new TransferPath("Mock Path"));

			_bridge.Dispose();

			_innerTapiBridge.Verify(b => b.Dispose());
		}

		[Test]
		public void ItShouldDisposeInnerTapiBridgeOnDisconnect()
		{
			_bridge.AddPath(new TransferPath("Mock Path"));

			_bridge.Disconnect();

			_innerTapiBridge.Verify(b => b.Dispose());
		}

		[Test]
		public void ItShouldCreateNewInnerTapiBridgeAfterOldDisposed()
		{
			_bridge.AddPath(new TransferPath("Mock Path"));

			_bridge.Disconnect();

			_bridge.AddPath(new TransferPath("Mock Path"));

			_wrapperFactory.Verify(factory => factory.Create(), Times.Exactly(2));
		}

		[Test]
		public void ItShouldForwardTapiStatusMessage()
		{
			bool eventRaised = false;
			_bridge.TapiStatusMessage += (sender, args) => eventRaised = true;
			_bridge.AddPath(new TransferPath("Mock Path"));

			_innerTapiBridge.Raise(b => b.TapiStatusMessage += null, new TapiMessageEventArgs("Mock message", _LINE_NUMBER));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardTapiErrorMessage()
		{
			bool eventRaised = false;
			_bridge.TapiErrorMessage += (sender, args) => eventRaised = true;
			_bridge.AddPath(new TransferPath("Mock Path"));

			_innerTapiBridge.Raise(b => b.TapiErrorMessage += null, new TapiMessageEventArgs("Mock message", _LINE_NUMBER));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardTapiWarningMessage()
		{
			bool eventRaised = false;
			_bridge.TapiWarningMessage += (sender, args) => eventRaised = true;
			_bridge.AddPath(new TransferPath("Mock Path"));

			_innerTapiBridge.Raise(b => b.TapiWarningMessage += null, new TapiMessageEventArgs("Mock message", _LINE_NUMBER));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardTapiFatalError()
		{
			bool eventRaised = false;
			_bridge.TapiFatalError += (sender, args) => eventRaised = true;
			_bridge.AddPath(new TransferPath("Mock Path"));

			_innerTapiBridge.Raise(b => b.TapiFatalError += null, new TapiMessageEventArgs("Mock message", _LINE_NUMBER));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardTapiClientChanged()
		{
			bool eventRaised = false;
			_bridge.TapiClientChanged += (sender, args) => eventRaised = true;
			_bridge.AddPath(new TransferPath("Mock Path"));

			_innerTapiBridge.Raise(b => b.TapiClientChanged += null, new TapiClientEventArgs("Client name", TapiClient.Aspera));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardTapiProgress()
		{
			bool eventRaised = false;
			_bridge.TapiProgress += (sender, args) => eventRaised = true;
			_bridge.AddPath(new TransferPath("Mock Path"));

			_innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardTapiStatistics()
		{
			bool eventRaised = false;
			_bridge.TapiStatistics += (sender, args) => eventRaised = true;
			_bridge.AddPath(new TransferPath("Mock Path"));

			_innerTapiBridge.Raise(b => b.TapiStatistics += null, new TapiStatisticsEventArgs(1024, 100, 1024 * 1024, 5.55));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardAddPath()
		{
			_bridge.AddPath(new TransferPath("Mock Path"));
			_bridge.AddPath(new TransferPath("Mock Path"));

			_innerTapiBridge.Verify(inner => inner.AddPath(It.IsAny<TransferPath>()), Times.Exactly(2));
		}

		[Test]
		public async Task ItShouldWaitForTransferJobWithoutSideEffects()
		{
			_config.SetupGet(conf => conf.MaximumFilesForTapiBridge).Returns(3);
			_config.SetupGet(conf => conf.TapiBridgeExportTransferWaitingTimeInSeconds).Returns(1);

			_bridge = new SmartTapiBridge(_config.Object, _wrapperFactory.Object, _log.Object, CancellationToken.None);

			_bridge.AddPath(new TransferPath("File 1"));
			_bridge.AddPath(new TransferPath("File 2"));

			Task raiseEventsTask = Task.Delay(1).ContinueWith(_ =>
			{
				_innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));
				_innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));
			});

			_bridge.WaitForTransferJob();

			await raiseEventsTask.ConfigureAwait(false);

			_innerTapiBridge.Verify(inner => inner.WaitForTransferJob(), Times.Never);
			_innerTapiBridge.Verify(inner => inner.Dispose(), Times.Never);
		}

		[Test]
		public async Task ItShouldForwardWaitForTransferJobAndDisposeWhenWaitingTimePassed()
		{
			_config.SetupGet(conf => conf.MaximumFilesForTapiBridge).Returns(3);
			_config.SetupGet(conf => conf.TapiBridgeExportTransferWaitingTimeInSeconds).Returns(0);

			_bridge = new SmartTapiBridge(_config.Object, _wrapperFactory.Object, _log.Object, CancellationToken.None);

			_bridge.AddPath(new TransferPath("File 1"));
			_bridge.AddPath(new TransferPath("File 2"));

			Task raiseEventsTask = Task.Delay(1).ContinueWith(_ =>
			{
				_innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));
				_innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));
			});

			_bridge.WaitForTransferJob();

			await raiseEventsTask.ConfigureAwait(false);

			_innerTapiBridge.Verify(inner => inner.WaitForTransferJob(), Times.Once);
			_innerTapiBridge.Verify(inner => inner.Dispose(), Times.Once);
		}

		[Test]
		public async Task ItShouldForwardWaitForTransferJobAndDisposeWhenMaximumFilesLimitReached()
		{
			_config.SetupGet(conf => conf.MaximumFilesForTapiBridge).Returns(2);
			_config.SetupGet(conf => conf.TapiBridgeExportTransferWaitingTimeInSeconds).Returns(1);

			_bridge = new SmartTapiBridge(_config.Object, _wrapperFactory.Object, _log.Object, CancellationToken.None);

			_bridge.AddPath(new TransferPath("File 1"));
			_bridge.AddPath(new TransferPath("File 2"));

			Task raiseEventsTask = Task.Delay(1).ContinueWith(_ =>
			{
				_innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));
				_innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));
			});

			_bridge.WaitForTransferJob();

			await raiseEventsTask.ConfigureAwait(false);

			_innerTapiBridge.Verify(inner => inner.WaitForTransferJob(), Times.Once);
			_innerTapiBridge.Verify(inner => inner.Dispose(), Times.Once);
		}
	}
}