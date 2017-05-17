using kCura.WinEDDS.Core.Import.Errors;
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

		[SetUp]
		public void SetUp()
		{
			_transferConfig = new Mock<ITransferConfig>();

			_instance = new ErrorReporter(_transferConfig.Object);
		}

		[Test]
		public void ItShouldRaiseErrorEvent()
		{
			_transferConfig.Setup(x => x.DefaultMaximumErrorCount).Returns(10);

			var eventRaised = false;
			LineError expectedLineError = new LineError();
			LineError actualLineError = null;

			_instance.ErrorOccurred += (s, e) =>
			{
				eventRaised = true;
				actualLineError = e;
			};

			// ACT
			_instance.WriteError(expectedLineError);

			// ASSERT
			Assert.That(eventRaised, Is.True);
			Assert.That(actualLineError, Is.EqualTo(expectedLineError));
		}

		[Test]
		public void ItShouldSkipEventWhenLimitExceeded()
		{
			_transferConfig.Setup(x => x.DefaultMaximumErrorCount).Returns(0);

			var eventRaised = false;
			LineError lineError = new LineError();

			_instance.ErrorOccurred += (s, e) => eventRaised = true;

			// ACT
			_instance.WriteError(lineError);

			// ASSERT
			Assert.That(eventRaised, Is.False);
		}

		[Test]
		public void ItShouldRaiseLimitReachedMessage()
		{
			_transferConfig.Setup(x => x.DefaultMaximumErrorCount).Returns(1);

			var eventRaised = false;
			LineError lineError = new LineError();
			LineError actualLineError = null;

			_instance.ErrorOccurred += (s, e) =>
			{
				eventRaised = true;
				actualLineError = e;
			};

			// ACT
			_instance.WriteError(lineError);

			// ASSERT
			Assert.That(eventRaised, Is.True);
			Assert.That(actualLineError.Message, Is.EqualTo(_MAX_ERROR_COUNT_REACHED_MESSAGE));
		}
	}
}