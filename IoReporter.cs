using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.Logging;
using Relativity.Transfer;

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
        private IFileInfoFailedExceptionHelper _fileInfoFailedExceptionHelper;
        private ILog _log;
        private bool _disableNativeLocationValidation;

        /// <summary>
        /// Constructor for IO reporter
        /// </summary>
        public IoReporter(IFileSystemService fileService, IWaitAndRetryPolicy waitAndRetry, ILog log,
            IoWarningPublisher ioWarningPublisher, IFileInfoFailedExceptionHelper fileInfoFailedExceptionHelper, bool disableNativeLocationValidation)
        {
            _fileSystemService = fileService;
            _waitAndRetryPolicy = waitAndRetry;
            _log = log;
            _ioWarningPublisher = ioWarningPublisher;
            _fileInfoFailedExceptionHelper = fileInfoFailedExceptionHelper;

            _disableNativeLocationValidation = disableNativeLocationValidation;
        }

        /// <inheritdoc />
        public IoWarningPublisher IOWarningPublisher => _ioWarningPublisher;

        /// <inheritdoc />
        public long GetFileLength(string fileName, int lineNumberInParentFile)
        {
            long fileLength = 0;
            _waitAndRetryPolicy.WaitAndRetry<Exception>(retryAttempt =>
                    TimeSpan.FromSeconds(retryAttempt == 1 ? 0 : _waitAndRetryPolicy.WaitTimeBetweenRetryAttempts),
                (exception, timeSpan) =>
                {
                    GetFileLengthRetryAction(exception, timeSpan, fileName, lineNumberInParentFile);
                },
                () => { fileLength = _fileSystemService.GetFileLength(fileName); }
            );

            return fileLength;
        }

        private void GetFileLengthRetryAction(Exception ex, TimeSpan timeSpan, string fileName, int lineNumberInParentFile)
        {
            if (_disableNativeLocationValidation && ex is ArgumentException &&
                ex.Message.Contains("Illegal characters in path."))
            {
                string errorMessage = $"File {fileName} not found: illegal characters in path.";
                _log.LogError(ex, errorMessage);
                _fileInfoFailedExceptionHelper.ThrowNewException(errorMessage);
            }

            string warningMessage = BuildIOReporterWarningMessage(ex);

            _ioWarningPublisher?.OnIoWarningEvent(new IoWarningEventArgs(timeSpan.Seconds, ex, warningMessage, lineNumberInParentFile));
            
            _log.LogWarning(ex, warningMessage);
        }

        /// <inheritdoc />
        public string BuildIOReporterWarningMessage(Exception ex)
        {
            return $"Error when accessing load file - retrying. Actual error: {ex.Message}";   
        }
    }
}
