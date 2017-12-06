// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoReporter.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents the base class for all I/O report objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;
	using System.Threading;
	using Relativity.Logging;
	using Relativity.Transfer;

	/// <summary>
	/// Base class for IO reporter
	/// </summary>
	public class IoReporter : IIoReporter
	{
		/// <summary>
		/// The value that indicates no retry information is provided.
		/// </summary>
		private const int NoRetryInfo = -1;
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

		/// <summary>
		/// Creates warning message from <paramref name="exception"/>.
		/// </summary>
		/// <param name="exception">
		/// The handled exception to report.
		/// </param>
		/// <param name="timeoutSeconds">
		/// The total time, in seconds, to wait.
		/// </param>
		/// <returns>
		/// The message.
		/// </returns>
		public static string BuildIoReporterWarningMessage(Exception exception, double timeoutSeconds)
		{
			return BuildIoReporterWarningMessage(exception, timeoutSeconds, NoRetryInfo, NoRetryInfo);
		}

		/// <summary>
		/// Creates warning message out of passed exception
		/// </summary>
		/// <param name="exception">
		/// The handled exception to report.
		/// </param>
		/// <param name="timeoutSeconds">
		/// The total time, in seconds, to wait.
		/// </param>
		/// <param name="retryCount">
		/// The current retry count.
		/// </param>
		/// <param name="totalRetryCount">
		/// The total retry count.
		/// </param>
		/// <returns>
		/// The message.
		/// </returns>
		public static string BuildIoReporterWarningMessage(Exception exception, double timeoutSeconds, int retryCount, int totalRetryCount)
		{
			var triesLeft = totalRetryCount - retryCount;
			if (triesLeft < 0)
			{
				triesLeft = 0;
			}

			if (exception == null)
			{
				if (retryCount == NoRetryInfo && totalRetryCount == NoRetryInfo)
				{
					return string.Format(Resources.Strings.IoReporterWarningMessageWithoutException, timeoutSeconds);
				}

				return string.Format(
					Resources.Strings.IoReporterWarningMessageWithoutExceptionAndRetryInfo,
					timeoutSeconds,
					triesLeft);
			}

			if (retryCount == NoRetryInfo && totalRetryCount == NoRetryInfo)
			{
				return string.Format(
					Resources.Strings.IoReporterWarningMessageWithException,
					timeoutSeconds,
					exception.Message);
			}

			return string.Format(
				Resources.Strings.IoReporterWarningMessageWithExceptionAndRetryInfo,
				timeoutSeconds,
				triesLeft,
				exception.Message);
		}

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
					this.GetFileLengthRetryAction(exception, lineNumberInParentFile, timeSpan.TotalSeconds);
				},
				(_cancellationToken) =>
				{
					fileLength = GetFileLengthAction(fileName);
				},
				_cancellationToken
			);

			return fileLength;
		}

		private void GetFileLengthRetryAction(Exception ex, int lineNumberInParentFile, double timeoutSeconds)
		{
			var warningMessage = BuildIoReporterWarningMessage(ex, timeoutSeconds);
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
					var errorMessage = $"File {fileName} not found: illegal characters in path.";
					_log.LogError(ex, errorMessage);
					throw new FileInfoInvalidPathException(errorMessage);
				}

				throw;
			}
		}
	}
}
