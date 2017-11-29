using System;
using Moq;
using NUnit.Framework;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.TApi.NUnit.Integration
{
    [TestFixture]
    public class IoReporterTests
    {
        private IIoReporter _ioReporterInstance;
        private Mock<IFileSystemService> _fileService;
        private Mock<IWaitAndRetryPolicy> _waitAndRetry;
        private Mock<ILog> _logger;
        private IoWarningPublisher _ioWarningPublisher;
        private long _actualFileLength;
        private Func<int, TimeSpan> _actualRetryDuractionFunc = null;
        private Exception _expectedException; 
        private ArgumentException _expectedArgumentException;
        private const string _FILE_NAME = "TestFileName";
        private const string _EXPECTED_DEFAULT_EXCEPTION_MESSAGE = "Expected exception message";
        private const string _EXPECTED_INVALID_PATH_EXCEPTION_MESSAGE = "Illegal characters in path.";
        private const string _EXPECTED_RETHROWN_EXCEPTION_MESSAGE = "rethrowed exception";
        private string _expectedLogWarningMessage;
        private string _expectedLogErrorMessage;
        private string _actualExceptionMessage = string.Empty;
        private string _actualLogWarningMessage = string.Empty;
        private string _actualInvalidPathExceptionMessage = string.Empty;
        private string _actualLogErrorMessage = string.Empty;

        [SetUp]
        public void Setup()
        {
            _fileService = new Mock<IFileSystemService>();
            _waitAndRetry = new Mock<IWaitAndRetryPolicy>();
            _logger = new Mock<ILog>();

            _ioWarningPublisher = new IoWarningPublisher();
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(1299)]
        public void ItShouldGetFileLength(int expectedLength)
        {
            GivenTheWaitAndRetryCallback();
            GivenTheFileLength(expectedLength);
            
            WhenExecutingTheGetFileLength();
            
            ThenTheActualFileLengthShouldEqual(expectedLength);
        }

        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 10)]
        [TestCase(2, -1)]
        [TestCase(2, 0)]
        public void ItShouldCalculateProperRetryDuration(int retryAttempt, int waitTimeBetweenRetryAttempts)
        {
            GivenTheWaitAndRetryReturns(waitTimeBetweenRetryAttempts);
            GivenTheWaitAndRetryCallback();
            
            WhenExecutingTheGetFileLength();
            
            ThenTheActualRetryDuractionShouldCalculated(retryAttempt, waitTimeBetweenRetryAttempts);
        }

        [Test]
        public void ItShouldRetryOnExceptionWhenNotDisabledNativeLocationValidation()
        {
            GivenTheExpectedException();
            GivenTheExpectedLogWarningMessage();
            
            GivenTheWaitAndRetryCallback();
            GivenTheLoggerWarningCallback();
            
            WhenExecutingTheGetFileLength();
            
            ThenTheActualExceptionMessageShouldEqual();
            ThenTheActualLogWarningMessageShouldEqual();
            ThenTheLoggerWarningShouldBeInvokedOneTime();
        }
        
        [Test]
        public void ItShouldRetryOnExceptionWhenDisabledNativeLocationValidation()
        {
            GivenTheExpectedInvalidPathException();
            GivenTheExpectedLogErrorMessage();
            GivenTheWaitAndRetryCallbackThatThrowsArgumentException();
            GivenTheLoggerErrorCallback();
            
            WhenExecutingIoReporterGetFileLengthThenThwowsException(true);

            ThenTheActualInvalidPathExceptionMessageShouldEqual();
            ThenTheActualLogErrorMessageShouldEqual();
            ThenTheLoggerErrorShouldBeInvokedOneTime();
        }

        private void ThenTheLoggerErrorShouldBeInvokedOneTime()
        {
            _logger.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        private void ThenTheActualLogErrorMessageShouldEqual()
        {
            Assert.That(_actualLogErrorMessage, Is.EqualTo(_expectedLogErrorMessage));
        }

        private void ThenTheActualInvalidPathExceptionMessageShouldEqual()
        {
            Assert.That(_actualInvalidPathExceptionMessage, Is.EqualTo(_EXPECTED_INVALID_PATH_EXCEPTION_MESSAGE));
        }

        #region "Helper methods"

        private void GivenTheWaitAndRetryReturns(int waitTimeBetweenRetryAttempts)
        {
            _waitAndRetry.Setup(obj => obj.WaitTimeSecondsBetweenRetryAttempts).Returns(waitTimeBetweenRetryAttempts);
        }

        private void GivenTheWaitAndRetryCallback()
        {
            _waitAndRetry.Setup(obj => obj.WaitAndRetry<Exception>(It.IsAny<Func<int, TimeSpan>>(),
                    It.IsAny<Action<Exception, TimeSpan>>(), It.IsAny<Action>()))
                .Callback<Func<int, TimeSpan>, Action<Exception, TimeSpan>, Action>((retryDuration, retryAction, execFunc) =>
                {
                    _actualRetryDuractionFunc = retryDuration;
                    retryAction(_expectedException, TimeSpan.Zero);
                    execFunc();
                });
        }

        private void GivenTheFileLength(int expectedLength)
        {
            _fileService.Setup(obj => obj.GetFileLength(_FILE_NAME)).Returns(expectedLength);
        }

        private void GivenTheLoggerWarningCallback()
        {
            _logger.Setup(logger => logger.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, logWarningMessage, param) =>
                {
                    _actualExceptionMessage = ex.Message;
                    _actualLogWarningMessage = logWarningMessage;
                });
        }

        private void GivenTheLoggerErrorCallback()
        {
            _logger.Setup(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, logWarningMessage, param) =>
                {
                    _actualInvalidPathExceptionMessage = ex.Message;
                    _actualLogErrorMessage = logWarningMessage;
                });
        }

        private void GivenTheWaitAndRetryCallbackThatThrowsArgumentException()
        {
            _waitAndRetry.Setup(obj => obj.WaitAndRetry<Exception>(It.IsAny<Func<int, TimeSpan>>(),
                    It.IsAny<Action<Exception, TimeSpan>>(), It.IsAny<Action>()))
                .Callback<Func<int, TimeSpan>, Action<Exception, TimeSpan>, Action>((retryDuration, retryAction, execFunc) =>
                {
                    retryAction(_expectedArgumentException, TimeSpan.Zero);
                });
        }

        private void GivenTheExpectedLogWarningMessage()
        {
            _expectedLogWarningMessage = IoReporter.BuildIoReporterWarningMessage(_expectedException);
        }

        private void GivenTheExpectedLogErrorMessage()
        {
            _expectedLogErrorMessage = $"File {_FILE_NAME} not found: illegal characters in path.";
        }

        private void GivenTheExpectedException()
        {
            _expectedException = new Exception(_EXPECTED_DEFAULT_EXCEPTION_MESSAGE);
        }

        private void GivenTheExpectedInvalidPathException()
        {
            _expectedArgumentException = new ArgumentException(_EXPECTED_INVALID_PATH_EXCEPTION_MESSAGE);
        }

        private void WhenExecutingTheGetFileLength(bool disableNativeLocationValidation = false)
        {
            _ioReporterInstance = new IoReporter(_fileService.Object, _waitAndRetry.Object, _logger.Object, _ioWarningPublisher, disableNativeLocationValidation);

            _actualFileLength = _ioReporterInstance.GetFileLength(_FILE_NAME, 0);
        }

        private void WhenExecutingIoReporterGetFileLengthThenThwowsException(bool disableNativeLocationValidation)
        {
            _ioReporterInstance = new IoReporter(_fileService.Object, _waitAndRetry.Object, _logger.Object, _ioWarningPublisher, disableNativeLocationValidation);

            Assert.That(() => _ioReporterInstance.GetFileLength(_FILE_NAME, 0),
                Throws.Exception.TypeOf<FileInfoInvalidPathException>());
        }

        private void ThenTheActualRetryDuractionShouldCalculated(int retryAttempt, int waitTimeBetweenRetryAttempts)
        {
            TimeSpan actualRetryDuraction = _actualRetryDuractionFunc(retryAttempt);
            Assert.That(actualRetryDuraction,
                retryAttempt == 1
                    ? Is.EqualTo(TimeSpan.FromSeconds(0))
                    : Is.EqualTo(TimeSpan.FromSeconds(waitTimeBetweenRetryAttempts)));
        }

        private void ThenTheActualFileLengthShouldEqual(int expectedLength)
        {
            Assert.That(_actualFileLength, Is.EqualTo(expectedLength));
        }

        private void ThenTheLoggerWarningShouldBeInvokedOneTime()
        {
            _logger.Verify(logger => logger.LogWarning(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        private void ThenTheActualLogWarningMessageShouldEqual()
        {
            Assert.That(_actualLogWarningMessage, Is.EqualTo(_expectedLogWarningMessage));
        }

        private void ThenTheActualExceptionMessageShouldEqual()
        {
            Assert.That(_actualExceptionMessage, Is.EqualTo(_EXPECTED_DEFAULT_EXCEPTION_MESSAGE));
        }

        #endregion
    }
}
