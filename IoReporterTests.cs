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
        private Mock<IFileInfoFailedExceptionHelper> _fileInfoFailedExceptionHelper;
        private bool _disableNativeLocationValidation;
        private const string _FILE_NAME = "TestFileName";

        [SetUp]
        public void Setup()
        {
            _fileService = new Mock<IFileSystemService>();
            _waitAndRetry = new Mock<IWaitAndRetryPolicy>();
            _logger = new Mock<ILog>();
            _fileInfoFailedExceptionHelper = new Mock<IFileInfoFailedExceptionHelper>();

            _disableNativeLocationValidation = false;
        }

        [Test]
        public void ItShouldGetFileLength()
        {
            //Arrange
            const int expectedLength = 1299;
            _waitAndRetry.Setup(obj => obj.WaitAndRetry<Exception>(It.IsAny<Func<int, TimeSpan>>(),
                It.IsAny<Action<Exception, TimeSpan>>(), It.IsAny<Action>()))
                .Callback<Func<int, TimeSpan>, Action<Exception, TimeSpan>, Action>((retryDuration, retryAction, execFunc) =>
                    {
                        execFunc();
                    });
            _fileService.Setup(obj => obj.GetFileLength(_FILE_NAME)).Returns(expectedLength);
            
            _ioReporterInstance = new IoReporter(_fileService.Object, _waitAndRetry.Object, _logger.Object, _ioWarningPublisher, 
                _fileInfoFailedExceptionHelper.Object, _disableNativeLocationValidation);

            //Act
            long actualFileLength = _ioReporterInstance.GetFileLength(_FILE_NAME, 0);

            //Assert
            Assert.That(actualFileLength, Is.EqualTo(expectedLength));
        }

        
        [TestCase(1, 1)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 10)]
        [TestCase(2, -1)]
        [TestCase(2, 0)]
        public void ItShouldCalculateProperRetryDuration(int retryAttempt, int waitTimeBetweenRetryAttempts)
        {
            //Arrange
            Func<int, TimeSpan> actualRetryDuractionFunc = null;
            _waitAndRetry.Setup(obj => obj.WaitTimeBetweenRetryAttempts).Returns(waitTimeBetweenRetryAttempts);
            _waitAndRetry.Setup(obj => obj.WaitAndRetry<Exception>(It.IsAny<Func<int, TimeSpan>>(),
                    It.IsAny<Action<Exception, TimeSpan>>(), It.IsAny<Action>()))
                .Callback<Func<int, TimeSpan>, Action<Exception, TimeSpan>, Action>((retryDuration, retryAction, execFunc) =>
                {
                    actualRetryDuractionFunc = retryDuration;
                });

            _ioReporterInstance = new IoReporter(_fileService.Object, _waitAndRetry.Object, _logger.Object, _ioWarningPublisher,
                _fileInfoFailedExceptionHelper.Object, _disableNativeLocationValidation);

            //Act
            _ioReporterInstance.GetFileLength(_FILE_NAME, 0);

            //Assert
            TimeSpan actualRetryDuraction = actualRetryDuractionFunc(retryAttempt);
            Assert.That(actualRetryDuraction,
                retryAttempt == 1
                    ? Is.EqualTo(TimeSpan.FromSeconds(0))
                    : Is.EqualTo(TimeSpan.FromSeconds(waitTimeBetweenRetryAttempts)));
        }

        [Test]
        public void ItShouldRetryOnExceptionWhenNotDisabledNativeLocationValidation()
        {
            //Arrange

            const string expectedExceptionMessage = "Expected exception message";
            var expectedException = new Exception(expectedExceptionMessage);
            string expectedLogWarningMessage = IoReporter.BuildIoReporterWarningMessage(expectedException);
            string actualExceptionMessage = string.Empty;
            string actualLogWarningMessage = string.Empty;
            
            _waitAndRetry.Setup(obj => obj.WaitAndRetry<Exception>(It.IsAny<Func<int, TimeSpan>>(),
                    It.IsAny<Action<Exception, TimeSpan>>(), It.IsAny<Action>()))
                .Callback<Func<int, TimeSpan>, Action<Exception, TimeSpan>, Action>((retryDuration, retryAction, execFunc) =>
                {
                    retryAction(expectedException, TimeSpan.Zero);
                });

            _disableNativeLocationValidation = false;

            _logger.Setup(logger => logger.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, logWarningMessage, param) =>
                {
                    actualExceptionMessage = ex.Message;
                    actualLogWarningMessage = logWarningMessage;
                });

            _ioReporterInstance = new IoReporter(_fileService.Object, _waitAndRetry.Object, _logger.Object, _ioWarningPublisher,
                _fileInfoFailedExceptionHelper.Object, _disableNativeLocationValidation);

            //Act
            _ioReporterInstance.GetFileLength(_FILE_NAME, 0);

            //Assert
            Assert.That(actualExceptionMessage, Is.EqualTo(expectedExceptionMessage));
            Assert.That(actualLogWarningMessage, Is.EqualTo(expectedLogWarningMessage));
            _logger.Verify(logger => logger.LogWarning(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ItShouldRetryOnExceptionWhenDisabledNativeLocationValidation()
        {
            //Arrange

            const string expectedExceptionMessage = "Illegal characters in path.";
            var expectedArgumentException = new ArgumentException(expectedExceptionMessage);
            string expectedLogWarningMessage = $"File {_FILE_NAME} not found: illegal characters in path.";
            string actualExceptionMessage = string.Empty;
            string actualLogErrorMessage = string.Empty;
            
            _waitAndRetry.Setup(obj => obj.WaitAndRetry<Exception>(It.IsAny<Func<int, TimeSpan>>(),
                    It.IsAny<Action<Exception, TimeSpan>>(), It.IsAny<Action>()))
                .Callback<Func<int, TimeSpan>, Action<Exception, TimeSpan>, Action>((retryDuration, retryAction, execFunc) =>
                {
                    retryAction(expectedArgumentException, TimeSpan.Zero);
                });

            _disableNativeLocationValidation = true;


            _logger.Setup(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Callback<Exception, string, object[]>((ex, logWarningMessage, param) =>
                {
                    actualExceptionMessage = ex.Message;
                    actualLogErrorMessage = logWarningMessage;
                });

            const string expectedRethrowedExceptionMessage = "rethrowed exception";
            var expectedRethrowedException = new Exception(expectedRethrowedExceptionMessage);
            _fileInfoFailedExceptionHelper.Setup(helper => helper.ThrowNewException(It.IsAny<string>()))
                .Throws(expectedRethrowedException);

            _ioReporterInstance = new IoReporter(_fileService.Object, _waitAndRetry.Object, _logger.Object, _ioWarningPublisher,
                _fileInfoFailedExceptionHelper.Object, _disableNativeLocationValidation);

            //Act
            Assert.That(() => _ioReporterInstance.GetFileLength(_FILE_NAME, 0),
                Throws.Exception.TypeOf<Exception>());

            //Assert
            Assert.That(actualExceptionMessage, Is.EqualTo(expectedExceptionMessage));
            Assert.That(actualLogErrorMessage, Is.EqualTo(expectedLogWarningMessage));
            _logger.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }
    }
}
