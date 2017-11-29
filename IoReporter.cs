﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
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
        private readonly IFileSystemService _fileSystemService;
        private readonly IWaitAndRetryPolicy _waitAndRetryPolicy;
        private readonly IoWarningPublisher _ioWarningPublisher;
        private readonly IFileInfoFailedExceptionHelper _fileInfoFailedExceptionHelper;
        private readonly ILog _log;
        private readonly bool _disableNativeLocationValidation;

        /// <summary>
        /// Constructor for IO reporter
        /// </summary>
        public IoReporter(IFileSystemService fileService, IWaitAndRetryPolicy waitAndRetry, ILog log,
            IoWarningPublisher ioWarningPublisher, IFileInfoFailedExceptionHelper fileInfoFailedExceptionHelper, bool disableNativeLocationValidation)
        {
	        if (fileService == null)
	        {
		        throw new ArgumentNullException(nameof(fileService));
	        }

	        if (waitAndRetry == null)
	        {
		        throw new ArgumentNullException(nameof(waitAndRetry));
			}

			if (log == null)
			{
				throw new ArgumentNullException(nameof(log));
			}

	        if (ioWarningPublisher == null)
	        {
		        throw new ArgumentNullException(nameof(ioWarningPublisher));
			}

			if (fileInfoFailedExceptionHelper == null)
			{
				throw new ArgumentNullException(nameof(fileInfoFailedExceptionHelper));
			}

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
	        if (string.IsNullOrEmpty(fileName))
	        {
		        throw new ArgumentNullException(nameof(fileName));
	        }
	        if (lineNumberInParentFile < 0)
	        {
		        throw new ArgumentOutOfRangeException(nameof(lineNumberInParentFile), string.Format(Resources.Strings.LineNumberOutOfRangeExceptionMessage, nameof(lineNumberInParentFile)) );
	        }

	        long fileLength = 0;
            _waitAndRetryPolicy.WaitAndRetry<Exception>(retryAttempt =>
                    TimeSpan.FromSeconds(retryAttempt == 1 ? 0 : _waitAndRetryPolicy.WaitTimeSecondsBetweenRetryAttempts),
                (exception, timeSpan) =>
                {
                    GetFileLengthRetryAction(exception, fileName, lineNumberInParentFile);
                },
                () => { fileLength = _fileSystemService.GetFileLength(fileName); }
            );

            return fileLength;
        }
		
        /// <summary>
		/// Creates warning message out of passed exception
		/// </summary>
		public static string BuildIoReporterWarningMessage(Exception ex)
        {
	        if (ex == null)
	        {
		        return Resources.Strings.IoReporterWarningMessageWithoutException;
	        }

	        return string.Format(Resources.Strings.IoReporterWarningMessageWithException, ex.Message);
        }
   

		private void GetFileLengthRetryAction(Exception ex, string fileName, int lineNumberInParentFile)
        {
            if (_disableNativeLocationValidation && ex is ArgumentException &&
                ex.Message.Contains("Illegal characters in path."))
            {
                string errorMessage = $"File {fileName} not found: illegal characters in path.";
                _log.LogError(ex, errorMessage);
                _fileInfoFailedExceptionHelper.ThrowNewException(errorMessage);
            }

            string warningMessage = BuildIoReporterWarningMessage(ex);

            _ioWarningPublisher?.PublishIoWarningEvent(new IoWarningEventArgs(warningMessage, lineNumberInParentFile));
            
            _log.LogWarning(ex, warningMessage);
		}
	}
}
