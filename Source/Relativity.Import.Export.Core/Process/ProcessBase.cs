// ----------------------------------------------------------------------------
// <copyright file="ProcessBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Defines an abstract object that performs a runnable process.
	/// </summary>
	public abstract class ProcessBase : IRunnable, IDisposable
	{
		/// <summary>
		/// The context used to publish events.
		/// </summary>
		private readonly ProcessContext context;

		/// <summary>
		/// The logger instance.
		/// </summary>
		private readonly Relativity.Logging.ILog logger;

		/// <summary>
		/// The process error writer.
		/// </summary>
		private readonly ProcessErrorWriter errorWriter;

		/// <summary>
		/// The disposed backing.
		/// </summary>
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessBase"/> class.
		/// </summary>
		/// <param name="settings">
		/// The application settings.
		/// </param>
		/// <param name="logger">
		/// The logger instance.
		/// </param>
		protected ProcessBase(IAppSettings settings, Relativity.Logging.ILog logger)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			this.logger = logger;
			this.errorWriter = new ProcessErrorWriter(logger);
			this.context = new ProcessContext(this.errorWriter, settings, logger);
			this.ProcessId = Guid.NewGuid();
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
				this.logger.LogInformation(
					"The runnable process {ProcessType}-{ProcessId} has started.",
					this.GetType(),
					this.ProcessId);
				this.OnExecute();
				this.logger.LogInformation(
					"The runnable process {ProcessType}-{ProcessId} successfully started.",
					this.GetType(),
					this.ProcessId);
			}
			catch (Exception e)
			{
				this.logger.LogError(
					e,
					"The runnable process {ProcessType}-{ProcessId} experienced a fatal exception.",
					this.GetType(),
					this.ProcessId);
				this.context.PublishFatalException(e);
			}
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
				this.errorWriter.Dispose();
			}

			this.disposed = true;
		}

		/// <summary>
		/// Called when the process is executed.
		/// </summary>
		protected abstract void OnExecute();
	}
}