// ----------------------------------------------------------------------------
// <copyright file="ProcessBase2.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System;
	using System.Threading;

	using Relativity.DataExchange.Io;

	/// <summary>
	/// Defines an abstract object that performs a runnable process.
	/// </summary>
	public abstract class ProcessBase2 : IRunnable, IDisposable
	{
		/// <summary>
		/// The process error writer.
		/// </summary>
		private readonly ProcessErrorWriter processErrorWriter;

		/// <summary>
		/// The process event writer.
		/// </summary>
		private readonly ProcessEventWriter processEventWriter;

		/// <summary>
		/// The disposed backing.
		/// </summary>
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessBase2"/> class.
		/// </summary>
		protected ProcessBase2()
			: this(
				Io.FileSystem.Instance,
				DataExchange.AppSettings.Instance,
				RelativityLogFactory.CreateLog(),
				null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessBase2"/> class.
		/// </summary>
		/// <param name="fileSystem">
		/// The file system wrapper.
		/// </param>
		/// <param name="settings">
		/// The application settings.
		/// </param>
		/// <param name="logger">
		/// The logger instance.
		/// </param>
		/// <param name="tokenSource">
		/// The cancellation token source.
		/// </param>
		protected ProcessBase2(IFileSystem fileSystem, IAppSettings settings, Relativity.Logging.ILog logger, CancellationTokenSource tokenSource)
		{
			if (fileSystem == null)
			{
				throw new ArgumentNullException(nameof(fileSystem));
			}

			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			this.AppSettings = settings;
			this.CancellationTokenSource = tokenSource ?? new CancellationTokenSource();
			this.FileSystem = fileSystem;
			this.Logger = logger;
			this.processErrorWriter = new ProcessErrorWriter(fileSystem, logger);
			this.processEventWriter = new ProcessEventWriter(fileSystem);
			this.Context = new ProcessContext(this.processEventWriter, this.processErrorWriter, settings, logger);
			this.ProcessId = Guid.NewGuid();
		}

		/// <summary>
		/// Gets the process context used to publish events.
		/// </summary>
		/// <value>
		/// The <see cref="ProcessContext"/> instance.
		/// </value>
		public ProcessContext Context
		{
			get;
		}

		/// <summary>
		/// Gets or sets the process unique identifier.
		/// </summary>
		/// <value>
		/// The <see cref="Guid"/> value.
		/// </value>
		public Guid ProcessId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the application settings.
		/// </summary>
		/// <value>
		/// The <see cref="IAppSettings"/> instance.
		/// </value>
		protected IAppSettings AppSettings
		{
			get;
		}

		/// <summary>
		/// Gets the cancellation token.
		/// </summary>
		/// <value>
		/// The <see cref="CancellationToken"/> value.
		/// </value>
		protected CancellationTokenSource CancellationTokenSource
		{
			get;
		}

		/// <summary>
		/// Gets the file system wrapper.
		/// </summary>
		/// <value>
		/// The <see cref="IFileSystem"/> instance.
		/// </value>
		protected IFileSystem FileSystem
		{
			get;
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

		/// <inheritdoc />
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Starts the runnable process.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "This was done for backwards compatibility reasons.")]
		public void Start()
		{
			try
			{
				this.Logger.LogInformation(
					"The runnable process {ProcessType}-{ProcessId} has started.",
					this.GetType(),
					this.ProcessId);
				this.OnExecute();
				this.Logger.LogInformation(
					"The runnable process {ProcessType}-{ProcessId} successfully started.",
					this.GetType(),
					this.ProcessId);
			}
			catch (Exception e)
			{
				this.Logger.LogError(
					e,
					"The runnable process {ProcessType}-{ProcessId} experienced a fatal exception.",
					this.GetType(),
					this.ProcessId);
				this.Context.PublishFatalException(e);
			}
		}

		/// <summary>
		/// Creates the I/O reporter instance.
		/// </summary>
		/// <param name="context">
		/// The reporter context.
		/// </param>
		/// <returns>
		/// The <see cref="IIoReporter"/> instance.
		/// </returns>
		protected IIoReporter CreateIoReporter(IoReporterContext context)
		{
			return new IoReporter(context, this.Logger, this.CancellationTokenSource.Token);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing">
		/// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}

			if (disposing)
			{
				if (this.processErrorWriter != null)
				{
					this.processErrorWriter.Dispose();
				}

				if (this.processEventWriter != null)
				{
					this.processEventWriter.Dispose();
				}
			}

			this.disposed = true;
		}

		/// <summary>
		/// Called when the process is executed.
		/// </summary>
		protected abstract void OnExecute();
	}
}