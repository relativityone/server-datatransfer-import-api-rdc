// -----------------------------------------------------------------------------------------------------
// <copyright file="MessagesHandlerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	[TestFixture]
	public class MessagesHandlerTests
	{
		private MessagesHandler _instance;

		private Mock<IStatus> _status;
		private Mock<ITapiBridge> _tapiBridge;

		[SetUp]
		public void SetUp()
		{
			this._status = new Mock<IStatus>();
			this._tapiBridge = new Mock<ITapiBridge>();

			this._instance = new MessagesHandler(this._status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldWriteErrorMessage()
		{
			const string message = "error_message";

			this._instance.Subscribe(this._tapiBridge.Object);

			// ACT
			this._tapiBridge.Raise(x => x.TapiErrorMessage += null, new TapiMessageEventArgs(message, 0));

			// ASSERT
			this._status.Verify(x => x.WriteError(message), Times.Once);
		}

		[Test]
		public void ItShouldWriteWarningMessage()
		{
			const string message = "warning_message";

			this._instance.Subscribe(this._tapiBridge.Object);

			// ACT
			this._tapiBridge.Raise(x => x.TapiWarningMessage += null, new TapiMessageEventArgs(message, 0));

			// ASSERT
			this._status.Verify(x => x.WriteWarning(message), Times.Once);
		}

		[Test]
		public void ItShouldWriteFatalMessage()
		{
			const string message = "fatal_message";

			this._instance.Subscribe(this._tapiBridge.Object);

			// ACT
			this._tapiBridge.Raise(x => x.TapiFatalError += null, new TapiMessageEventArgs(message, 0));

			// ASSERT
			this._status.Verify(x => x.WriteError(message), Times.Once);
		}

		[Test]
		public void ItShouldWriteStatusMessage()
		{
			const string message = "status_message";

			this._instance.Subscribe(this._tapiBridge.Object);

			// ACT
			this._tapiBridge.Raise(x => x.TapiStatusMessage += null, new TapiMessageEventArgs(message, 0));

			// ASSERT
			this._status.Verify(x => x.WriteStatusLine(EventType2.Status, message, false), Times.Once);
		}

		[Test]
		public void ItShouldDetach()
		{
			const string message = "message";

			this._instance.Subscribe(this._tapiBridge.Object);
			this._instance.Unsubscribe(this._tapiBridge.Object);

			// ACT
			this._tapiBridge.Raise(x => x.TapiErrorMessage += null, new TapiMessageEventArgs(message, 0));
			this._tapiBridge.Raise(x => x.TapiWarningMessage += null, new TapiMessageEventArgs(message, 0));
			this._tapiBridge.Raise(x => x.TapiFatalError += null, new TapiMessageEventArgs(message, 0));
			this._tapiBridge.Raise(x => x.TapiStatusMessage += null, new TapiMessageEventArgs(message, 0));

			// ASSERT
			this._status.Verify(x => x.WriteWarning(message), Times.Never);
			this._status.Verify(x => x.WriteError(message), Times.Never);
			this._status.Verify(x => x.WriteStatusLine(EventType2.Status, message, false), Times.Never);
		}
	}
}