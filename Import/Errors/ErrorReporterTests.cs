using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Status;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Import.Errors
{
	[TestFixture]
	public class ErrorReporterTests
	{
		private const string _MAX_ERROR_COUNT_REACHED_MESSAGE = "Maximum number of errors for display reached.  Export errors to view full list.";

		private ErrorReporter _instance;
		private Mock<ITransferConfig> _transferConfig;
		private Mock<IImportStatusManager> _importStatusMgrMock;

		[SetUp]
		public void SetUp()
		{
			_transferConfig = new Mock<ITransferConfig>();
			_importStatusMgrMock = new Mock<IImportStatusManager>();

			_instance = new ErrorReporter(_transferConfig.Object, _importStatusMgrMock.Object);
		}

		[Test]
		public void ItShouldRaiseErrorEvent()
		{
			_transferConfig.Setup(x => x.DefaultMaximumErrorCount).Returns(10);

			var lineError = new LineError
			{
				LineNumber = 1234,
				Message = "Error"
			};

			_importStatusMgrMock.Setup(obj => obj.RaiseErrorImportEvent(It.IsAny<ErrorReporter>(), It.IsAny<LineError>()));
			
			// ACT
			_instance.WriteError(lineError);

			// ASSERT
			_importStatusMgrMock.Verify(obj => obj.RaiseErrorImportEvent(_instance, lineError), Times.Once);
		}

		[Test]
		public void ItShouldSkipEventWhenLimitExceeded()
		{
			_transferConfig.Setup(x => x.DefaultMaximumErrorCount).Returns(0);

			var lineError = new LineError
			{
				LineNumber = 1234,
				Message = "Error"
			};

			_importStatusMgrMock.Setup(obj => obj.RaiseErrorImportEvent(It.IsAny<ErrorReporter>(), It.IsAny<LineError>()));

			// ACT
			_instance.WriteError(lineError);

			// ASSERT
			_importStatusMgrMock.Verify(obj => obj.RaiseErrorImportEvent(_instance, lineError), Times.Never);
		}

		[Test]
		public void ItShouldRaiseLimitReachedMessage()
		{
			_transferConfig.Setup(x => x.DefaultMaximumErrorCount).Returns(1);

			var lineError = new LineError
			{
				LineNumber = 1234,
				Message = "Error"
			};

			_importStatusMgrMock.Setup(obj => obj.RaiseErrorImportEvent(It.IsAny<ErrorReporter>(), It.IsAny<LineError>()));

			// ACT
			_instance.WriteError(lineError);

			// ASSERT
			_importStatusMgrMock.Verify(obj => obj.RaiseErrorImportEvent(_instance, It.Is<LineError>(_ => _.LineNumber == 0 && 
			_.Message.EndsWith("Maximum number of errors for display reached.  Export errors to view full list."))), Times.Once);
		}
	}
}