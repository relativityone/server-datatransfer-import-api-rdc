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

    using Moq;

    using global::NUnit.Framework;

    using global::Relativity.Logging;
    using global::Relativity.Transfer;

    /// <summary>
    /// Represents <see cref="IoReporter"/> tests.
    /// </summary>
    [TestFixture]
    public class IoReporterTests
    {
        private IIoReporter ioReporterInstance;
        private Mock<IFileSystemService> fileSystemService;
        private Mock<IWaitAndRetryPolicy> mockedWaitAndRetry;
        private IWaitAndRetryPolicy waitAndRetry;
        private Mock<ILog> logger;
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

        [SetUp]
        public void Setup()
        {
            this.fileSystemService = new Mock<IFileSystemService>();
            this.mockedWaitAndRetry = new Mock<IWaitAndRetryPolicy>();
            this.waitAndRetry = null;
            this.logger = new Mock<ILog>();
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
            this.GivenTheMockedWaitAndRetryPolicyCallback(1);
            this.WhenExecutingTheGetFileLength();
            this.ThenTheActualRetryDuractionShouldCalculated(retryAttempt, waitTimeBetweenRetryAttempts);
        }

        [Test]
        public void ItShouldRetryOnExceptionWhenNotDisabledNativeLocationValidation()
        {
            this.GivenTheExpectedException();
            this.GivenTheExpectedLogWarningMessage();
            this.GivenTheMockedWaitAndRetryPolicyCallback(1);
            this.GivenTheLoggerWarningCallback();
            this.WhenExecutingTheGetFileLength();
            this.ThenTheActualExceptionMessageShouldEqual();
            this.ThenTheActualLogWarningMessageShouldEqual();
            this.ThenTheLoggerWarningShouldBeInvokedOneTime();
        }
        
        [Test]
        public void ItShouldRetryOnExceptionWhenDisabledNativeLocationValidation()
        {
            this.GivenTheExpectedInvalidPathException();
            this.GivenTheExpectedLogErrorMessage();
            this.GivenTheWaitAndRetryCallbackThatThrowsArgumentException();
            this.GivenTheLoggerErrorCallback();
            this.WhenExecutingIoReporterGetFileLengthThenThwowsException(true);
            this.ThenTheActualInvalidPathExceptionMessageShouldEqual();
            this.ThenTheActualLogErrorMessageShouldEqual();
            this.ThenTheLoggerErrorShouldBeInvokedOneTime();
        }

        private void ThenTheLoggerErrorShouldBeInvokedOneTime()
        {
            this.logger.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
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
            this.mockedWaitAndRetry.Setup(obj => obj.WaitTimeSecondsBetweenRetryAttempts).Returns(waitTimeBetweenRetryAttempts);
        }

        /// <summary>
        /// Givens the wait and retry.
        /// </summary>
        private void GivenTheRealWaitAndRetryPolicy()
        {
            this.waitAndRetry = new WaitAndRetryPolicy(1, 0);
        }

        private void GivenTheMockedWaitAndRetryPolicyCallback(long expected)
        {
            this.mockedWaitAndRetry
                .Setup(
                    obj => obj.WaitAndRetry<long, Exception>(
                        It.IsAny<Func<int, TimeSpan>>(),
                        It.IsAny<Action<Exception, TimeSpan>>(),
                        It.IsAny<Func<long>>())).Callback<Func<int, TimeSpan>, Action<Exception, TimeSpan>, Func<long>>(
                    (retryDuration, retryAction, execFunc) =>
                        {
                            this.actualRetryDuractionFunc = retryDuration;
                            retryAction(this.expectedException, TimeSpan.Zero);
                            execFunc();
                        });
        }

        private void GivenTheFileLength(int expectedLength)
        {
            this.fileSystemService.Setup(obj => obj.GetFileLength(_FILE_NAME)).Returns(expectedLength);
        }

        private void GivenTheLoggerWarningCallback()
        {
            this.logger.Setup(
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
            this.logger.Setup(log => log.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, logWarningMessage, param) =>
                {
                    this.actualInvalidPathExceptionMessage = ex.Message;
                    this.actualLogErrorMessage = logWarningMessage;
                });
        }

        private void GivenTheWaitAndRetryCallbackThatThrowsArgumentException()
        {
            this.mockedWaitAndRetry
                .Setup(
                    obj => obj.WaitAndRetry<long, Exception>(
                        It.IsAny<Func<int, TimeSpan>>(),
                        It.IsAny<Action<Exception, TimeSpan>>(),
                        It.IsAny<Func<long>>())).Callback<Func<int, TimeSpan>, Action<Exception, TimeSpan>, Func<long>>(
                    (retryDuration, retryAction, execFunc) =>
                        {
                            retryAction(this.expectedArgumentException, TimeSpan.Zero);
                        });
        }

        private void GivenTheExpectedLogWarningMessage()
        {
            this.expectedLogWarningMessage = IoReporter.BuildIoReporterWarningMessage(this.expectedException, 0);
        }

        private void GivenTheExpectedLogErrorMessage()
        {
            this.expectedLogErrorMessage = string.Format(
                kCura.WinEDDS.TApi.Resources.Strings.ImportInvalidPathCharactersExceptionMessage,
                _FILE_NAME);
        }

        private void GivenTheExpectedException()
        {
            this.expectedException = new Exception(_EXPECTED_DEFAULT_EXCEPTION_MESSAGE);
        }

        private void GivenTheExpectedInvalidPathException()
        {
            this.expectedArgumentException = new ArgumentException(_EXPECTED_INVALID_PATH_EXCEPTION_MESSAGE);
        }

        private void WhenExecutingTheGetFileLength(bool disableNativeLocationValidation = false)
        {
            var policy = this.waitAndRetry != null ? this.waitAndRetry : this.mockedWaitAndRetry.Object;
            this.ioReporterInstance = new IoReporter(
                this.fileSystemService.Object,
                policy,
                this.logger.Object,
                this.publisher,
                disableNativeLocationValidation);
            this.actualFileLength = this.ioReporterInstance.GetFileLength(_FILE_NAME, 0);
        }

        private void WhenExecutingIoReporterGetFileLengthThenThwowsException(bool disableNativeLocationValidation)
        {
            this.ioReporterInstance = new IoReporter(
                this.fileSystemService.Object,
                this.mockedWaitAndRetry.Object,
                this.logger.Object,
                this.publisher,
                disableNativeLocationValidation);
            Assert.That(
                () => this.ioReporterInstance.GetFileLength(_FILE_NAME, 0),
                Throws.Exception.TypeOf<FileInfoInvalidPathException>());
        }

        private void ThenTheActualRetryDuractionShouldCalculated(int retryAttempt, int waitTimeBetweenRetryAttempts)
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
            this.logger.Verify(log => log.LogWarning(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
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
