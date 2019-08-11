// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiBridgeBase2.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to provide a bridge from the Transfer API to existing Import/Export code.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Threading;

	using Polly;

	using Relativity.DataExchange.Resources;
	using Relativity.Transfer;
	using Relativity.Transfer.Http;

	/// <summary>
	/// Represents a class object to provide a bridge from the Transfer API to existing Import/Export code.
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public abstract class TapiBridgeBase2 : ITapiBridge
	{
		/// <summary>
		/// The thread synchronization root.
		/// </summary>
		private readonly object syncRoot = new object();

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
		private readonly TapiBridgeParameters2 parameters;

		/// <summary>
		/// The list of transfer event listeners.
		/// </summary>
		private readonly List<TapiListenerBase> transferListeners = new List<TapiListenerBase>();

		/// <summary>
		/// The current transfer direction.
		/// </summary>
		private readonly TransferDirection currentDirection;

		/// <summary>
		/// The batch totals.
		/// </summary>
		private readonly TapiTotals jobTotals = new TapiTotals();

		/// <summary>
		/// The batch totals.
		/// </summary>
		private readonly TapiTotals batchTotals = new TapiTotals();

		/// <summary>
		/// The job unique identifier associated with the current job.
		/// </summary>
		private Guid? currentJobId;

		/// <summary>
		/// The current job number. This is tagged to the request to provide a non-zero line number.
		/// </summary>
		private int currentJobNumber;

		/// <summary>
		/// The maximum number of seconds to wait for inactive data.
		/// </summary>
		private double maxInactivitySeconds;

		/// <summary>
		/// The current job request.
		/// </summary>
		private ITransferRequest jobRequest;

		/// <summary>
		/// The flag that indicates whether a transfer permission issue has occurred.
		/// </summary>
		private bool raisedPermissionIssue;

		/// <summary>
		/// The transfer permission issue message.
		/// </summary>
		private string raisedPermissionIssueMessage;

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
		/// The current transfer job.
		/// </summary>
		private ITransferJob transferJob;

		/// <summary>
		/// The timestamp that tracks how long since the last movement of data.
		/// </summary>
		private DateTime transferActivityTimestamp;

		/// <summary>
		/// The disposed backing.
		/// </summary>
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiBridgeBase2"/> class.
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
		internal TapiBridgeBase2(
			ITapiObjectService service,
			TapiBridgeParameters2 parameters,
			TransferDirection direction,
			ITransferLog log,
			CancellationToken token)
			: this(service, parameters, direction, CreateDefaultTransferContext(parameters), log, token)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiBridgeBase2"/> class.
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
		/// <param name="context">
		/// The transfer context.
		/// </param>
		/// <param name="log">
		/// The transfer log.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		internal TapiBridgeBase2(
			ITapiObjectService service,
			TapiBridgeParameters2 parameters,
			TransferDirection direction,
			TransferContext context,
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

			if (context == null)
			{
				context = CreateDefaultTransferContext(parameters);
			}

			this.InstanceId = Guid.NewGuid();
			this.tapiObjectService = service;
			this.currentDirection = direction;
			this.parameters = parameters;
			this.TargetPath = parameters.TargetPath;
			this.cancellationToken = token;
			this.TransferLog = log;
			this.currentJobNumber = 0;
			this.transferContext = context;
			this.SetupTransferListeners();
			this.UpdateAllTransferListenersClientName();
		}

		/// <inheritdoc />
		public event EventHandler<TapiMessageEventArgs> TapiStatusMessage;

		/// <inheritdoc />
		public event EventHandler<TapiMessageEventArgs> TapiErrorMessage;

		/// <inheritdoc />
		public event EventHandler<TapiMessageEventArgs> TapiWarningMessage;

		/// <inheritdoc />
		public event EventHandler<TapiClientEventArgs> TapiClientChanged;

		/// <inheritdoc />
		public event EventHandler<TapiProgressEventArgs> TapiProgress;

		/// <inheritdoc />
		public event EventHandler<TapiLargeFileProgressEventArgs> TapiLargeFileProgress;

		/// <inheritdoc />
		public event EventHandler<TapiStatisticsEventArgs> TapiStatistics;

		/// <inheritdoc />
		public event EventHandler<TapiMessageEventArgs> TapiFatalError;

		/// <inheritdoc />
		public TapiClient Client => this.tapiObjectService.GetTapiClient(this.ClientId);

		/// <summary>
		/// Gets the current transfer client unique identifier.
		/// </summary>
		/// <value>
		/// The <see cref="Guid"/> value.
		/// </value>
		public Guid ClientId => this.transferClient?.Id ?? Guid.Empty;

		/// <inheritdoc />
		public Guid InstanceId
		{
			get;
		}

		/// <inheritdoc />
		public TapiTotals JobTotals => this.jobTotals;

		/// <inheritdoc />
		public TapiBridgeParameters2 Parameters => this.parameters;

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
		/// Gets a value indicating whether there are transfers pending.
		/// </summary>
		/// <remarks>
		/// The RequestTransferPathCount property was added to avoid costly hits to the repository.
		/// </remarks>
		public bool TransfersPending => this.TransferJob != null
		                                && this.TransferJob.JobService.RequestTransferPathCount > 0;

		/// <summary>
		/// Gets the workspace artifact unique identifier.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		public int WorkspaceId => this.parameters.WorkspaceId;

		/// <summary>
		/// Gets a value indicating whether any permission related transfer issue has been raised.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> if any permission related transfer issue has been raised; otherwise, <see langword="false" />.
		/// </returns>
		protected bool RaisedPermissionIssue
		{
			get
			{
				lock (this.syncRoot)
				{
					return this.raisedPermissionIssue;
				}
			}

			private set
			{
				lock (this.syncRoot)
				{
					this.raisedPermissionIssue = value;
				}
			}
		}

		/// <summary>
		/// Gets the permission related transfer issue message.
		/// </summary>
		/// <returns>
		/// The message.
		/// </returns>
		protected string RaisedPermissionIssueMessage
		{
			get
			{
				lock (this.syncRoot)
				{
					return this.raisedPermissionIssueMessage;
				}
			}

			private set
			{
				lock (this.syncRoot)
				{
					this.raisedPermissionIssueMessage = value;
				}
			}
		}

		/// <summary>
		/// Gets the current transfer job.
		/// </summary>
		/// <value>
		/// The <see cref="ITransferJob"/> instance.
		/// </value>
		protected ITransferJob TransferJob
		{
			get
			{
				lock (this.syncRoot)
				{
					return this.transferJob;
				}
			}

			private set
			{
				lock (this.syncRoot)
				{
					this.transferJob = value;
				}
			}
		}

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

		/// <inheritdoc />
		public string AddPath(TransferPath path)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}

			this.ValidateTransferPath(path);
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
							transferException = exception;
							if (this.TransferJob?.JobService.Statistics != null && this.TransferJob.JobService.Statistics.JobError)
							{
								this.TransferLog.LogError(
									exception,
									"Failed to add a path to the transfer job due to a job-level error. Job error: {JobErrorMessage}",
									this.TransferJob.JobService.Statistics.JobErrorMessage);
							}
							else
							{
								this.TransferLog.LogError(
									exception,
									"Failed to add a path to the transfer job.");
							}

							// Note: if the switch is successful, the path will get added below.
							this.SwitchToWebMode(exception);
						}).Execute(
						() =>
						{
							// Fallback automatically attempts to add paths. Make sure the path isn't added twice.
							if (transferException == null || !this.GetIsTransferPathInJobQueue(path))
							{
								try
								{
									this.TransferJob.AddPath(path, this.cancellationToken);
									this.IncrementTotalFileTransferRequests();
								}
								catch
								{
									// This handles the edge case where an exception may be thrown but the object was actually added to the queue.
									if (this.GetIsTransferPathInJobQueue(path))
									{
										this.IncrementTotalFileTransferRequests();
									}

									throw;
								}
							}

							return !string.IsNullOrEmpty(path.TargetFileName)
									   ? path.TargetFileName
									   : Relativity.DataExchange.Io.FileSystem.Instance.Path.GetFileName(path.SourcePath);
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
				// TODO: I believe that NOT throwing the exception was intentional because cancellation support didn't exist in 2017.
				// TODO: Need to revisit this and see if this can safely rethrow.
				this.LogCancelRequest();
				return !string.IsNullOrEmpty(path.TargetFileName)
					       ? path.TargetFileName
					       : Relativity.DataExchange.Io.FileSystem.Instance.Path.GetFileName(path.SourcePath);
			}
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Logs all transfer bridge parameters.
		/// </summary>
		public virtual void LogTransferParameters()
		{
			var importExportCoreVersion = this.GetType().Assembly.GetName().Version;
			var tapiVersion = typeof(ITransferClient).Assembly.GetName().Version;
			this.TransferLog.LogInformation("Import/Export Core - Version: {ImportExportCoreVersion}", importExportCoreVersion);
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
			this.TransferLog.LogInformation("Max inactivity seconds: {MaxInactivitySeconds}", this.parameters.MaxInactivitySeconds);
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

		/// <inheritdoc />
		public TapiTotals WaitForTransfers(
			string waitMessage,
			string successMessage,
			string errorMessage,
			bool keepJobAlive)
		{
			this.CheckDispose();
			this.PublishStatusMessage(waitMessage, TapiConstants.NoLineNumber);

			try
			{
				// Some jobs may be small and contain invalid data that prevent adding to the job - don't throw unnecessarily.
				TapiTotals totals;
				this.LogTransferTotals("Pre", false, keepJobAlive, keepJobAlive ? this.batchTotals : this.jobTotals);
				if (this.TransferJob == null || (this.batchTotals.TotalFileTransferRequests == 0
				                                 && this.jobTotals.TotalFileTransferRequests == 0))
				{
					totals = keepJobAlive ? this.batchTotals.DeepCopy() : this.jobTotals.DeepCopy();
				}
				else
				{
					totals = keepJobAlive ? this.WaitForCompletedTransfers() : this.WaitForCompletedTransferJob();
				}

				this.LogTransferTotals("Post", true, keepJobAlive, totals);
				this.PublishStatusMessage(successMessage, TapiConstants.NoLineNumber);
				return totals;
			}
			catch (Exception e)
			{
				// Note: for backwards compatibility purposes, don't publish an error message.
				this.PublishWarningMessage(errorMessage, TapiConstants.NoLineNumber);
				this.TransferLog.LogError(e, errorMessage);
				throw;
			}
		}

		/// <summary>
		/// Clears both batch and job totals.
		/// </summary>
		/// <remarks>
		/// This is marked internal for unit tests only.
		/// </remarks>
		internal void ClearAllTotals()
		{
			this.batchTotals.Clear();
			this.jobTotals.Clear();
		}

		/// <summary>
		/// Creates the default transfer context object.
		/// </summary>
		/// <param name="parameters">
		/// The transfer bridge parameters.
		/// </param>
		/// <returns>
		/// The <see cref="TransferContext"/> instance.
		/// </returns>
		protected static TransferContext CreateDefaultTransferContext(TapiBridgeParameters2 parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			// Note: large file progress is always enabled.
			return new TransferContext
				       {
					       StatisticsRateSeconds = 1.0,
					       LargeFileProgressEnabled = true,
					       LargeFileProgressRateSeconds = 1.0,
				       };
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
					                    Credential = this.parameters.TransferCredential,
					                    FileTransferHint = FileTransferHint.Natives,
					                    FileNotFoundErrorsDisabled = this.parameters.FileNotFoundErrorsDisabled,
					                    FileNotFoundErrorsRetry = this.parameters.FileNotFoundErrorsRetry,
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
		/// Retrieves the overall transfer error message.
		/// </summary>
		/// <param name="result">
		/// The transfer result.
		/// </param>
		/// <returns>
		/// The error message.
		/// </returns>
		private static string GetTransferErrorMessage(ITransferResult result)
		{
			// Note: the TransferError should always provide the best error message.
			string message = string.Empty;
			if (result.TransferError != null)
			{
				message = result.TransferError.Message;
			}

			if (string.IsNullOrEmpty(message) && result.Issues.Count > 0)
			{
				// Just in case - consider the last raised issue.
				List<ITransferIssue> issues = result.Issues.OrderBy(x => x.Index).ToList();
				ITransferIssue lastIssue = issues.FindLast(x => x.Path != null) ?? issues.LastOrDefault();
				if (lastIssue != null)
				{
					message = lastIssue.Message;
				}
			}

			if (string.IsNullOrEmpty(message))
			{
				// When all else fails.
				message = Strings.TransferJobExceptionMessage;
			}

			return message;
		}

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
		/// Perform a check to determine whether all transfers have completed.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> when all transfers have completed; otherwise, <see langword="false" />.
		/// </returns>
		private bool CheckCompletedTransfers()
		{
			bool completed = this.batchTotals.TotalFileTransferRequests == this.batchTotals.TotalCompletedFileTransfers;
			if (completed)
			{
				this.TransferLog.LogInformation2(this.jobRequest, "Successfully waited for all transfers to complete.");
			}

			return completed;
		}

		/// <summary>
		/// Perform a check to determine whether the data inactivity time has been exceeded.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> when the time has been exceeded; otherwise, <see langword="false" />.
		/// </returns>
		private bool CheckDataInactivityTimeExceeded()
		{
			lock (this.syncRoot)
			{
				// Note: the timestamp is updated when ANY movement of data occurs.
				DateTime lastTransferActivityTimestamp = this.transferActivityTimestamp;
				bool exceeded = (DateTime.Now - lastTransferActivityTimestamp).TotalSeconds > this.maxInactivitySeconds;
				if (exceeded)
				{
					this.TransferLog.LogWarning2(
						this.jobRequest,
						"Exceeded the max inactivity time of {MaxInactivitySeconds} seconds since the previous {LastTransferActivityTimestamp} timestamp update.",
						this.maxInactivitySeconds,
						lastTransferActivityTimestamp);
				}

				return exceeded;
			}
		}

		/// <summary>
		/// Perform a check to determine whether the batched transfer job should abort when permission issues are raised.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> to abort the batched transfer job; otherwise, <see langword="false" />.
		/// </returns>
		private bool CheckAbortOnRaisedPermissionIssues()
		{
			if (this.TransferJob == null)
			{
				return false;
			}

			// Note: the PermissionErrorsRetry instructs Transfer API whether to internally retry this issue.
			bool result = this.RaisedPermissionIssue && !this.parameters.PermissionErrorsRetry;
			if (result)
			{
				this.TransferLog.LogWarning2(
					this.jobRequest,
					"The transfer job will abort because a file permission issue was raised. {Error}",
					this.RaisedPermissionIssueMessage);
			}

			return result;
		}

		/// <summary>
		/// Perform a check to determine whether the transfer job status is valid.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> when the transfer job is valid; otherwise, <see langword="false" />.
		/// </returns>
		private bool CheckValidTransferJobStatus()
		{
			if (this.TransferJob == null)
			{
				return false;
			}

			TransferJobStatus status = this.TransferJob.Status;
			bool result = status == TransferJobStatus.RetryPending || status == TransferJobStatus.Retrying
			                                                       || status == TransferJobStatus.Running;
			if (!result)
			{
				this.TransferLog.LogWarning2(
					this.jobRequest,
					"The transfer job status {TransferJobStatus} is neither running or retrying and is considered invalid.",
					status);
			}

			return result;
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

			// TODO: eliminate this method and related code after Patch 1 to address cache coherency issues.
			ClientConfiguration configuration = this.CreateClientConfiguration();
			this.parameters.FileNotFoundErrorsDisabled = configuration.FileNotFoundErrorsDisabled;
			this.parameters.FileNotFoundErrorsRetry = configuration.FileNotFoundErrorsRetry;
			this.parameters.PermissionErrorsRetry = configuration.PermissionErrorsRetry;

			// Note: allow zero for improved testability.
			this.maxInactivitySeconds = this.parameters.MaxInactivitySeconds;
			if (this.maxInactivitySeconds < 0)
			{
				this.maxInactivitySeconds = 1.25 * (this.parameters.WaitTimeBetweenRetryAttempts
				                                    * (this.parameters.MaxJobRetryAttempts + 1));
			}

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
		/// <param name="webModeSwitch">
		/// Specify whether the job is created when switching to web mode.
		/// </param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "Never fail due to retrieving process info.")]
		private void CreateTransferJob(bool webModeSwitch)
		{
			this.CheckDispose();
			if (this.TransferJob != null)
			{
				return;
			}

			this.TransferLog.LogInformation("Create job started...");
			this.CreateTransferClient();
			lock (this.syncRoot)
			{
				this.transferActivityTimestamp = DateTime.Now;
			}

			// Never reset the counts when switching to web mode.
			if (!webModeSwitch)
			{
				this.ClearAllTotals();
			}

			this.currentJobNumber++;
			this.currentJobId = Guid.NewGuid();
			this.RaisedPermissionIssue = false;
			this.RaisedPermissionIssueMessage = null;
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
				throw;
			}
			catch (Exception e)
			{
				this.TransferLog.LogError(e, "Failed to create the transfer job.");
				if (webModeSwitch)
				{
					// Nothing more can be done.
					throw;
				}

				this.SwitchToWebMode(e);
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
				new TapiPathIssueListener(this.TransferLog, this.currentDirection, this.transferContext));
			this.transferContext.TransferPathIssue += this.OnTransferPathIssue;
		}

		/// <summary>
		/// Creates initializes a <inheritdoc cref="TapiPathProgressListener"/> instance.
		/// </summary>
		private void CreatePathProgressListener()
		{
			var listener = new TapiPathProgressListener(this.TransferLog, this.transferContext);
			listener.ProgressEvent += (sender, args) =>
				{
					if (args.Completed)
					{
						this.batchTotals.IncrementTotalCompletedFileTransfers();
						this.jobTotals.IncrementTotalCompletedFileTransfers();
					}

					if (args.Successful)
					{
						this.batchTotals.IncrementTotalSuccessfulFileTransfers();
						this.jobTotals.IncrementTotalSuccessfulFileTransfers();
					}

					this.UpdateTransferActivityTimestamp();
					this.TapiProgress?.Invoke(sender, args);
				};

			listener.LargeFileProgressEvent += (sender, args) =>
				{
					this.UpdateTransferActivityTimestamp();
					this.TapiLargeFileProgress?.Invoke(sender, args);
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
			this.transferContext.TransferPathIssue -= this.OnTransferPathIssue;
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
		/// Switch to web mode and re-queue all paths that previously failed.
		/// </summary>
		/// <param name="exception">
		/// The optional exception that forced the switch.
		/// </param>
		/// <exception cref="TransferException">
		/// Thrown when the existing job failed due to permissions or switching itself failed.
		/// </exception>
		private void SwitchToWebMode(Exception exception)
		{
			// Note: permission issues are fatal and cannot be "fixed" by switching to web mode.
			//       this check will prevent unnecessary spinning and force an immediate job failure.
			const bool Fatal = true;
			if (this.RaisedPermissionIssue)
			{
				this.DestroyTransferJob();
				string permissionMessage = string.Format(
					CultureInfo.CurrentCulture,
					Strings.WebModeFallbackPermissionsFatalExceptionMessage,
					this.RaisedPermissionIssueMessage);
				throw new TransferException(permissionMessage, exception, Fatal);
			}

			if (this.transferClient?.Id == new Guid(TransferClientConstants.HttpClientId))
			{
				this.DestroyTransferJob();
				throw new TransferException(Strings.WebModeFallbackAlreadyWebModeFatalExceptionMessage, exception, Fatal);
			}

			this.TransferLog.LogWarning("Preparing to fallback to web mode.");

			// Ensure the fallback mode is acknowledged via Warning message.
			var message = string.Format(
				CultureInfo.CurrentCulture,
				Strings.WebModeFallbackNoErrorWarningMessage,
				this.ClientDisplayName);
			if (exception != null)
			{
				message = string.Format(
					CultureInfo.CurrentCulture,
					Strings.WebModeFallbackWarningMessage,
					this.ClientDisplayName,
					exception.Message);
			}

			this.PublishWarningMessage(message, TapiConstants.NoLineNumber);
			var retryablePaths = this.GetRetryableTransferPaths();
			if (retryablePaths.Count == 0)
			{
				this.TransferLog.LogInformation("The current transfer job is switching to web mode and no retryable paths exist.");
			}

			this.DestroyTransferJob();
			this.DestroyTransferClient();
			this.CreateHttpClient();
			this.PublishClientChanged(ClientChangeReason.HttpFallback);
			this.CreateTransferJob(true);
			foreach (TransferPath path in retryablePaths)
			{
				// Restore the original path before adding to the web mode job.
				path.RevertPaths();

				// Do NOT call AddPath since this would introduce infinite recursion.
				// Do NOT call IncrementTotalFileTransferRequests since these have already been counted!
				this.TransferJob.AddPath(path, this.cancellationToken);
			}

			this.TransferLog.LogInformation("Successfully switched to web mode.");
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
			if (this.TransferJob == null)
			{
				return false;
			}

			// Note: this is the most efficient way to determine whether this object has been added to the queue!
			bool exists = this.TransferJob.JobService.GetJobTransferPath(path) != null;
			return exists;
		}

		/// <summary>
		/// Gets a collection of transfer paths that are in the queue and not yet transferred.
		/// </summary>
		/// <returns>
		/// The <see cref="TransferPath"/> instances.
		/// </returns>
		private IList<TransferPath> GetRetryableTransferPaths()
		{
			var paths = new List<TransferPath>();
			if (this.TransferJob != null)
			{
				paths.AddRange(this.TransferJob.JobService.GetRetryableRequestTransferPaths());

				// Note: potentially adding fatal error because the job service doesn't consider it retryable.
				paths.AddRange(
					this.TransferJob.JobService.GetJobTransferPaths()
						.Where(x => x.Status == TransferPathStatus.Fatal && !paths.Contains(x.Path))
						.Select(jobPath => jobPath.Path));
			}

			this.TransferLog.LogInformation("Total number of retryable paths: {TotalRetryablePaths:n0}", paths.Count);
			this.TransferLog.LogInformation("Total number of retryable bytes: {TotalRetryableBytes:n0}", paths.Sum(x => x.Bytes));
			return paths;
		}

		/// <summary>
		/// Increment the transfer request total.
		/// </summary>
		private void IncrementTotalFileTransferRequests()
		{
			this.batchTotals.IncrementTotalFileTransferRequests();
			this.jobTotals.IncrementTotalFileTransferRequests();
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
		/// Logs the supplied transfer totals.
		/// </summary>
		/// <param name="prefix">
		/// The text inserted before the log entry.
		/// </param>
		/// <param name="completed">
		/// <see langword="true" /> when all transfers should be completed; otherwise, <see langword="false" />.
		/// </param>
		/// <param name="batched">
		/// <see langword="true" /> to log batched totals; otherwise, <see langword="false" />.
		/// </param>
		/// <param name="totals">
		/// The totals to log.
		/// </param>
		private void LogTransferTotals(string prefix, bool completed, bool batched, TapiTotals totals)
		{
			string formattedPrefix = $"WaitForTransfers-{prefix}: ";
			StringBuilder sb = new StringBuilder(formattedPrefix);
			if (!completed)
			{
				sb.Append(
					batched
						? "Awaiting {TotalFileTransferRequests:n0} batched transfer files using {TransferMode} mode."
						: "Awaiting {TotalFileTransferRequests:n0} job transfer files using {TransferMode} mode.");
				this.TransferLog.LogInformation(
					sb.ToString(),
					totals.TotalFileTransferRequests,
					this.ClientDisplayName);
			}
			else
			{
				sb.Append(
					batched
						? "Completed {TotalSuccessfulFileTransfers:n0} of {TotalFileTransferRequests:n0} batched transfer files using {TransferMode} mode."
						: "Completed {TotalSuccessfulFileTransfers:n0} of {TotalFileTransferRequests:n0} job transfer files using {TransferMode} mode.");
				this.TransferLog.LogInformation(
					sb.ToString(),
					totals.TotalSuccessfulFileTransfers,
					totals.TotalFileTransferRequests,
					this.ClientDisplayName);
				if (totals.TotalFileTransferRequests == 0)
				{
					this.TransferLog.LogWarning(
						batched
							? formattedPrefix
							  + "Although the batch completed, the total number of batched file requests is zero and may suggest a logic issue or unexpected result."
							: formattedPrefix
							  + "Although the job completed, the total number of job file requests is zero and may suggest a logic issue or unexpected result.");
				}
			}
		}

		/// <summary>
		/// Handles the <see cref="E:TransferContextOnTransferPathIssue" /> event.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The <see cref="TransferPathIssueEventArgs"/> instance containing the event data.
		/// </param>
		private void OnTransferPathIssue(object sender, TransferPathIssueEventArgs e)
		{
			if (e.Issue.Attributes.HasFlag(IssueAttributes.ReadWritePermissions))
			{
				this.RaisedPermissionIssue = true;
				this.RaisedPermissionIssueMessage = e.Issue.Message;
			}
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

		/// <summary>
		/// Updates the transfer activity with a new timestamp.
		/// </summary>
		private void UpdateTransferActivityTimestamp()
		{
			lock (this.syncRoot)
			{
				this.transferActivityTimestamp = DateTime.Now;
			}
		}

		private void ValidateTransferPath(TransferPath path)
		{
			if (string.IsNullOrWhiteSpace(path.SourcePath)
			    && (!path.SourcePathId.HasValue || path.SourcePathId.Value < 1))
			{
				throw new ArgumentException(
					this.currentDirection == TransferDirection.Download || path.Direction == TransferDirection.Download
						? Strings.TransferPathArgumentDownloadSourcePathExceptionMessage
						: Strings.TransferPathArgumentUploadSourcePathExceptionMessage,
					nameof(path));
			}

			if (string.IsNullOrWhiteSpace(path.TargetPath) && string.IsNullOrWhiteSpace(this.TargetPath))
			{
				throw new ArgumentException(
					this.currentDirection == TransferDirection.Download || path.Direction == TransferDirection.Download
						? Strings.TransferPathArgumentDownloadTargetPathExceptionMessage
						: Strings.TransferPathArgumentUploadTargetPathExceptionMessage,
					nameof(path));
			}
		}

		/// <summary>
		/// Waits for all files to transfer without destroying the job.
		/// </summary>
		/// <returns>
		/// The <see cref="TapiTotals"/> instance.
		/// </returns>
		private TapiTotals WaitForCompletedTransfers()
		{
			const bool BatchedTotals = true;
			this.TransferLog.LogInformation("Preparing to wait for the batched transfers to complete...");

			try
			{
				// 1. Stop as soon as all file transfers are completed.
				// 2. Stop as soon as cancellation is requested and rethrow OperationCanceledException.
				// 3. Stop as soon as a permission issue is raised.
				// 4. Switch to web mode when the transfer job is no longer valid or the inactivity time is exceeded.
				// 5. All non-cancellation exceptions force switching to web mode.
				// 6. When all else fails, call WaitForCompletedTransferJob.
				const int WaitTimeBetweenChecks = 250;
				Policy.HandleResult(false).WaitAndRetryForever(
					i => TimeSpan.FromMilliseconds(WaitTimeBetweenChecks),
					(result, span) =>
						{
							// Do nothing.
						}).Execute(
					() =>
						{
							try
							{
								if (!this.CheckValidTransferJobStatus())
								{
									this.SwitchToWebMode(null);
								}

								this.cancellationToken.ThrowIfCancellationRequested();
								bool terminateWait = this.CheckCompletedTransfers()
								                     || this.CheckDataInactivityTimeExceeded()
								                     || this.CheckAbortOnRaisedPermissionIssues();
								return terminateWait;
							}
							catch (OperationCanceledException)
							{
								this.LogCancelRequest();
								this.DestroyTransferJob();
								throw;
							}
							catch (Exception e)
							{
								if (ExceptionHelper.IsFatalException(e))
								{
									throw;
								}

								// Note: this will throw if already in web mode.
								this.TransferLog.LogError(
									e,
									"An exception was thrown waiting for the transfers to complete.");
								this.SwitchToWebMode(e);
								return true;
							}
						});

				// When all else fails, just wait for the transfer job to complete
				if (!this.CheckCompletedTransfers())
				{
					this.TransferLog.LogWarning(
						"WaitForCompletedTransfers has exited, not all transfers have completed, and now going to wait for the transfer job to complete.");
					this.LogTransferTotals("Inactivity", false, BatchedTotals, this.batchTotals);

					// Do NOT return WaitForCompletedTransferJob because it returns the job totals.
					this.WaitForCompletedTransferJob();
				}

				return this.batchTotals.DeepCopy();
			}
			finally
			{
				this.batchTotals.Clear();
			}
		}

		/// <summary>
		/// Waits for all files to transfer and destroy the job.
		/// </summary>
		/// <returns>
		/// The <see cref="TapiTotals"/> instance.
		/// </returns>
		private TapiTotals WaitForCompletedTransferJob()
		{
			if (this.TransferJob == null)
			{
				return this.jobTotals.DeepCopy();
			}

			const bool BatchedTotals = false;
			this.TransferLog.LogInformation("Preparing to wait for the transfer job to complete...");

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
							this.SwitchToWebMode(exception);
						}).Execute(
					() =>
						{
							var taskResult = this.TransferJob.CompleteAsync(this.cancellationToken);
							var transferResult = taskResult.GetAwaiter().GetResult();
							this.TransferLog.LogInformation("Transfer job completed.");
							this.TransferLog.LogInformation(
								"{Name} transfer status: {Status}, elapsed time: {Elapsed}, data rate: {TransferRate:0.00} Mbps",
								this.ClientDisplayName,
								transferResult.Status,
								transferResult.Elapsed,
								transferResult.TransferRateMbps);
							switch (transferResult.Status)
							{
								case TransferStatus.Failed:
								case TransferStatus.Fatal:
									this.LogTransferTotals("NotSuccessful", true, BatchedTotals, this.jobTotals);
									string errorMessage = GetTransferErrorMessage(transferResult);
									this.PublishStatusMessage(errorMessage, TapiConstants.NoLineNumber);

									// Force web mode when job-based fatal errors or file-based errors occur.
									if (handledException == null)
									{
										throw new TransferException(errorMessage);
									}

									// Gracefully terminate.
									this.PublishFatalError(errorMessage, TapiConstants.NoLineNumber);
									break;
							}
						});
				return this.jobTotals.DeepCopy();
			}
			catch (OperationCanceledException)
			{
				this.LogCancelRequest();
				throw;
			}
			finally
			{
				this.DestroyTransferJob();
			}
		}
	}
}