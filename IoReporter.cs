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

	/// <summary>
	/// Base class for IO reporter
	/// </summary>
	public class IoReporter : IIoReporter
	{
		/// <summary>
		/// The value that indicates no retry information is provided.
		/// </summary>
		private const int NoRetryInfo = -1;

		private readonly IFileSystem fileSystem;
		private readonly IWaitAndRetryPolicy waitAndRetryPolicy;
		private readonly ILog log;
        private readonly bool disableNativeLocationValidation;
		private readonly CancellationToken cancellationToken;

		/// <summary>
		/// Constructor for IO reporter
		/// </summary>
		/// <param name="fileSystem">
		/// The file system wrapper.
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
			IFileSystem fileSystem,
            IWaitAndRetryPolicy waitAndRetry,
            ILog log,
            IoWarningPublisher publisher,
			bool disableNativeLocationValidation,
			CancellationToken cancellationToken)
		{
			if (fileSystem == null)
			{
				throw new ArgumentNullException(nameof(fileSystem));
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

			this.fileSystem = fileSystem;
            this.waitAndRetryPolicy = waitAndRetry;
            this.log = log;
            this.IOWarningPublisher = publisher;
            this.disableNativeLocationValidation = disableNativeLocationValidation;
			this.cancellationToken = cancellationToken;
		}

		/// <inheritdoc />
		public IoWarningPublisher IOWarningPublisher
		{
			get;
		}

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
		public void CopyFile(string sourceFileName, string destFileName, bool overwrite, int lineNumber)
		{
			if (string.IsNullOrEmpty(sourceFileName))
			{
				throw new ArgumentNullException(nameof(sourceFileName));
			}

			if (string.IsNullOrEmpty(destFileName))
			{
				throw new ArgumentNullException(nameof(destFileName));
			}

			if (lineNumber < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(lineNumber),
					string.Format(Resources.Strings.LineNumberOutOfRangeExceptionMessage, nameof(lineNumber)));
			}

			this.Exec(lineNumber, sourceFileName, "Copy File", () =>
			{
				this.fileSystem.File.Copy(sourceFileName, destFileName, overwrite);
				return 0;
			});
		}

		/// <inheritdoc />
		public bool GetFileExists(string fileName, int lineNumber)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException(nameof(fileName));
			}

			if (lineNumber < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(lineNumber),
					string.Format(Resources.Strings.LineNumberOutOfRangeExceptionMessage, nameof(lineNumber)));
			}

			return this.Exec(lineNumber, fileName, "File Exists", () =>
			{
				IFileInfo fileInfo = this.fileSystem.CreateFileInfo(fileName);
				bool fileExists =  fileInfo != null && fileInfo.Exists;
				return fileExists;
			});
		}

		/// <inheritdoc />
		public long GetFileLength(string fileName, int lineNumber)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (lineNumber < 0)
            {
	            throw new ArgumentOutOfRangeException(nameof(lineNumber),
		            string.Format(Resources.Strings.LineNumberOutOfRangeExceptionMessage, nameof(lineNumber)));
            }

            return this.Exec(lineNumber, fileName, "File Length", () =>
            {
	            IFileInfo fileInfo = this.fileSystem.CreateFileInfo(fileName);

				// We want any exceptions that occur when accessing properties to get thrown.
	            long fileLength = fileInfo.Length;
	            return fileLength;
            });
        }

		/// <inheritdoc />
		public void PublishRetryMessage(Exception exception, TimeSpan timeSpan, int retryCount, int totalRetryCount, long lineNumber)
		{
			var warningMessage = BuildIoReporterWarningMessage(exception, timeSpan.TotalSeconds, retryCount, totalRetryCount);
			this.IOWarningPublisher.PublishIoWarningEvent(new IoWarningEventArgs(warningMessage, lineNumber));
			log.LogWarning(exception, warningMessage);
		}

		private bool CheckInvalidPathCharactersException(Exception exception)
        {
	        return this.disableNativeLocationValidation && exception is ArgumentException &&
	               exception.Message.Contains("Illegal characters in path.");
        }

        private FileInfoInvalidPathException CreateInvalidPathCharactersException(Exception exception, string fileName)
        {
	        var message = string.Format(
		        CultureInfo.CurrentCulture,
		        Strings.ImportInvalidPathCharactersExceptionMessage,
		        fileName);
	        this.log.LogError(exception, message);
	        return new FileInfoInvalidPathException(message);
		}

		private T Exec<T>(
			int lineNumber,
			string fileName,
			string description,
			Func<T> execFunc)
		{
			try
			{
				int maxRetryAttempts = this.waitAndRetryPolicy.MaxRetryAttempts;
				int currentRetryAttempt = 0;
				return this.waitAndRetryPolicy.WaitAndRetry(
					RetryPolicies.IoStandardPolicy,
					retryAttempt =>
					{
						currentRetryAttempt = retryAttempt;
						return TimeSpan.FromSeconds(retryAttempt == 1
							? 0
							: this.waitAndRetryPolicy.WaitTimeSecondsBetweenRetryAttempts);
					},
					(exception, timeSpan) =>
					{
						this.PublishRetryMessage(
							exception,
							timeSpan,
							currentRetryAttempt,
							maxRetryAttempts,
							lineNumber);
					},
					(ct) =>
					{
						try
						{
							return execFunc();
						}
						catch (Exception exception)
						{
							if (this.CheckInvalidPathCharactersException(exception))
							{
								throw this.CreateInvalidPathCharactersException(exception, fileName);
							}

							throw;
						}
					},
					cancellationToken
				);
			}
			catch (OperationCanceledException)
			{
				this.log.LogInformation(
					$"The {description} I/O operation for file {fileName} and line number {lineNumber} has been canceled.");
				return default(T);
			}
		}
	}
}