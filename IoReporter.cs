using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.Logging;

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Base class for IO reporter
    /// </summary>
    public class IoReporter : IIoReporter
    {
        private IFileSystemService _fileSystemService;
        private IWaitAndRetryPolicy _waitAndRetryPolicy;
        private IoWarningPublisher _ioWarningPublisher;
        private IFileInfoFailedExceptionPublisher _fileInfoFailedExceptionPublisher;
        private ILog _log;
        private bool _disableNativeLocationValidation;

        /// <summary>
        /// Constructor for IO reporter
        /// </summary>
        public IoReporter(IFileSystemService fileService, IWaitAndRetryPolicy waitAndRetry, ILog log,
            IoWarningPublisher ioWarningPublisher, IFileInfoFailedExceptionPublisher fileInfoFailedExceptionPublisher, bool disableNativeLocationValidation)
        {
            _fileSystemService = fileService;
            _waitAndRetryPolicy = waitAndRetry;
            _log = log;
            _ioWarningPublisher = ioWarningPublisher;
            _fileInfoFailedExceptionPublisher = fileInfoFailedExceptionPublisher;

            _disableNativeLocationValidation = disableNativeLocationValidation;
        }

        /// <inheritdoc />
        public long GetFileLength(string fileName, int lineNumberInParentFile)
        {
            long fileLength = 0;
            _waitAndRetryPolicy.WaitAndRetry<Exception>(retryAttempt =>
                    TimeSpan.FromSeconds(retryAttempt == 1 ? 0 : _waitAndRetryPolicy.WaitTimeBetweenRetryAttempts),
                (exception, timeSpan) =>
                {
                    getFileLengthRetryAction(exception, timeSpan, fileName, lineNumberInParentFile);
                },
                () => { fileLength = _fileSystemService.GetFileLength(fileName); }
            );

            return fileLength;
        }

        private void getFileLengthRetryAction(Exception ex, TimeSpan timeSpan, string fileName, int lineNumberInParentFile)
        {
            if (_disableNativeLocationValidation && ex is ArgumentException &&
                ex.Message.Contains("Illegal characters in path."))
            {
                //TODO how to break WaitAndRetry in Polly?
                //What we should do here? throw this exceprtion
                //throw new FileInfoFailedException(String.Format("File {0} not found: illegal characters in path.", filename))
                _fileInfoFailedExceptionPublisher.Message
                throw _fileInfoFailedExceptionPublisher;
            }

            _ioWarningPublisher?.OnIoWarningEvent(new IoWarningEventArgs(timeSpan.Seconds, ex, lineNumberInParentFile));
        }
    }
}
