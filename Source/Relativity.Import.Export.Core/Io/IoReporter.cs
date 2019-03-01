// ----------------------------------------------------------------------------
// <copyright file="IoReporter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to perform I/O operations, publish warning messages, and retry the operation.
// </summary>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;
	using System.Globalization;
	using System.Threading;

	using Relativity.Import.Export.Resources;
	using Relativity.Logging;

	/// <summary>
	/// Represents a class object to perform I/O operations, publish warning messages, and retry the operation.
	/// </summary>
	public class IoReporter : IIoReporter
	{
		/// <summary>
		/// The value that indicates no retry information is provided.
		/// </summary>
		private const int NoRetryInfo = -1;

		/// <summary>
		/// The Relativity logger instance.
		/// </summary>
		private readonly ILog logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="IoReporter"/> class.
		/// </summary>
		/// <param name="context">
		/// The I/O reporter context.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		public IoReporter(
			IoReporterContext context,
			ILog logger,
			CancellationToken token)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			this.logger = logger;
			this.Context = context;
			this.CancellationToken = token;
			this.CachedAppSettings = context.AppSettings.DeepCopy();
		}

		/// <summary>
		/// Gets the cached settings.
		/// </summary>
		/// <value>
		/// The <see cref="AppSettingsDto"/> instance.
		/// </value>
		protected AppSettingsDto CachedAppSettings
		{
			get;
		}

		/// <summary>
		/// Gets the cancellation token.
		/// </summary>
		/// <value>
		/// The <see cref="CancellationToken"/> value.
		/// </value>
		protected CancellationToken CancellationToken
		{
			get;
		}

		/// <summary>
		/// Gets the I/O reporter context.
		/// </summary>
		/// <value>
		/// The <see cref="IoReporterContext"/> instance.
		/// </value>
		protected IoReporterContext Context
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
				throw new ArgumentOutOfRangeException(
					nameof(lineNumber),
					string.Format(Resources.Strings.LineNumberOutOfRangeExceptionMessage, nameof(lineNumber)));
			}

			this.Exec(lineNumber, sourceFileName, "Copy File", () =>
			{
				this.Context.FileSystem.File.Copy(sourceFileName, destFileName, overwrite);
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
				throw new ArgumentOutOfRangeException(
					nameof(lineNumber),
					string.Format(Resources.Strings.LineNumberOutOfRangeExceptionMessage, nameof(lineNumber)));
			}

			return this.Exec(lineNumber, fileName, "File Exists", () =>
			{
				IFileInfo fileInfo = this.Context.FileSystem.CreateFileInfo(fileName);
				bool fileExists = fileInfo.Exists;
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
	            throw new ArgumentOutOfRangeException(
		            nameof(lineNumber),
		            string.Format(Resources.Strings.LineNumberOutOfRangeExceptionMessage, nameof(lineNumber)));
            }

            return this.Exec(lineNumber, fileName, "File Length", () =>
            {
	            IFileInfo fileInfo = this.Context.FileSystem.CreateFileInfo(fileName);

				// We want any exceptions that occur when accessing properties to get thrown.
	            long fileLength = fileInfo.Length;
	            return fileLength;
            });
        }

		/// <inheritdoc />
		public void PublishRetryMessage(Exception exception, TimeSpan timeSpan, int retryCount, int totalRetryCount, long lineNumber)
		{
			var warningMessage = BuildIoReporterWarningMessage(exception, timeSpan.TotalSeconds, retryCount, totalRetryCount);
			this.Context.PublishIoWarningEvent(new IoWarningEventArgs(warningMessage, lineNumber));
			this.logger.LogWarning(exception, warningMessage);
		}

		/// <inheritdoc />
		public void PublishWarningMessage(IoWarningEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			this.Context.PublishIoWarningEvent(args);
			this.logger.LogWarning(args.Message);
		}

		private bool ThrowFileInfoInvalidPathException(Exception exception)
		{
			return this.CachedAppSettings.DisableThrowOnIllegalCharacters
			       && RetryExceptionHelper.IsIllegalCharactersInPathException(exception);
		}

        private FileInfoInvalidPathException CreateFileInfoInvalidPathException(Exception exception, string fileName)
        {
	        var message = string.Format(
		        CultureInfo.CurrentCulture,
		        Strings.ImportInvalidPathCharactersExceptionMessage,
		        fileName);
	        this.logger.LogError(exception, message);
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
				int maxRetryAttempts = this.CachedAppSettings.IoErrorNumberOfRetries;
				int currentRetryAttempt = 0;
				return this.Context.WaitAndRetryPolicy.WaitAndRetry(
					RetryExceptionHelper.CreateRetryPredicate(this.Context.RetryOptions),
					retryAttempt =>
						{
							currentRetryAttempt = retryAttempt;
							return TimeSpan.FromSeconds(
								retryAttempt == 1 ? 0 : this.CachedAppSettings.IoErrorWaitTimeInSeconds);
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
								if (this.ThrowFileInfoInvalidPathException(exception))
								{
									throw this.CreateFileInfoInvalidPathException(exception, fileName);
								}

								throw;
							}
						},
					this.CancellationToken);
			}
			catch (OperationCanceledException)
			{
				this.logger.LogInformation(
					$"The {description} I/O operation for file {fileName} and line number {lineNumber} has been canceled.");
				return default(T);
			}
		}
	}
}