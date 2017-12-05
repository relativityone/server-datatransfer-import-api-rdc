using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
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
		private readonly ILog _log;
		private readonly bool _disableNativeLocationValidation;
		private readonly CancellationToken _cancellationToken;

		/// <summary>
		/// Constructor for IO reporter
		/// </summary>
		public IoReporter(IFileSystemService fileService, IWaitAndRetryPolicy waitAndRetry, ILog log,
			IoWarningPublisher ioWarningPublisher, bool disableNativeLocationValidation, CancellationToken cancellationToken)
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
			
			_fileSystemService = fileService;
			_waitAndRetryPolicy = waitAndRetry;
			_log = log;
			_ioWarningPublisher = ioWarningPublisher;

			_disableNativeLocationValidation = disableNativeLocationValidation;
			_cancellationToken = cancellationToken;
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
				throw new ArgumentOutOfRangeException(nameof(lineNumberInParentFile), string.Format(Resources.Strings.LineNumberOutOfRangeExceptionMessage, nameof(lineNumberInParentFile)));
			}

			long fileLength = 0;
			_waitAndRetryPolicy.WaitAndRetry<Exception>(
				retryAttempt => TimeSpan.FromSeconds(retryAttempt == 1 ? 0 : _waitAndRetryPolicy.WaitTimeSecondsBetweenRetryAttempts),
				(exception, timeSpan) =>
				{
					GetFileLengthRetryAction(exception, lineNumberInParentFile);
				},
				(_cancellationToken) =>
				{
					fileLength = GetFileLengthAction(fileName);
				},
				_cancellationToken
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


		private void GetFileLengthRetryAction(Exception ex, int lineNumberInParentFile)
		{
			string warningMessage = BuildIoReporterWarningMessage(ex);
			_ioWarningPublisher?.PublishIoWarningEvent(new IoWarningEventArgs(warningMessage, lineNumberInParentFile));
			_log.LogWarning(ex, warningMessage);
		}

		private long GetFileLengthAction(string fileName)
		{
			try
			{
				return _fileSystemService.GetFileLength(fileName);
			}
			catch (Exception ex)
			{
				if (_disableNativeLocationValidation && ex is ArgumentException &&
					ex.Message.Contains("Illegal characters in path."))
				{
					string errorMessage = $"File {fileName} not found: illegal characters in path.";
					_log.LogError(ex, errorMessage);

					throw new FileInfoInvalidPathException(errorMessage);
				}

				throw;
			}
		}
	}
}
