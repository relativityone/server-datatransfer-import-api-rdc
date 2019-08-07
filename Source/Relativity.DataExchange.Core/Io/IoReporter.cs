// ----------------------------------------------------------------------------
// <copyright file="IoReporter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to perform I/O operations, publish warning messages, and retry the operation.
// </summary>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System;
	using System.Globalization;
	using System.Threading;

	using Relativity.DataExchange.Resources;
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
		/// The shadow copy for the configurable I/O max retry count.
		/// </summary>
		/// <remarks>
		/// This doubled GetChar performance.
		/// </remarks>
		private int? ioErrorNumberOfRetriesShadowCopy;

		/// <summary>
		/// The shadow copy for the configurable I/O wait time.
		/// </summary>
		/// <remarks>
		/// This doubled GetChar performance.
		/// </remarks>
		private int? ioErrorWaitTimeInSecondsShadowCopy;

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

			this.Logger = logger;
			this.Context = context;
			this.CancellationToken = token;
			this.CachedAppSettings = context.AppSettings.DeepCopy();
		}

		/// <summary>
		/// Gets the I/O reporter context.
		/// </summary>
		/// <value>
		/// The <see cref="IoReporterContext"/> instance.
		/// </value>
		public IoReporterContext Context
		{
			get;
		}

		/// <summary>
		/// Gets the cached application settings.
		/// </summary>
		/// <value>
		/// The <see cref="IAppSettings"/> instance.
		/// </value>
		protected IAppSettings CachedAppSettings
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
		/// Gets the maximum number of retries. This value is cached for performance critical code.
		/// </summary>
		/// <value>
		/// The retry count.
		/// </value>
		protected int IoErrorNumberOfRetries
		{
			get
			{
				// REL-343213: Caching used for performance critical code.
				if (this.ioErrorNumberOfRetriesShadowCopy == null)
				{
					// Just in case, sanity check the min value.
					this.ioErrorNumberOfRetriesShadowCopy = this.CachedAppSettings.IoErrorNumberOfRetries;
					if (this.CachedAppSettings.EnforceMinRetryCount && this.ioErrorNumberOfRetriesShadowCopy < 1)
					{
						this.ioErrorNumberOfRetriesShadowCopy = 1;
					}
				}

				return this.ioErrorNumberOfRetriesShadowCopy.Value;
			}
		}

		/// <summary>
		/// Gets the wait time in seconds between retry attempts. This value is cached for performance critical code.
		/// </summary>
		/// <value>
		/// The retry count.
		/// </value>
		protected int IoErrorWaitTimeInSeconds
		{
			get
			{
				// REL-343213: Caching used for performance critical code.
				if (this.ioErrorWaitTimeInSecondsShadowCopy == null)
				{
					// Just in case, sanity check the min value.
					this.ioErrorWaitTimeInSecondsShadowCopy = this.CachedAppSettings.IoErrorWaitTimeInSeconds;
					if (this.CachedAppSettings.EnforceMinWaitTime && this.ioErrorWaitTimeInSecondsShadowCopy < 1)
					{
						this.ioErrorWaitTimeInSecondsShadowCopy = 1;
					}
				}

				return this.ioErrorWaitTimeInSecondsShadowCopy.Value;
			}
		}

		/// <summary>
		/// Gets the Relativity logger.
		/// </summary>
		/// <value>
		/// The <see cref="Relativity.Logging.ILog"/> instance.
		/// </value>
		protected Relativity.Logging.ILog Logger
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
		/// Creates warning message out of passed exception.
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
		public virtual void CopyFile(string sourceFileName, string destFileName, bool overwrite, int lineNumber)
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
		public virtual bool GetFileExists(string fileName, int lineNumber)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return false;
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
		public virtual long GetFileLength(string fileName, int lineNumber)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return 0;
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
		public virtual void PublishRetryMessage(Exception exception, TimeSpan timeSpan, int retryCount, int totalRetryCount, long lineNumber)
		{
			var warningMessage = BuildIoReporterWarningMessage(exception, timeSpan.TotalSeconds, retryCount, totalRetryCount);
			this.Context.PublishIoWarningEvent(new IoWarningEventArgs(warningMessage, lineNumber));
			this.Logger.LogWarning(exception, warningMessage);
		}

		/// <inheritdoc />
		public virtual void PublishWarningMessage(IoWarningEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			this.Context.PublishIoWarningEvent(args);
			this.Logger.LogWarning(args.Message);
		}

		private bool ThrowFileInfoInvalidPathException(Exception exception)
		{
			return this.CachedAppSettings.DisableThrowOnIllegalCharacters
				   && ExceptionHelper.IsIllegalCharactersInPathException(exception);
		}

		private FileInfoInvalidPathException CreateFileInfoInvalidPathException(Exception exception, string fileName)
		{
			var message = string.Format(
				CultureInfo.CurrentCulture,
				Strings.ImportInvalidPathCharactersExceptionMessage,
				fileName);
			this.Logger.LogError(exception, message);
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
				int maxRetryAttempts = this.IoErrorNumberOfRetries;
				int currentRetryAttempt = 0;
				return this.Context.WaitAndRetryPolicy.WaitAndRetry(
					RetryExceptionHelper.CreateRetryPredicate(this.Context.RetryOptions),
					retryAttempt =>
						{
							currentRetryAttempt = retryAttempt;
							return TimeSpan.FromSeconds(retryAttempt == 1 ? 0 : this.IoErrorWaitTimeInSeconds);
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
				this.Logger.LogInformation(
					$"The {description} I/O operation for file {fileName} and line number {lineNumber} has been canceled.");
				return default(T);
			}
		}
	}
}