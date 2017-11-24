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
        private Mock<ILog> _log;
        private IoWarningPublisher _ioWarningPublisher;
        private Mock<IFileInfoFailedExceptionHelper> _fileInfoFailedExceptionHelper;
        private bool _disableNativeLocationValidation;

        [SetUp]
        public void Setup()
        {
            _fileService = new Mock<IFileSystemService>();
            _waitAndRetry = new Mock<IWaitAndRetryPolicy>();
            _log = new Mock<ILog>();
            _fileInfoFailedExceptionHelper = new Mock<IFileInfoFailedExceptionHelper>();

            _disableNativeLocationValidation = true;
        }

        [Test]
        public void ItShouldGetFileLength()
        {
            //Arrange
            _waitAndRetry.Setup(obj => obj.WaitAndRetry<Exception>(It.IsAny<Func<int, TimeSpan>>(),
                    It.IsAny<Action<Exception, TimeSpan>>(), It.IsAny<Action>()))
                .Callback<Func<int, TimeSpan>, Action<Exception, TimeSpan>, Action>((retryDuration, ) => { });

            _ioReporterInstance = new IoReporter(_fileService.Object, _waitAndRetry.Object, _log.Object, _ioWarningPublisher, _fileInfoFailedExceptionHelper.Object, _disableNativeLocationValidation);

            //Act


            //Assert

        }
    }
}
