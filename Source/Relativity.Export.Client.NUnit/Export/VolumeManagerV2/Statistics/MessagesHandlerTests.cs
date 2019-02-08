using kCura.Windows.Process;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.TApi;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Statistics
{
	[TestFixture]
	public class MessagesHandlerTests
	{
		private MessagesHandler _instance;

		private Mock<IStatus> _status;
		private Mock<ITapiBridge> _tapiBridge;

		[SetUp]
		public void SetUp()
		{
			_status = new Mock<IStatus>();
			_tapiBridge = new Mock<ITapiBridge>();

			_instance = new MessagesHandler(_status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldWriteErrorMessage()
		{
			const string message = "error_message";

			_instance.Attach(_tapiBridge.Object);

			//ACT
			_tapiBridge.Raise(x => x.TapiErrorMessage += null, new TapiMessageEventArgs(message, 0));

			//ASSERT
			_status.Verify(x => x.WriteError(message), Times.Once);
		}

		[Test]
		public void ItShouldWriteWarningMessage()
		{
			const string message = "warning_message";

			_instance.Attach(_tapiBridge.Object);

			//ACT
			_tapiBridge.Raise(x => x.TapiWarningMessage += null, new TapiMessageEventArgs(message, 0));

			//ASSERT
			_status.Verify(x => x.WriteWarning(message), Times.Once);
		}

		[Test]
		public void ItShouldWriteFatalMessage()
		{
			const string message = "fatal_message";

			_instance.Attach(_tapiBridge.Object);

			//ACT
			_tapiBridge.Raise(x => x.TapiFatalError += null, new TapiMessageEventArgs(message, 0));

			//ASSERT
			_status.Verify(x => x.WriteError(message), Times.Once);
		}

		[Test]
		public void ItShouldWriteStatusMessage()
		{
			const string message = "status_message";

			_instance.Attach(_tapiBridge.Object);

			//ACT
			_tapiBridge.Raise(x => x.TapiStatusMessage += null, new TapiMessageEventArgs(message, 0));

			//ASSERT
			_status.Verify(x => x.WriteStatusLine(EventType.Status, message, false), Times.Once);
		}

		[Test]
		public void ItShouldDetach()
		{
			const string message = "message";

			_instance.Attach(_tapiBridge.Object);
			_instance.Detach();

			//ACT
			_tapiBridge.Raise(x => x.TapiErrorMessage += null, new TapiMessageEventArgs(message, 0));
			_tapiBridge.Raise(x => x.TapiWarningMessage += null, new TapiMessageEventArgs(message, 0));
			_tapiBridge.Raise(x => x.TapiFatalError += null, new TapiMessageEventArgs(message, 0));
			_tapiBridge.Raise(x => x.TapiStatusMessage += null, new TapiMessageEventArgs(message, 0));

			//ASSERT
			_status.Verify(x => x.WriteWarning(message), Times.Never);
			_status.Verify(x => x.WriteError(message), Times.Never);
			_status.Verify(x => x.WriteStatusLine(EventType.Status, message, false), Times.Never);
		}
	}
}