// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoReporterTests.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="IoReporter"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi.NUnit.Integration
{
    using System;
	using System.Threading;
    using Moq;

    using global::NUnit.Framework;

    using global::Relativity.Logging;

    /// <summary>
    /// Represents <see cref="IoReporter"/> tests.
    /// </summary>
	[TestFixture]
	public class IoReporterTests
	{
        private IIoReporter ioReporterInstance;
        private Mock<IFileSystem> mockfileSystem;
        private Mock<IWaitAndRetryPolicy> mockWaitAndRetry;
        private IWaitAndRetryPolicy waitAndRetry;
        private Mock<ILog> mockLogger;
        private IoWarningPublisher publisher;
        private long actualFileLength;
        private Func<int, TimeSpan> actualRetryDuractionFunc = null;
        private Exception expectedException; 
        private ArgumentException expectedArgumentException;
		private const string _FILE_NAME = "TestFileName";
		private const string _EXPECTED_DEFAULT_EXCEPTION_MESSAGE = "Expected exception message";
		private const string _EXPECTED_INVALID_PATH_EXCEPTION_MESSAGE = "Illegal characters in path.";
        private string expectedLogWarningMessage;
        private string expectedLogErrorMessage;
        private string actualExceptionMessage = string.Empty;
        private string actualLogWarningMessage = string.Empty;
        private string actualInvalidPathExceptionMessage = string.Empty;
        private string actualLogErrorMessage = string.Empty;
		private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

		[SetUp]
		public void Setup()
		{
			this.mockfileSystem = new Mock<IFileSystem>();
            this.mockWaitAndRetry = new Mock<IWaitAndRetryPolicy>();
            this.waitAndRetry = null;
            this.mockLogger = new Mock<ILog>();
            this.publisher = new IoWarningPublisher();
		}

		[TestCase(1)]
		[TestCase(10)]
		[TestCase(1299)]
		public void ItShouldGetFileLength(int expectedLength)
		{
            this.GivenTheRealWaitAndRetryPolicy();
            this.GivenTheFileLength(expectedLength);            
            this.WhenExecutingTheGetFileLength();            
            this.ThenTheActualFileLengthShouldEqual(expectedLength);
		}

		[TestCase(1, 1)]
		[TestCase(2, 1)]
		[TestCase(2, 2)]
		[TestCase(3, 10)]
		[TestCase(2, -1)]
		[TestCase(2, 0)]
		public void ItShouldCalculateProperRetryDuration(int retryAttempt, int waitTimeBetweenRetryAttempts)
		{
            this.GivenTheWaitAndRetryReturns(waitTimeBetweenRetryAttempts);
            this.GivenTheMockedWaitAndRetryPolicyCallback();
			this.GivenTheFileLength(1000);
			this.WhenExecutingTheGetFileLength();            
            this.ThenTheActualRetryDurationShouldCalculated(retryAttempt, waitTimeBetweenRetryAttempts);
		}

		[Test]
		public void ItShouldRetryOnIoExceptionWhenNotDisabledNativeLocationValidation()
		{
            this.GivenTheExpectedIoException();
            this.GivenTheExpectedLogWarningMessage(0, 0, 0);
            this.GivenTheMockedWaitAndRetryPolicyCallback();
            this.GivenTheLoggerWarningCallback();
            this.GivenTheFileLength(1000);
			this.WhenExecutingTheGetFileLength();            
            this.ThenTheActualExceptionMessageShouldEqual();
            this.ThenTheActualLogWarningMessageShouldEqual();
            this.ThenTheLoggerWarningShouldBeInvokedOneTime();
		}

		[Test]
		public void ItShouldNotRetryNonIoException()
		{
			this.GivenTheExpectedException(new InvalidOperationException(_EXPECTED_DEFAULT_EXCEPTION_MESSAGE));
			this.GivenTheExpectedLogWarningMessage(0, 0, 0);
			this.GivenTheMockedWaitAndRetryPolicyCallback();
			this.GivenTheLoggerWarningCallback();
			this.GivenTheFileLength(1000);
			this.WhenExecutingTheGetFileLength();
			this.ThenTheActualExceptionMessageShouldEqual();
			this.ThenTheActualLogWarningMessageShouldEqual();
			this.ThenTheLoggerWarningShouldBeInvokedOneTime();
		}

		[Test]
		public void ItShouldThrowFileInfoInvalidPathException()
		{
            this.GivenTheExpectedInvalidPathException();
            this.GivenTheExpectedLogErrorMessage();
		    this.GivenTheFileServiceWhichThrowsArgumentException(expectedArgumentException);
		    this.GivenTheWaitAndRetryCallback();
            this.GivenTheLoggerErrorCallback();
		    this.WhenExecutingIoReporterGetFileLengthThenThrowsException(true);
            this.ThenTheActualInvalidPathExceptionMessageShouldEqual();
            this.ThenTheActualLogErrorMessageShouldEqual();
            this.ThenTheLoggerErrorShouldBeInvokedOneTime();
		}

		private void ThenTheLoggerErrorShouldBeInvokedOneTime()
		{
            this.mockLogger.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
		}

		private void ThenTheActualLogErrorMessageShouldEqual()
		{
            Assert.That(this.actualLogErrorMessage, Is.EqualTo(this.expectedLogErrorMessage));
		}

		private void ThenTheActualInvalidPathExceptionMessageShouldEqual()
		{
            Assert.That(this.actualInvalidPathExceptionMessage, Is.EqualTo(_EXPECTED_INVALID_PATH_EXCEPTION_MESSAGE));
		}

		#region "Helper methods"

		private void GivenTheWaitAndRetryReturns(int waitTimeBetweenRetryAttempts)
		{
            this.mockWaitAndRetry.Setup(obj => obj.WaitTimeSecondsBetweenRetryAttempts).Returns(waitTimeBetweenRetryAttempts);
		}

        /// <summary>
        /// Givens the wait and retry.
        /// </summary>
        private void GivenTheRealWaitAndRetryPolicy()
		{
            this.waitAndRetry = new WaitAndRetryPolicy(1, 0);
        }

        private void GivenTheMockedWaitAndRetryPolicyCallback()
        {
			this.mockWaitAndRetry
		        .Setup(
			        obj => obj.WaitAndRetry(
				        It.IsAny<Func<Exception, bool>>(),
				        It.IsAny<Func<int, TimeSpan>>(),
				        It.IsAny<Action<Exception, TimeSpan>>(),
				        It.IsAny<Func<CancellationToken, long>>(),
				        It.IsAny<CancellationToken>()))
		        .Callback<
			        Func<Exception, bool>,
			        Func<int, TimeSpan>,
			        Action<Exception, TimeSpan>,
			        Func<CancellationToken, long>,
			        CancellationToken>(
			        (exceptionPredicate, retryDuration, retryAction, execFunc, token) =>
			        {
				        this.actualRetryDuractionFunc = retryDuration;
				        retryAction(this.expectedException, TimeSpan.Zero);
				        execFunc(token);
			        });
        }

		private void GivenTheFileLength(int expectedLength)
		{
			var mockFileInfo = new Mock<IFileInfo>();
			mockFileInfo.Setup(x => x.Length).Returns(expectedLength);
			this.mockfileSystem.Setup(x => x.CreateFileInfo(_FILE_NAME)).Returns(mockFileInfo.Object);
		}

		private void GivenTheFileServiceWhichThrowsArgumentException(ArgumentException exception)
		{
			this.mockfileSystem.Setup(x => x.CreateFileInfo(It.IsAny<string>())).Throws(exception);
		}

	    private void GivenTheLoggerWarningCallback()
	    {
	        this.mockLogger.Setup(
	                log => log.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
	            .Callback<Exception, string, object[]>(
	                (ex, logWarningMessage, param) =>
	                {
	                    this.actualExceptionMessage = ex.Message;
	                    this.actualLogWarningMessage = logWarningMessage;
	                });
	    }

	    private void GivenTheLoggerErrorCallback()
	    {
	        this.mockLogger.Setup(
	                log => log.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
	            .Callback<Exception, string, object[]>((ex, logWarningMessage, param) =>
	            {
	                this.actualInvalidPathExceptionMessage = ex.Message;
	                this.actualLogErrorMessage = logWarningMessage;
	            });
	    }

        private void GivenTheWaitAndRetryCallback()
        {
	        this.mockWaitAndRetry
		        .Setup(
			        obj => obj.WaitAndRetry(
				        It.IsAny<Func<Exception, bool>>(),
				        It.IsAny<Func<int, TimeSpan>>(),
				        It.IsAny<Action<Exception, TimeSpan>>(),
				        It.IsAny<Func<CancellationToken, long>>(),
				        It.IsAny<CancellationToken>()))
		        .Callback<Func<Exception, bool>, Func<int, TimeSpan>, Action<Exception, TimeSpan>,
			        Func<CancellationToken, long>, CancellationToken>(
			        (exceptionPredicate, retryDuration, retryAction, execFunc, token) =>
			        {
				        actualRetryDuractionFunc = retryDuration;
				        retryAction(this.expectedArgumentException, TimeSpan.Zero);
				        execFunc(token);
			        });
        }

		private void GivenTheExpectedLogWarningMessage(double timeoutSeconds, int? retryCount, int? totalRetryCount)
		{
			if (retryCount.HasValue && totalRetryCount.HasValue)
			{
				this.expectedLogWarningMessage =
					IoReporter.BuildIoReporterWarningMessage(this.expectedException, timeoutSeconds, retryCount.Value,
						totalRetryCount.Value);
			}
			else
			{
				this.expectedLogWarningMessage =
					IoReporter.BuildIoReporterWarningMessage(this.expectedException, timeoutSeconds);
			}
		}

		private void GivenTheExpectedLogErrorMessage()
		{
            this.expectedLogErrorMessage = string.Format(
                kCura.WinEDDS.TApi.Resources.Strings.ImportInvalidPathCharactersExceptionMessage,
                _FILE_NAME);
		}

		private void GivenTheExpectedIoException()
		{
			this.GivenTheExpectedException(new System.IO.IOException(_EXPECTED_DEFAULT_EXCEPTION_MESSAGE));
		}

		private void GivenTheExpectedException(Exception exception)
		{
			this.expectedException = exception;
		}

		private void GivenTheExpectedInvalidPathException()
		{
            this.expectedArgumentException = new ArgumentException(_EXPECTED_INVALID_PATH_EXCEPTION_MESSAGE);
		}

		private void WhenExecutingTheGetFileLength(bool disableNativeLocationValidation = false)
		{
		    IWaitAndRetryPolicy policy = waitAndRetry ?? this.mockWaitAndRetry.Object;
            this.ioReporterInstance = new IoReporter(
                this.mockfileSystem.Object,
                policy,
                this.mockLogger.Object,
                this.publisher,
                disableNativeLocationValidation, 
                tokenSource.Token);
            this.actualFileLength = this.ioReporterInstance.GetFileLength(_FILE_NAME, 0);
		}

		private void WhenExecutingIoReporterGetFileLengthThenThrowsException(bool disableNativeLocationValidation)
		{
			ioReporterInstance = new IoReporter(
                this.mockfileSystem.Object,
                this.mockWaitAndRetry.Object,
                this.mockLogger.Object,
                this.publisher,
                disableNativeLocationValidation, 
                tokenSource.Token);
            Assert.That(
                () => this.ioReporterInstance.GetFileLength(_FILE_NAME, 0),
				Throws.Exception.TypeOf<FileInfoInvalidPathException>());
		}

		private void ThenTheActualRetryDurationShouldCalculated(int retryAttempt, int waitTimeBetweenRetryAttempts)
		{
            TimeSpan actualRetryDuraction = this.actualRetryDuractionFunc(retryAttempt);
            Assert.That(
                actualRetryDuraction,
				retryAttempt == 1
					? Is.EqualTo(TimeSpan.FromSeconds(0))
					: Is.EqualTo(TimeSpan.FromSeconds(waitTimeBetweenRetryAttempts)));
		}

		private void ThenTheActualFileLengthShouldEqual(int expectedLength)
		{
            Assert.That(this.actualFileLength, Is.EqualTo(expectedLength));
		}

		private void ThenTheLoggerWarningShouldBeInvokedOneTime()
		{
            this.mockLogger.Verify(log => log.LogWarning(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
		}

		private void ThenTheActualLogWarningMessageShouldEqual()
		{
            Assert.That(this.actualLogWarningMessage, Is.EqualTo(this.expectedLogWarningMessage));
		}

		private void ThenTheActualExceptionMessageShouldEqual()
		{
            Assert.That(this.actualExceptionMessage, Is.EqualTo(_EXPECTED_DEFAULT_EXCEPTION_MESSAGE));
		}

		#endregion
	}
}