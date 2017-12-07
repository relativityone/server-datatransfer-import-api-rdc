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
    using System.Globalization;
    using kCura.WinEDDS.TApi.Resources;

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
        private readonly IFileSystemService fileSystemService;
        private readonly IWaitAndRetryPolicy waitAndRetryPolicy;
        private readonly IoWarningPublisher publisher;
        private readonly ILog log;
        private readonly bool disableNativeLocationValidation;
		private readonly CancellationToken cancellationToken;

		/// <summary>
		/// Constructor for IO reporter
		/// </summary>
		/// <param name="fileService">
		/// The file system service.
		/// </param>
		/// <param name="waitAndRetry">
		/// The resiliency component.
		/// </param>
		/// <param name="log">
		/// The Relativity log.
		/// </param>
		/// <param name="publisher">
		/// The I/O warning publisher.
		/// </param>
		/// <param name="disableNativeLocationValidation">
		/// if set to <c>true</c> [disable native location validation].
		/// </param>
		/// <param name="cancellationToken">
		/// The Cancel Token used to stop the process a any requested time.</param>
		public IoReporter(
            IFileSystemService fileService,
            IWaitAndRetryPolicy waitAndRetry,
            ILog log,
            IoWarningPublisher publisher,
			bool disableNativeLocationValidation, 
			CancellationToken cancellationToken)
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

            if (publisher == null)
			{
                throw new ArgumentNullException(nameof(publisher));
			}

            this.fileSystemService = fileService;
            this.waitAndRetryPolicy = waitAndRetry;
            this.log = log;
            this.publisher = publisher;
            this.disableNativeLocationValidation = disableNativeLocationValidation;
			this.cancellationToken = cancellationToken;
		}

		/// <inheritdoc />
        public IoWarningPublisher IOWarningPublisher => this.publisher;

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

            return this.waitAndRetryPolicy.WaitAndRetry<long, Exception>(
                retryAttempt => TimeSpan.FromSeconds(
                    retryAttempt == 1 ? 0 : this.waitAndRetryPolicy.WaitTimeSecondsBetweenRetryAttempts),
                (exception, timeSpan) =>
                    {
                        this.HandleGetFileLengthException(
                            exception,
                            lineNumberInParentFile,
                            timeSpan.TotalSeconds);
                    },
                (cancellationToken) => this.GetFileLength(fileName),
                cancellationToken
            );
        }

        /// <summary>
        ///  Handles all exceptions when retrieving the file length fails.
        /// </summary>
        /// <param name="exception">
        /// The thrown exception.
        /// </param>
        /// <param name="lineNumberInParentFile">
        /// The line number from the parent file.
        /// </param>
        /// <param name="timeoutSeconds">
        /// The timeout in seconds.
        /// </param>
        /// <exception cref="FileInfoInvalidPathException">
        /// Thrown when the path contains invalid path characters.
        /// </exception>
        private void HandleGetFileLengthException(Exception exception, int lineNumberInParentFile, double timeoutSeconds)
		{
			var warningMessage = BuildIoReporterWarningMessage(exception, timeoutSeconds);
			publisher?.PublishIoWarningEvent(new IoWarningEventArgs(warningMessage, lineNumberInParentFile));
			log.LogWarning(exception, warningMessage);
		}

		private long GetFileLength(string fileName)
		{
			try
			{
				return this.fileSystemService.GetFileLength(fileName);
			}
			catch (Exception exception)
			{
            if (this.disableNativeLocationValidation && exception is ArgumentException &&
                exception.Message.Contains("Illegal characters in path."))
				{
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    Strings.ImportInvalidPathCharactersExceptionMessage,
                    fileName);
                this.log.LogError(exception, message);
                throw new FileInfoInvalidPathException(message);
				}

				throw;
			}
		}
	}
}
