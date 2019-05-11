// -----------------------------------------------------------------------------------------------------
// <copyright file="SmartTapiBridgeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
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
			this._config = new Mock<IExportConfig>();
			this._innerTapiBridge = new Mock<ITapiBridgeWrapper>();
			this._wrapperFactory = new Mock<ITapiBridgeWrapperFactory>();
			this._wrapperFactory.Setup(factory => factory.Create()).Returns(this._innerTapiBridge.Object);
			this._log = new Mock<ILog>();
			this._bridge = new SmartTapiBridge(this._config.Object, this._wrapperFactory.Object, this._log.Object, CancellationToken.None);
		}

		[Test]
		public void ItShouldCreateInnerTapiBridgeOnFirstPath()
		{
			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._wrapperFactory.Verify(factory => factory.Create(), Times.Once);
		}

		[Test]
		public void ItShouldCreateInnerTapiBridgeOnce()
		{
			this._bridge.AddPath(new TransferPath("Mock Path"));
			this._bridge.AddPath(new TransferPath("Mock Path 2"));

			this._wrapperFactory.Verify(factory => factory.Create(), Times.Once);
		}

		[Test]
		public void ItShouldDisposeInnerTapiBridgeOnDispose()
		{
			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._bridge.Dispose();

			this._innerTapiBridge.Verify(b => b.Dispose());
		}

		[Test]
		public void ItShouldDisposeInnerTapiBridgeOnDisconnect()
		{
			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._bridge.Disconnect();

			this._innerTapiBridge.Verify(b => b.Dispose());
		}

		[Test]
		public void ItShouldCreateNewInnerTapiBridgeAfterOldDisposed()
		{
			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._bridge.Disconnect();

			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._wrapperFactory.Verify(factory => factory.Create(), Times.Exactly(2));
		}

		[Test]
		public void ItShouldForwardTapiStatusMessage()
		{
			bool eventRaised = false;
			this._bridge.TapiStatusMessage += (sender, args) => eventRaised = true;
			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._innerTapiBridge.Raise(b => b.TapiStatusMessage += null, new TapiMessageEventArgs("Mock message", _LINE_NUMBER));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardTapiErrorMessage()
		{
			bool eventRaised = false;
			this._bridge.TapiErrorMessage += (sender, args) => eventRaised = true;
			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._innerTapiBridge.Raise(b => b.TapiErrorMessage += null, new TapiMessageEventArgs("Mock message", _LINE_NUMBER));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardTapiWarningMessage()
		{
			bool eventRaised = false;
			this._bridge.TapiWarningMessage += (sender, args) => eventRaised = true;
			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._innerTapiBridge.Raise(b => b.TapiWarningMessage += null, new TapiMessageEventArgs("Mock message", _LINE_NUMBER));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardTapiFatalError()
		{
			bool eventRaised = false;
			this._bridge.TapiFatalError += (sender, args) => eventRaised = true;
			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._innerTapiBridge.Raise(b => b.TapiFatalError += null, new TapiMessageEventArgs("Mock message", _LINE_NUMBER));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardTapiClientChanged()
		{
			bool eventRaised = false;
			this._bridge.TapiClientChanged += (sender, args) => eventRaised = true;
			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._innerTapiBridge.Raise(b => b.TapiClientChanged += null, new TapiClientEventArgs("Client name", TapiClient.Aspera));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardTapiProgress()
		{
			bool eventRaised = false;
			this._bridge.TapiProgress += (sender, args) => eventRaised = true;
			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardTapiStatistics()
		{
			bool eventRaised = false;
			this._bridge.TapiStatistics += (sender, args) => eventRaised = true;
			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._innerTapiBridge.Raise(b => b.TapiStatistics += null, new TapiStatisticsEventArgs(1024, 100, 1024 * 1024, 5.55));

			Assert.IsTrue(eventRaised);
		}

		[Test]
		public void ItShouldForwardAddPath()
		{
			this._bridge.AddPath(new TransferPath("Mock Path"));
			this._bridge.AddPath(new TransferPath("Mock Path"));

			this._innerTapiBridge.Verify(inner => inner.AddPath(It.IsAny<TransferPath>()), Times.Exactly(2));
		}

		[Test]
		public async Task ItShouldWaitForTransferJobWithoutSideEffects()
		{
			this._config.SetupGet(conf => conf.MaximumFilesForTapiBridge).Returns(3);
			this._config.SetupGet(conf => conf.TapiBridgeExportTransferWaitingTimeInSeconds).Returns(1);

			this._bridge = new SmartTapiBridge(this._config.Object, this._wrapperFactory.Object, this._log.Object, CancellationToken.None);

			this._bridge.AddPath(new TransferPath("File 1"));
			this._bridge.AddPath(new TransferPath("File 2"));

			Task raiseEventsTask = Task.Delay(1).ContinueWith(_ =>
			{
				this._innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));
				this._innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));
			});

			this._bridge.WaitForTransferJob();

			await raiseEventsTask.ConfigureAwait(false);

			this._innerTapiBridge.Verify(inner => inner.WaitForTransferJob(), Times.Never);
			this._innerTapiBridge.Verify(inner => inner.Dispose(), Times.Never);
		}

		[Test]
		public async Task ItShouldForwardWaitForTransferJobAndDisposeWhenWaitingTimePassed()
		{
			this._config.SetupGet(conf => conf.MaximumFilesForTapiBridge).Returns(3);
			this._config.SetupGet(conf => conf.TapiBridgeExportTransferWaitingTimeInSeconds).Returns(0);

			this._bridge = new SmartTapiBridge(this._config.Object, this._wrapperFactory.Object, this._log.Object, CancellationToken.None);

			this._bridge.AddPath(new TransferPath("File 1"));
			this._bridge.AddPath(new TransferPath("File 2"));

			Task raiseEventsTask = Task.Delay(1).ContinueWith(_ =>
			{
				this._innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));
				this._innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));
			});

			this._bridge.WaitForTransferJob();

			await raiseEventsTask.ConfigureAwait(false);

			this._innerTapiBridge.Verify(inner => inner.WaitForTransferJob(), Times.Once);
			this._innerTapiBridge.Verify(inner => inner.Dispose(), Times.Once);
		}

		[Test]
		public async Task ItShouldForwardWaitForTransferJobAndDisposeWhenMaximumFilesLimitReached()
		{
			this._config.SetupGet(conf => conf.MaximumFilesForTapiBridge).Returns(2);
			this._config.SetupGet(conf => conf.TapiBridgeExportTransferWaitingTimeInSeconds).Returns(1);

			this._bridge = new SmartTapiBridge(this._config.Object, this._wrapperFactory.Object, this._log.Object, CancellationToken.None);

			this._bridge.AddPath(new TransferPath("File 1"));
			this._bridge.AddPath(new TransferPath("File 2"));

			Task raiseEventsTask = Task.Delay(1).ContinueWith(_ =>
			{
				this._innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));
				this._innerTapiBridge.Raise(b => b.TapiProgress += null, new TapiProgressEventArgs("file name", true, TransferPathStatus.Successful, _LINE_NUMBER, _FILE_BYTES, DateTime.Now, DateTime.Now));
			});

			this._bridge.WaitForTransferJob();

			await raiseEventsTask.ConfigureAwait(false);

			this._innerTapiBridge.Verify(inner => inner.WaitForTransferJob(), Times.Once);
			this._innerTapiBridge.Verify(inner => inner.Dispose(), Times.Once);
		}
	}
}