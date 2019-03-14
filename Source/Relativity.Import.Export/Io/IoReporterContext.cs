// ----------------------------------------------------------------------------
// <copyright file="IoReporterContext.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;

	/// <summary>
	/// Represents a thread-safe context for <see cref="IIoReporter"/> to publish events. This class cannot be inherited.
	/// </summary>
	public sealed class IoReporterContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IoReporterContext"/> class.
		/// </summary>
		public IoReporterContext()
			: this(
				Relativity.Import.Export.Io.FileSystem.Instance.DeepCopy(),
				Relativity.Import.Export.AppSettings.Instance,
				new WaitAndRetryPolicy())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IoReporterContext"/> class.
		/// </summary>
		/// <param name="fileSystem">
		/// The file system.
		/// </param>
		/// <param name="settings">
		/// The settings.
		/// </param>
		/// <param name="waitAndRetryPolicy">
		/// The wait and retry policy.
		/// </param>
		public IoReporterContext(
			IFileSystem fileSystem,
			IAppSettings settings,
			IWaitAndRetryPolicy waitAndRetryPolicy)
		{
			if (fileSystem == null)
			{
				throw new ArgumentNullException(nameof(fileSystem));
			}

			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			if (waitAndRetryPolicy == null)
			{
				throw new ArgumentNullException(nameof(waitAndRetryPolicy));
			}

			this.FileSystem = fileSystem;
			this.AppSettings = settings;
			this.WaitAndRetryPolicy = waitAndRetryPolicy;
			this.RetryOptions = settings.RetryOptions;
		}

		/// <summary>
		/// This is an event which can be raised by any method which handles IO Warnings.
		/// </summary>
		public event EventHandler<IoWarningEventArgs> IoWarningEvent;

		/// <summary>
		/// Gets or sets the application settings.
		/// </summary>
		/// <value>
		/// The <see cref="IAppSettings"/> instance.
		/// </value>
		public IAppSettings AppSettings
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the file system wrapper.
		/// </summary>
		/// <value>
		/// The <see cref="IFileSystem"/> instance.
		/// </value>
		public IFileSystem FileSystem
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the retry options.
		/// </summary>
		/// <value>
		/// The <see cref="RetryOptions"/> value.
		/// </value>
		public RetryOptions RetryOptions
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the wait and retry policy.
		/// </summary>
		/// <value>
		/// The <see cref="IWaitAndRetryPolicy"/> instance.
		/// </value>
		public IWaitAndRetryPolicy WaitAndRetryPolicy
		{
			get;
			set;
		}

		/// <summary>
		/// Publishes the I/O warning event.
		/// </summary>
		/// <param name="eventArgs">
		/// The event arguments.
		/// </param>
		public void PublishIoWarningEvent(IoWarningEventArgs eventArgs)
		{
			this.IoWarningEvent?.Invoke(this, eventArgs);
		}
	}
}