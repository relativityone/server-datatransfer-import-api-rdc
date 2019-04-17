// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiBridgeBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to provide a Transfer API bridge to existing WinEDDS code.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Threading;

	using Polly;

	using Relativity.Import.Export.Resources;
	using Relativity.Transfer;
	using Relativity.Transfer.Http;

	/// <summary>
	/// Represents a class object to provide a bridge from the Transfer API to existing Import/Export code.
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public abstract class TapiBridgeBase : IDisposable
	{
		/// <summary>
		/// The Transfer API object service.
		/// </summary>
		private readonly ITapiObjectService tapiObjectService;

		/// <summary>
		/// The cancellation token source.
		/// </summary>
		private readonly CancellationToken cancellationToken;

		/// <summary>
		/// The context used for transfer events.
		/// </summary>
		private readonly TransferContext transferContext;

		/// <summary>
		/// The native file transfer parameters.
		/// </summary>
		private readonly TapiBridgeParameters parameters;

		/// <summary>
		/// The file system service used to wrap up all IO API's.
		/// </summary>
		private readonly IFileSystemService fileSystemService;

		/// <summary>
		/// The list of transfer event listeners.
		/// </summary>
		private readonly List<TapiListenerBase> transferListeners = new List<TapiListenerBase>();

		/// <summary>
		/// The current transfer direction.
		/// </summary>
		private readonly TransferDirection currentDirection;

		/// <summary>
		/// The job unique identifier associated with the current job.
		/// </summary>
		private Guid? currentJobId;

		/// <summary>
		/// The current job number. This is tagged to the request to provide a non-zero line number.
		/// </summary>
		private int currentJobNumber;

		/// <summary>
		/// The current job request.
		/// </summary>
		private ITransferRequest jobRequest;

		/// <summary>
		/// The lazy constructed transfer client. Always call <see cref="CreateTransferHost"/>
		/// to get the reference instead of directly accessing this field.
		/// </summary>
		private IRelativityTransferHost relativityTransferHost;

		/// <summary>
		/// The transfer client.
		/// </summary>
		private ITransferClient transferClient;

		/// <summary>
		/// The disposed backing.
		/// </summary>
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiBridgeBase"/> class.
		/// </summary>
		/// <param name="service">
		/// The Transfer API object service.
		/// </param>
		/// <param name="parameters">
		/// The native file transfer parameters.
		/// </param>
		/// <param name="direction">
		/// The transfer direction.
		/// </param>
		/// <param name="log">
		/// The transfer log.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		/// <remarks>
		/// Don't expose Transfer API objects to WinEDDS - at least not yet. This is reserved for integration tests.
		/// </remarks>
		internal TapiBridgeBase(
			ITapiObjectService service,
			TapiBridgeParameters parameters,
			TransferDirection direction,
			ITransferLog log,
			CancellationToken token)
		{
			if (service == null)
			{
				throw new ArgumentNullException(nameof(service));
			}

			// Note: Do NOT argument check TargetPath. This is null for metadata-only transfers.
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (parameters.Credentials == null)
			{
				throw new ArgumentException("The credentials information must be specified.", nameof(parameters));
			}

			if (parameters.WorkspaceId < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(parameters), Strings.WorkspaceExceptionMessage);
			}

			if (parameters.WebCookieContainer == null)
			{
				parameters.WebCookieContainer = new CookieContainer();
			}

			this.tapiObjectService = service;
			this.fileSystemService = service.CreateFileSystemService();
			this.currentDirection = direction;
			this.parameters = parameters;
			this.TargetPath = parameters.TargetPath;
			this.cancellationToken = token;
			this.TransferLog = log;
			this.currentJobNumber = 0;
			this.transferContext = new TransferContext
			{
				StatisticsRateSeconds = 1.0,
				LargeFileProgressEnabled = parameters.LargeFileProgressEnabled,
			};

			this.SetupTransferListeners();
			this.UpdateAllTransferListenersClientName();
		}

		/// <summary>
		/// Occurs when a status message is available.
		/// </summary>
		public event EventHandler<TapiMessageEventArgs> TapiStatusMessage;

		/// <summary>
		/// Occurs when a non-fatal error message is available.
		/// </summary>
		public event EventHandler<TapiMessageEventArgs> TapiErrorMessage;

		/// <summary>
		/// Occurs when a warning message is available.
		/// </summary>
		public event EventHandler<TapiMessageEventArgs> TapiWarningMessage;

		/// <summary>
		/// Occurs when the transfer client is changed.
		/// </summary>
		public event EventHandler<TapiClientEventArgs> TapiClientChanged;

		/// <summary>
		/// Occurs when a path finishes transferring.
		/// </summary>
		public event EventHandler<TapiProgressEventArgs> TapiProgress;

		/// <summary>
		/// Occurs when transfer statistics are available.
		/// </summary>
		public event EventHandler<TapiStatisticsEventArgs> TapiStatistics;

		/// <summary>
		/// Occurs when there is a fatal error in the transfer.
		/// </summary>
		public event EventHandler<TapiMessageEventArgs> TapiFatalError;

		/// <summary>
		/// Gets the current transfer client.
		/// </summary>
		/// <value>
		/// The <see cref="TapiClient"/> value.
		/// </value>
		public TapiClient Client => this.tapiObjectService.GetTapiClient(this.ClientId);

		/// <summary>
		/// Gets the current transfer client unique identifier.
		/// </summary>
		/// <value>
		/// The <see cref="Guid"/> value.
		/// </value>
		public Guid ClientId => this.transferClient?.Id ?? Guid.Empty;

		/// <summary>
		/// Gets or sets the target path.
		/// </summary>
		/// <value>
		/// The path.
		/// </value>
		public string TargetPath
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the transfer job.
		/// </summary>
		/// <value>
		/// The <see cref="ITransferJob"/> instance.
		/// </value>
		protected ITransferJob TransferJob { get; private set; }

		/// <summary>
		/// Gets the Relativity transfer log.
		/// </summary>
		/// <value>
		/// The <see cref="ITransferLog"/> instance.
		/// </value>
		protected ITransferLog TransferLog { get; }

		/// <summary>
		/// Gets the current transfer client display name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		private string ClientDisplayName => this.transferClient?.DisplayName ?? Strings.ClientInitializing;

		/// <summary>
		/// Adds the path to a transfer job.
		/// </summary>
		/// <param name="path">
		/// The path to add to the job.
		/// </param>
		/// <returns>
		/// The file name.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		public string AddPath(TransferPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			this.CheckDispose();
			this.CreateTransferJob(false);
			if (this.TransferJob == null)
			{
				throw new InvalidOperationException(Strings.TransferJobNullExceptionMessage);
			}

			try
			{
				const int RetryAttempts = 2;
				Exception transferException = null;
				var result = Policy.Handle<TransferException>().Retry(
					RetryAttempts,
					(exception, count) =>
						{
							// This will automatically add paths.
							transferException = exception;
							this.TransferLog.LogError(exception, "Failed to add a path to the transfer job.");
							this.FallbackHttpClient(exception, path);
						}).Execute(
						() =>
						{
							// Fallback automatically attempts to add paths. Make sure the path isn't added twice.
							if (transferException == null || !this.GetIsTransferPathInJobQueue(path))
							{
								this.TransferJob.AddPath(path, this.cancellationToken);
							}

							return !string.IsNullOrEmpty(path.TargetFileName)
									   ? path.TargetFileName
									   : this.fileSystemService.GetFileName(path.SourcePath);
						});
				return result;
			}
			catch (ArgumentException e)
			{
				// Note: this exception is only thrown when ValidateSourcePaths is true.
				this.TransferLog.LogWarning(
					e,
					"There was a problem adding the '{SourceFile}' source file to the transfer job.",
					path.SourcePath);
				throw new FileNotFoundException(e.Message, path.SourcePath);
			}
			catch (FileNotFoundException e)
			{
				// Ensure this exception is accounted for.
				this.TransferLog.LogWarning(e, "The '{SourceFile}' source file doesn't exist.", path.SourcePath);
				throw;
			}
			catch (OperationCanceledException)
			{
				this.LogCancelRequest();
				return !string.IsNullOrEmpty(path.TargetFileName)
						   ? path.TargetFileName
						   : this.fileSystemService.GetFileName(path.SourcePath);
			}
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dump the transfer bridge parameter.
		/// </summary>
		public virtual void DumpInfo()
		{
			var importExportCoreVersion = this.GetType().Assembly.GetName().Version;
			var tapiVersion = typeof(ITransferClient).Assembly.GetName().Version;
			this.TransferLog.LogInformation("Import/Export Core - Version: {WinEDDSVersion}", importExportCoreVersion);
			this.TransferLog.LogInformation("TAPI - Version: {TapiVersion}", tapiVersion);
			this.TransferLog.LogInformation("Application: {Application}", this.parameters.Application);
			this.TransferLog.LogInformation("Client request id: {ClientRequestId}", this.parameters.ClientRequestId);
			this.TransferLog.LogInformation("Aspera doc root level: {AsperaDocRootLevels}", this.parameters.AsperaDocRootLevels);
			this.TransferLog.LogInformation("File share: {FileShare}", this.parameters.FileShare);
			this.TransferLog.LogInformation("Force Aspera client: {ForceAsperaClient}", this.parameters.ForceAsperaClient);
			this.TransferLog.LogInformation("Force Fileshare client: {ForceFileShareClient}", this.parameters.ForceFileShareClient);
			this.TransferLog.LogInformation("Force HTTP client: {ForceHttpClient}", this.parameters.ForceHttpClient);
			this.TransferLog.LogInformation("Force client candidates: {ForceClientCandidates}", this.parameters.ForceClientCandidates);
			this.TransferLog.LogInformation("HTTP timeout: {HttpTimeoutSeconds} seconds", this.parameters.TimeoutSeconds);
			this.TransferLog.LogInformation("Max job parallelism: {MaxJobParallelism}", this.parameters.MaxJobParallelism);
			this.TransferLog.LogInformation("Max job retry attempts: {MaxJobRetryAttempts}", this.parameters.MaxJobRetryAttempts);
			this.TransferLog.LogInformation("Min data rate: {MinDataRateMbps} Mbps", this.parameters.MinDataRateMbps);
			this.TransferLog.LogInformation("Preserve file timestamps: {PreserveFileTimestamps}", this.parameters.PreserveFileTimestamps);
			this.TransferLog.LogInformation("Retry on file permission error: {PermissionErrorsRetry}", this.parameters.PermissionErrorsRetry);
			this.TransferLog.LogInformation("Retry on bad path error: {BadPathErrorsRetry}", this.parameters.BadPathErrorsRetry);
			this.TransferLog.LogInformation("Submit APM metrics: {SubmitApmMetrics}", this.parameters.SubmitApmMetrics);
			this.TransferLog.LogInformation("Target data rate: {TargetDataRateMbps} Mbps", this.parameters.TargetDataRateMbps);
			this.TransferLog.LogInformation("Wait time between retry attempts: {WaitTimeBetweenRetryAttempts}", this.parameters.WaitTimeBetweenRetryAttempts);
			this.TransferLog.LogInformation("Workspace identifier: {WorkspaceId}", this.parameters.WorkspaceId);
		}

		/// <summary>
		/// Waits for the transfer job to complete all pending transfers in the queue.
		/// </summary>
		public void WaitForTransferJob()
		{
			this.CheckDispose();
			if (this.TransferJob == null)
			{
				throw new InvalidOperationException(Strings.TransferJobNullExceptionMessage);
			}

			try
			{
				const int RetryAttempts = 2;
				Exception handledException = null;
				Policy.Handle<TransferException>().Retry(
					RetryAttempts,
					(exception, count) =>
						{
							handledException = exception;
							this.TransferLog.LogWarning2(
								exception,
								this.jobRequest,
								Strings.CompleteJobExceptionMessage);
							this.FallbackHttpClient(exception, null);
						}).Execute(
					() =>
						{
							var taskResult = this.TransferJob.CompleteAsync(this.cancellationToken);
							var transferResult = taskResult.GetAwaiter().GetResult();
							this.TransferLog.LogInformation(
								"{Name} transfer status: {Status}, elapsed time: {Elapsed}, data rate: {TransferRate:0.00} Mbps",
								this.ClientDisplayName,
								transferResult.Status,
								transferResult.Elapsed,
								transferResult.TransferRateMbps);
							this.TransferLog.LogInformation(
								"{Name} total transferred files: {TotalTransferredFiles}, total failed files: {TotalFailedFiles}",
								this.ClientDisplayName,
								transferResult.TotalTransferredFiles,
								transferResult.TotalFailedFiles);
							switch (transferResult.Status)
							{
								case TransferStatus.Fatal:

									// Note: Fatal status is non-retryable and normally indicative of issues with data or permissions.
									const int ValidLineNumber = 1;
									var message = "This operation failed due to a fatal transfer error.";
									var lineNumber = ValidLineNumber;
									var lastIssue =
										transferResult.Issues.OrderBy(x => x.Index).ToList()
											.FindLast(x => x.Path != null) ?? transferResult.TransferError;
									if (lastIssue != null && lastIssue.Path != null)
									{
										var formattedMessage = this.TransferFileFatalMessage();
										message = string.Format(
											CultureInfo.CurrentCulture,
											formattedMessage,
											lastIssue.Message);
										lineNumber = lastIssue.Path.Order > 0 ? lastIssue.Path.Order : ValidLineNumber;
									}

									this.PublishStatusMessage(message, lineNumber);
									if (handledException == null)
									{
										// Force the fallback.
										throw new TransferException(Strings.TransferJobExceptionMessage);
									}

									// Gracefully terminate.
									this.PublishFatalError(message, lineNumber);
									break;

								case TransferStatus.Failed:

									// Note: Failed status indicates a problem with the transport.
									throw new TransferException(Strings.TransferJobExceptionMessage);
							}
						});
			}
			catch (OperationCanceledException)
			{
				this.LogCancelRequest();
			}
			finally
			{
				this.DestroyTransferJob();
			}
		}

		/// <summary>
		/// Creates client configuration.
		/// </summary>
		/// <returns>
		/// The <see cref="ClientConfiguration"/> instance.
		/// </returns>
		protected virtual ClientConfiguration CreateClientConfiguration()
		{
			// Due to FileNotFoundException expectations, do NOT enable this setting.
			const bool ValidateSourcePaths = false;

			// Intentionally limiting resiliency here; otherwise, status messages don't make it to IAPI.
			const int MaxHttpRetryAttempts = 1;
			var configuration = new ClientConfiguration
			{
				BadPathErrorsRetry = this.parameters.BadPathErrorsRetry,
				BcpRootFolder = this.parameters.AsperaBcpRootFolder,
				CookieContainer = this.parameters.WebCookieContainer,
				Credential =
											this.parameters.FileshareCredentials != null
												? this.parameters.FileshareCredentials.CreateCredential()
												: null,
				FileTransferHint = FileTransferHint.Natives,
				HttpTimeoutSeconds = this.parameters.TimeoutSeconds,
				MaxJobParallelism = this.parameters.MaxJobParallelism,
				MaxJobRetryAttempts = this.parameters.MaxJobRetryAttempts,
				MaxHttpRetryAttempts = MaxHttpRetryAttempts,
				MinDataRateMbps = this.parameters.MinDataRateMbps,
				PermissionErrorsRetry = this.parameters.PermissionErrorsRetry,

				// REL-298418: preserving file timestamps are now driven by a configurable setting.
				PreserveDates = this.parameters.PreserveFileTimestamps,
				SupportCheckPath = this.parameters.SupportCheckPath,
				TargetDataRateMbps = this.parameters.TargetDataRateMbps,
				TransferLogDirectory = this.parameters.TransferLogDirectory,
				ValidateSourcePaths = ValidateSourcePaths,
			};
			return configuration;
		}

		/// <summary>
		/// Creates transfer request for job.
		/// </summary>
		/// <param name="context">
		/// The transfer context.
		/// </param>
		/// <returns>
		/// The <see cref="TransferRequest"/> instance.
		/// </returns>
		protected abstract TransferRequest CreateTransferRequestForJob(TransferContext context);

		/// <summary>
		/// Setup the customer resolvers for both source and target paths.
		/// </summary>
		/// <param name="request">
		/// The transfer request.
		/// </param>
		/// <remarks>
		/// This provides backwards compatibility with IAPI.
		/// </remarks>
		protected abstract void SetupRemotePathResolvers(ITransferRequest request);

		/// <summary>
		/// Specifies the fatal error message.
		/// </summary>
		/// <returns>
		/// The error message.
		/// </returns>
		protected abstract string TransferFileFatalMessage();

		/// <summary>
		/// Checks to see whether this instance has been disposed.
		/// </summary>
		/// <exception cref="System.ObjectDisposedException">
		/// Thrown when this instance has been disposed.
		/// </exception>
		private void CheckDispose()
		{
			if (!this.disposed)
			{
				return;
			}

			throw new ObjectDisposedException(Strings.ObjectDisposedExceptionMessage);
		}

		/// <summary>
		/// Creates the best transfer client.
		/// </summary>
		private void CreateTransferClient()
		{
			this.CheckDispose();
			if (this.transferClient != null)
			{
				return;
			}

			var configuration = this.CreateClientConfiguration();

			try
			{
				var clientId = this.tapiObjectService.GetClientId(this.parameters);
				if (clientId != Guid.Empty)
				{
					configuration.ClientId = clientId;
					this.CreateTransferClient(configuration);
					this.PublishClientChanged(ClientChangeReason.ForceConfig);
				}
				else
				{
					configuration.ClientId = Guid.Empty;

					// The configuration parameters may want to change order or restrict certain clients.
					TransferClientStrategy clientStrategy;
					if (!string.IsNullOrEmpty(this.parameters.ForceClientCandidates))
					{
						clientStrategy = new TransferClientStrategy(this.parameters.ForceClientCandidates);
						this.TransferLog.LogInformation(
							"Override the default transfer client strategy. Candidates={ForceClientCandidates}",
							this.parameters.ForceClientCandidates);
					}
					else
					{
						clientStrategy = new TransferClientStrategy();
						this.TransferLog.LogInformation("Using the default default transfer client strategy.");
					}

					var transferHost = this.CreateTransferHost();
					this.transferClient = transferHost
						.CreateClientAsync(configuration, clientStrategy, this.cancellationToken)
						.GetAwaiter()
						.GetResult();
					this.TransferLog.LogInformation(
						"TAPI created the {Client} client via best-fit strategy.",
						this.transferClient.DisplayName);
					this.PublishClientChanged(ClientChangeReason.BestFit);
				}
			}
			catch (Exception e)
			{
				this.TransferLog.LogError(e, "The transfer client construction failed.");
				configuration.ClientId = new Guid(TransferClientConstants.HttpClientId);
				this.CreateTransferClient(configuration);
				this.PublishClientChanged(ClientChangeReason.HttpFallback);
			}
			finally
			{
				this.OptimizeClient();
			}
		}

		/// <summary>
		/// Creates the <see cref="RelativityTransferHost"/> instance if not already constructed.
		/// </summary>
		/// <returns>
		/// The <see cref="IRelativityTransferHost"/> instance.
		/// </returns>
		private IRelativityTransferHost CreateTransferHost()
		{
			// REL-281370: Lazy construct to avoid lengthy construction
			//             and exceptions getting thrown via constructor.
			if (this.relativityTransferHost == null)
			{
				var connectionInfo = this.tapiObjectService.CreateRelativityConnectionInfo(this.parameters);
				this.relativityTransferHost =
					this.tapiObjectService.CreateRelativityTransferHost(connectionInfo, this.TransferLog);
			}

			return this.relativityTransferHost;
		}

		/// <summary>
		/// Creates the client using only the specified configuration object.
		/// </summary>
		/// <param name="configuration">
		/// The transfer client configuration.
		/// </param>
		private void CreateTransferClient(ClientConfiguration configuration)
		{
			var transferHost = this.CreateTransferHost();
			transferHost.Clear();
			this.transferClient = transferHost.CreateClient(configuration);
			this.UpdateAllTransferListenersClientName();
		}

		/// <summary>
		/// Creates a new transfer job.
		/// </summary>
		/// <param name="httpFallback">
		/// Specify whether this method is setting up the HTTP fallback client.
		/// </param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "Never fail due to retrieving process info.")]
		private void CreateTransferJob(bool httpFallback)
		{
			this.CheckDispose();
			if (this.TransferJob != null)
			{
				return;
			}

			this.TransferLog.LogInformation("Create job started...");
			this.CreateTransferClient();
			this.currentJobNumber++;
			this.currentJobId = Guid.NewGuid();
			this.jobRequest = this.CreateTransferRequestForJob(this.transferContext);
			this.jobRequest.Application = this.parameters.Application;
			if (string.IsNullOrEmpty(this.jobRequest.Application))
			{
				try
				{
					var file = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
					this.jobRequest.Application = System.IO.Path.GetFileName(file);
				}
				catch (Exception)
				{
					this.jobRequest.Application = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
				}
			}

			this.jobRequest.ClientRequestId = this.parameters.ClientRequestId;
			this.jobRequest.JobId = this.currentJobId;
			this.jobRequest.Tag = this.currentJobNumber;

			// This will allow better tracking of transfers on the server.
			this.jobRequest.Name = $"RDC-{this.ClientId}-batch {this.currentJobNumber:0000}";

			// Note: avoid exponential backoff since that number will be excessive given the default max retry period.
			this.jobRequest.RetryStrategy =
				RetryStrategies.CreateFixedTimeStrategy(this.parameters.WaitTimeBetweenRetryAttempts);
			this.SetupRemotePathResolvers(this.jobRequest);

			// Submitting APM metrics is an opt-in feature.
			this.jobRequest.SubmitApmMetrics = this.parameters.SubmitApmMetrics;

			try
			{
				var task = this.transferClient.CreateJobAsync(this.jobRequest, this.cancellationToken);
				this.TransferJob = task.GetAwaiter().GetResult();
				this.TransferLog.LogInformation("Create job ended.");
			}
			catch (OperationCanceledException)
			{
				this.LogCancelRequest();
				this.DestroyTransferJob();
			}
			catch (Exception e)
			{
				this.TransferLog.LogError(e, "Failed to create the transfer job.");
				if (httpFallback)
				{
					// Nothing more can be done.
					throw;
				}

				this.FallbackHttpClient(e, null);
			}
		}

		/// <summary>
		/// Creates the HTTP client.
		/// </summary>
		private void CreateHttpClient()
		{
			this.CreateTransferClient(
				new HttpClientConfiguration { CookieContainer = this.parameters.WebCookieContainer });
		}

		/// <summary>
		/// Creates and initializes a <inheritdoc cref="TapiJobRetryListener"/> to listen for job retry events.
		/// </summary>
		private void CreateJobRetryListener()
		{
			this.transferListeners.Add(
				new TapiJobRetryListener(this.TransferLog, this.parameters.MaxJobRetryAttempts, this.transferContext));
		}

		/// <summary>
		/// Creates and initializes a <inheritdoc cref="TapiPathIssueListener"/> instance.
		/// </summary>
		private void CreatePathIssueListener()
		{
			this.transferListeners.Add(
				new TapiPathIssueListener(
					this.TransferLog,
					this.currentDirection,
					this.transferContext));
		}

		/// <summary>
		/// Creates initializes a <inheritdoc cref="TapiPathProgressListener"/> instance.
		/// </summary>
		private void CreatePathProgressListener()
		{
			var listener = new TapiPathProgressListener(this.TransferLog, this.transferContext);
			listener.ProgressEvent += (sender, args) =>
				{
					this.TapiProgress?.Invoke(sender, args);
				};
			this.transferListeners.Add(listener);
		}

		/// <summary>
		/// Creates and initializes a <inheritdoc cref="TapiRequestListener"/> to listen for transfer request events.
		/// </summary>
		private void CreateRequestListener()
		{
			this.transferListeners.Add(new TapiRequestListener(this.TransferLog, this.transferContext));
		}

		/// <summary>
		/// Creates and initializes a <inheritdoc cref="TapiStatisticsListener"/> to listen for transfer statistic events.
		/// </summary>
		private void CreateStatisticsListener()
		{
			var listener = new TapiStatisticsListener(this.TransferLog, this.transferContext);
			listener.StatisticsEvent += (sender, args) => this.TapiStatistics?.Invoke(sender, args);
			this.transferListeners.Add(listener);
		}

		/// <summary>
		/// Destroys the transfer client.
		/// </summary>
		private void DestroyTransferClient()
		{
			if (this.transferClient == null)
			{
				return;
			}

			this.transferClient.Dispose();
			this.transferClient = null;
			this.UpdateAllTransferListenersClientName();
		}

		/// <summary>
		/// Destroys the transfer host.
		/// </summary>
		private void DestroyTransferHost()
		{
			// Note: this is the only method that should directly reference the field.
			if (this.relativityTransferHost == null)
			{
				return;
			}

			this.relativityTransferHost.Dispose();
			this.relativityTransferHost = null;
		}

		/// <summary>
		/// Destroys the transfer job.
		/// </summary>
		private void DestroyTransferJob()
		{
			this.currentJobId = null;
			if (this.TransferJob == null)
			{
				return;
			}

			this.TransferJob.Dispose();
			this.TransferJob = null;
		}

		/// <summary>
		/// Destroys transfer listeners.
		/// </summary>
		private void DestroyTransferListeners()
		{
			if (this.transferListeners == null)
			{
				return;
			}

			foreach (TapiListenerBase listener in this.transferListeners)
			{
				listener.Dispose();
			}

			this.transferListeners.Clear();
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing">
		/// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
		/// </param>
		private void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}

			if (disposing)
			{
				this.DestroyTransferJob();
				this.DestroyTransferClient();
				this.DestroyTransferHost();
				this.DestroyTransferListeners();
			}

			this.disposed = true;
		}

		/// <summary>
		/// Setup a new HTTP client and potentially re-queue all paths that previously failed.
		/// </summary>
		/// <param name="exception">
		/// The exception that forced the fallback.
		/// </param>
		/// <param name="addedPath">
		/// The path attempting to get added to the job. This can be null if the failure occurred outside of adding the path to a job.
		/// </param>
		private void FallbackHttpClient(Exception exception, TransferPath addedPath)
		{
			// If we're already using the HTTP client, it's hopeless.
			if (this.transferClient.Id == new Guid(TransferClientConstants.HttpClientId))
			{
				throw new TransferException(Strings.HttpFallbackExceptionMessage, exception);
			}

			this.TransferLog.LogInformation(exception, "Preparing to fallback to the HTTP client due to an unexpected error.");

			// Ensure the fallback mode is acknowledged via Warning message.
			var message = string.Format(
				CultureInfo.CurrentCulture,
				Strings.HttpFallbackWarningMessage,
				this.ClientDisplayName,
				exception.Message);
			this.PublishWarningMessage(message, TapiConstants.NoLineNumber);
			var retryablePaths = this.GetRetryableTransferPaths().ToList();
			if (addedPath != null && !retryablePaths.Any(x => x.Equals(addedPath)))
			{
				// Do NOT call AddPath as this could introduce infinite recursion.
				retryablePaths.Add(addedPath);
			}

			this.DestroyTransferJob();
			this.DestroyTransferClient();
			this.CreateHttpClient();
			this.PublishClientChanged(ClientChangeReason.HttpFallback);
			this.CreateTransferJob(true);

			// Restore the original path before adding to the HTTP-based job.
			foreach (var path in retryablePaths)
			{
				path.RevertPaths();
				this.TransferJob.AddPath(path);
			}

			this.TransferLog.LogInformation(exception, "Successfully switched the transfer client to HTTP.");
		}

		/// <summary>
		/// Gets a value indicating whether the specified path has already been added to the queue.
		/// </summary>
		/// <param name="path">
		/// The path.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the path has already been added to the queue; otherwise, <see langword="false" />.
		/// </returns>
		private bool GetIsTransferPathInJobQueue(TransferPath path)
		{
			var queuedTransferPaths = this.TransferJob.JobService.GetJobTransferPaths().Select(jobPath => jobPath.Path);
			return queuedTransferPaths.Any(x => x.Equals(path));
		}

		/// <summary>
		/// Gets a collection of transfer paths that are in the queue and not yet transferred.
		/// </summary>
		/// <returns>
		/// The <see cref="TransferPath"/> instance.
		/// </returns>
		private IEnumerable<TransferPath> GetRetryableTransferPaths()
		{
			var paths = new List<TransferPath>();
			if (this.TransferJob != null)
			{
				paths.AddRange(
					this.TransferJob.JobService.GetJobTransferPaths().Where(x => x.Status != TransferPathStatus.Successful)
						.Select(jobPath => jobPath.Path));
			}

			return paths;
		}

		/// <summary>
		/// Logs a cancellation request.
		/// </summary>
		private void LogCancelRequest()
		{
			this.TransferLog.LogInformation(
				"The file transfer has been cancelled. ClientId={ClientId}, JobId={JobId} ",
				this.parameters.ClientRequestId,
				this.currentJobId);
		}

		/// <summary>
		/// Apply optimizations to the client.
		/// </summary>
		private void OptimizeClient()
		{
			if (this.transferClient == null)
			{
				return;
			}

			// Tune the job retry to account for different kinds of jobs.
			switch (this.transferClient.Id.ToString().ToUpperInvariant())
			{
				case TransferClientConstants.FileShareClientId:
					break;

				case TransferClientConstants.HttpClientId:
					this.transferClient.Configuration.MaxJobParallelism = 1;
					break;

				case TransferClientConstants.AsperaClientId:
					this.transferClient.Configuration.MaxJobParallelism = 1;
					break;
			}
		}

		/// <summary>
		/// Publish a client changed event.
		/// </summary>
		/// <param name="reason">
		/// The reason for the client change.
		/// </param>
		private void PublishClientChanged(ClientChangeReason reason)
		{
			this.CheckDispose();
			string message;
			switch (reason)
			{
				case ClientChangeReason.BestFit:
					message = string.Format(
						CultureInfo.CurrentCulture,
						Strings.TransferClientChangedBestFitMessage,
						this.ClientDisplayName);
					break;

				case ClientChangeReason.ForceConfig:
					message = string.Format(
						CultureInfo.CurrentCulture,
						Strings.TransferClientChangedForceConfigMessage,
						this.ClientDisplayName);
					break;

				case ClientChangeReason.HttpFallback:
					message = string.Format(
						CultureInfo.CurrentCulture,
						Strings.TransferClientChangedHttpFallbackMessage,
						this.ClientDisplayName);
					break;

				default:
					message = string.Format(
						CultureInfo.CurrentCulture,
						Strings.TransferClientChangedDefaultMessage,
						this.ClientDisplayName);
					break;
			}

			this.PublishStatusMessage(message, TapiConstants.NoLineNumber);
			var eventArgs = new TapiClientEventArgs(this.ClientDisplayName, this.Client);
			this.TapiClientChanged?.Invoke(this, eventArgs);
			this.UpdateAllTransferListenersClientName();
		}

		/// <summary>
		/// Publish a fatal error.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="lineNumber">
		/// The line number.
		/// </param>
		private void PublishFatalError(string message, int lineNumber)
		{
			this.CheckDispose();
			this.TapiFatalError?.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
		}

		/// <summary>
		/// Publish a status message event.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="lineNumber">
		/// The line number.
		/// </param>
		private void PublishStatusMessage(string message, int lineNumber)
		{
			this.CheckDispose();
			this.TapiStatusMessage?.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
		}

		/// <summary>
		/// Publish a warning message event.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <param name="lineNumber">
		/// The line number.
		/// </param>
		private void PublishWarningMessage(string message, int lineNumber)
		{
			this.CheckDispose();
			this.TapiWarningMessage?.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
		}

		/// <summary>
		/// Setup the transfer listeners.
		/// </summary>
		private void SetupTransferListeners()
		{
			this.CreatePathProgressListener();
			this.CreatePathIssueListener();
			this.CreateRequestListener();
			this.CreateJobRetryListener();
			this.CreateStatisticsListener();
			foreach (var listener in this.transferListeners)
			{
				listener.ErrorMessage += (sender, args) =>
					{
						this.TapiErrorMessage?.Invoke(sender, args);
					};
				listener.FatalError += (sender, args) =>
					{
						this.TapiFatalError?.Invoke(sender, args);
					};
				listener.StatusMessage += (sender, args) =>
					{
						this.TapiStatusMessage?.Invoke(sender, args);
					};
				listener.WarningMessage += (sender, args) =>
					{
						this.TapiWarningMessage?.Invoke(sender, args);
					};
			}
		}

		/// <summary>
		/// Updates the client display name found on all listeners.
		/// </summary>
		private void UpdateAllTransferListenersClientName()
		{
			if (this.disposed)
			{
				return;
			}

			string name = this.ClientDisplayName;
			foreach (var listener in this.transferListeners)
			{
				listener.ClientDisplayName = name;
			}
		}
	}
}