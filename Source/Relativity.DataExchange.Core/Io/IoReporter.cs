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

	using Relativity.DataExchange.Logger;
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1062:Validate arguments of public methods",
			MessageId = "0",
			Justification = "The context argument is validated via ThrowIfNull.")]
		public IoReporter(IoReporterContext context, ILog logger, CancellationToken token)
		{
			this.Logger = logger.ThrowIfNull(nameof(logger));
			this.Context = context.ThrowIfNull(nameof(context));
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
					string.Format(Strings.LineNumberOutOfRangeExceptionMessage, nameof(lineNumber)));
			}

			this.Context.FileSystem.File.Copy(sourceFileName, destFileName, overwrite);
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
		public virtual void PublishRetryMessage(
			Exception exception,
			TimeSpan timeSpan,
			int retryCount,
			int totalRetryCount,
			long lineNumber)
		{
			string warningMessage = BuildIoReporterWarningMessage(
				exception,
				timeSpan.TotalSeconds,
				retryCount,
				totalRetryCount);
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

		/// <summary>
		/// Calculates the total number of seconds to wait between each retry attempt.
		/// </summary>
		/// <param name="numberOfRetries">
		/// The number of retries left.
		/// </param>
		/// <returns>
		/// The total number of seconds.
		/// </returns>
		internal int CalculateWaitTimeInSeconds(int numberOfRetries)
		{
			int waitTime = 0;
			if (numberOfRetries < this.IoErrorNumberOfRetries - 1)
			{
				waitTime = this.IoErrorWaitTimeInSeconds;
				if (waitTime < 0)
				{
					waitTime = 0;
				}
			}

			return waitTime;
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
			this.Logger.LogError(exception, "{message}", message.Secure());
			return new FileInfoInvalidPathException(message);
		}

		private void LogWarning(
			Exception exception,
			TimeSpan timeSpan,
			int retryCount,
			int totalRetryCount)
		{
			var warningMessage = BuildIoReporterWarningMessage(
				exception,
				timeSpan.TotalSeconds,
				retryCount,
				totalRetryCount);
			this.Logger.LogWarning(exception, warningMessage);
		}

		private void LogCancellation(int lineNumber, string fileName, string description)
		{
			this.Logger.LogInformation("The {description} I/O operation for file {fileName} and line number {lineNumber} has been canceled.", description, fileName.Secure(), lineNumber);
		}

		private T Exec<T>(
			int lineNumber,
			string fileName,
			string description,
			Func<T> execFunc)
		{
			// REL-343213: This is performance critical code and no longer using WaitAndRetryPolicy.
			T result = default(T);
			int maxRetries = this.IoErrorNumberOfRetries;
			int numberOfRetries = maxRetries;
			Func<Exception, bool> retryPredicate = null;
			while (numberOfRetries > 0)
			{
				if (this.CancellationToken.IsCancellationRequested)
				{
					this.LogCancellation(lineNumber, fileName, description);
					break;
				}

				try
				{
					numberOfRetries--;
					result = execFunc();
					break;
				}
				catch (OperationCanceledException)
				{
					this.LogCancellation(lineNumber, fileName, description);
					break;
				}
				catch (Exception e)
				{
					if (retryPredicate == null)
					{
						retryPredicate = RetryExceptionHelper.CreateRetryPredicate(this.Context.RetryOptions);
					}

					if (!retryPredicate(e))
					{
						throw;
					}

					int retryAttemptNumber = maxRetries - numberOfRetries;
					int waitTime = this.CalculateWaitTimeInSeconds(numberOfRetries);
					TimeSpan timeSpan = TimeSpan.FromSeconds(waitTime);
					if (this.ThrowFileInfoInvalidPathException(e))
					{
						this.LogWarning(e, timeSpan, retryAttemptNumber, maxRetries);
						throw this.CreateFileInfoInvalidPathException(e, fileName);
					}

					if (numberOfRetries == 0 || this.Context.RetryOptions == RetryOptions.None)
					{
						this.LogWarning(e, timeSpan, retryAttemptNumber, maxRetries);
						throw;
					}

					this.PublishRetryMessage(e, timeSpan, retryAttemptNumber, maxRetries, lineNumber);
					if (waitTime > 0)
					{
						Thread.CurrentThread.Join(1000 * waitTime);
					}
				}
			}

			return result;
		}
	}
}