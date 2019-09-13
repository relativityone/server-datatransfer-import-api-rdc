// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiBridgeParameters2.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents the generic parameters to setup a native file transfer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Net;

	using Relativity.Transfer;

	/// <summary>
	/// Represents the generic parameters to setup a Transfer API bridge.
	/// </summary>
	public class TapiBridgeParameters2
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TapiBridgeParameters2"/> class.
		/// </summary>
		public TapiBridgeParameters2()
		{
			this.Application = null;
			this.AsperaBcpRootFolder = AppSettingsConstants.TapiAsperaBcpRootFolderDefaultValue;
			this.AsperaDocRootLevels = AppSettingsConstants.TapiAsperaNativeDocRootLevelsDefaultValue;
			this.BadPathErrorsRetry = AppSettingsConstants.TapiBadPathErrorsRetryDefaultValue;
			this.ClientRequestId = Guid.NewGuid();
			this.Credentials = null;
			this.FileNotFoundErrorsDisabled = AppSettingsConstants.TapiFileNotFoundErrorsDisabledDefaultValue;
			this.FileNotFoundErrorsRetry = AppSettingsConstants.TapiFileNotFoundErrorsRetryDefaultValue;
			this.FileShare = null;
			this.ForceAsperaClient = AppSettingsConstants.TapiForceAsperaClientDefaultValue;
			this.ForceClientCandidates = AppSettingsConstants.TapiForceClientCandidatesDefaultValue;
			this.ForceHttpClient = AppSettingsConstants.TapiForceHttpClientDefaultValue;
			this.ForceFileShareClient = AppSettingsConstants.TapiForceFileShareClientDefaultValue;
			this.LargeFileProgressEnabled = AppSettingsConstants.TapiLargeFileProgressEnabledDefaultValue;
			this.LogConfigFile = null;
			this.MaxInactivitySeconds = AppSettingsConstants.TapiMaxInactivitySecondsDefaultValue;
			this.MaxJobParallelism = AppSettingsConstants.TapiMaxJobParallelismDefaultValue;
			this.MaxJobRetryAttempts = 3;
			this.MinDataRateMbps = 0;
			this.PermissionErrorsRetry = AppSettingsConstants.PermissionErrorsRetryDefaultValue;
			this.PreserveFileTimestamps = AppSettingsConstants.TapiPreserveFileTimestampsDefaultValue;
			this.SubmitApmMetrics = AppSettingsConstants.TapiSubmitApmMetricsDefaultValue;
			this.SupportCheckPath = null;
			this.TargetDataRateMbps = AppSettingsConstants.TapiTargetDataRateMbpsDefaultValue;
			this.TargetPath = null;
			this.TransferCredential = null;
			this.TransferLogDirectory = null;
			this.TimeoutSeconds = AppSettingsConstants.HttpTimeoutSecondsDefaultValue;
			this.WaitTimeBetweenRetryAttempts = AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue;
			this.WebServiceUrl = null;
			this.WebCookieContainer = null;
			this.WorkspaceId = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiBridgeParameters2"/> class.
		/// </summary>
		/// <param name="copy">
		/// The parameters to copy.
		/// </param>
		public TapiBridgeParameters2(TapiBridgeParameters2 copy)
		{
			if (copy == null)
			{
				throw new ArgumentNullException(nameof(copy));
			}

			this.Application = copy.Application;
			this.AsperaBcpRootFolder = copy.AsperaBcpRootFolder;
			this.AsperaDocRootLevels = copy.AsperaDocRootLevels;
			this.BadPathErrorsRetry = copy.BadPathErrorsRetry;
			this.ClientRequestId = copy.ClientRequestId;
			this.Credentials = copy.Credentials;
			this.FileNotFoundErrorsRetry = copy.FileNotFoundErrorsRetry;
			this.FileNotFoundErrorsDisabled = copy.FileNotFoundErrorsDisabled;
			this.FileShare = copy.FileShare;
			this.ForceAsperaClient = copy.ForceAsperaClient;
			this.ForceClientCandidates = copy.ForceClientCandidates;
			this.ForceHttpClient = copy.ForceHttpClient;
			this.ForceFileShareClient = copy.ForceFileShareClient;
			this.LargeFileProgressEnabled = copy.LargeFileProgressEnabled;
			this.TargetDataRateMbps = copy.TargetDataRateMbps;
			this.LogConfigFile = copy.LogConfigFile;
			this.MaxInactivitySeconds = copy.MaxInactivitySeconds;
			this.MaxJobParallelism = copy.MaxJobParallelism;
			this.MaxJobRetryAttempts = copy.MaxJobRetryAttempts;
			this.MinDataRateMbps = copy.MinDataRateMbps;
			this.PermissionErrorsRetry = copy.PermissionErrorsRetry;
			this.PreserveFileTimestamps = copy.PreserveFileTimestamps;
			this.SubmitApmMetrics = copy.SubmitApmMetrics;
			this.SupportCheckPath = copy.SupportCheckPath;
			this.TargetPath = copy.TargetPath;
			this.TimeoutSeconds = copy.TimeoutSeconds;
			this.TransferCredential = copy.TransferCredential;
			this.TransferLogDirectory = copy.TransferLogDirectory;
			this.WaitTimeBetweenRetryAttempts = copy.WaitTimeBetweenRetryAttempts;
			this.WebServiceUrl = copy.WebServiceUrl;
			this.WebCookieContainer = copy.WebCookieContainer;
			this.WorkspaceId = copy.WorkspaceId;
		}

		/// <summary>
		/// Gets or sets the optional application name for this request. When specified, this value is attached to the APM metrics and other reporting features for added insight.
		/// </summary>
		/// <value>
		/// The application.
		/// </value>
		public string Application
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Aspera BCP root folder.
		/// </summary>
		/// <value>
		/// The folder.
		/// </value>
		public string AsperaBcpRootFolder
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number of levels the Aspera doc root folder is relative to the file share.
		/// </summary>
		/// <value>
		/// The number of levels.
		/// </value>
		public int AsperaDocRootLevels
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether Transfer API should retry on bad path errors.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if TAPI should retry; otherwise, <see langword="false" />.
		/// </value>
		public bool BadPathErrorsRetry
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the client request unique identifier.
		/// </summary>
		/// <value>
		/// The <see cref="Guid"/> value.
		/// </value>
		public Guid ClientRequestId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity network credentials.
		/// </summary>
		/// <value>
		/// The <see cref="NetworkCredential"/> instance.
		/// </value>
		public NetworkCredential Credentials
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to treat missing files as warnings or errors.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to disable treating missing files as errors; otherwise, <see langword="false" />.
		/// </value>
		public bool FileNotFoundErrorsDisabled
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to retry missing file errors.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to retry missing file errors; otherwise, <see langword="false" />.
		/// </value>
		public bool FileNotFoundErrorsRetry
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the file share UNC path. This value should come directly from the Workspace.
		/// </summary>
		/// <value>
		/// The file share.
		/// </value>
		public string FileShare
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the semi-colon delimited list of client candidates. Native Transfer API client identifiers must be used (IE FileShare;Aspera;Http).
		/// </summary>
		/// <value>
		/// The client candidates.
		/// </value>
		public string ForceClientCandidates
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to force using the Aspera client.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to force using the Aspera client; otherwise, <see langword="false" />.
		/// </value>
		public bool ForceAsperaClient
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to force using the HTTP client.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to force using the HTTP client; otherwise, <see langword="false" />.
		/// </value>
		public bool ForceHttpClient
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to force using the file share client.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to force using the file share client; otherwise, <see langword="false" />.
		/// </value>
		public bool ForceFileShareClient
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether large file progress is enabled.
		/// </summary>
		/// <value>
		/// The large file progress enabled value.
		/// </value>
		public bool LargeFileProgressEnabled
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the full path to the log configuration file.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public string LogConfigFile
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the maximum number of seconds to wait before considering a transfer where data isn't being written inactive.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		public int MaxInactivitySeconds
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the max degree of parallelism when creating a job.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		public int MaxJobParallelism
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the max number of job retries.
		/// </summary>
		/// <value>
		/// The max job retry count.
		/// </value>
		public int MaxJobRetryAttempts
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the minimum data rate in Mbps units.
		/// </summary>
		/// <value>
		/// The minimum data rate.
		/// </value>
		public int MinDataRateMbps
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether Transfer API should retry on file permission errors.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if Transfer API should retry; otherwise, <see langword="false" />.
		/// </value>
		public bool PermissionErrorsRetry
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to preserve import and export file timestamps.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to preserve file timestamps; otherwise, <see langword="false" />.
		/// </value>
		public bool PreserveFileTimestamps
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to submit APM metrics to Relativity once the transfer job completes. This is <see langword="true" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to submit APM metrics; otherwise, <see langword="false" />.
		/// </value>
		public bool SubmitApmMetrics
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the support check path.
		/// </summary>
		/// <value>
		/// The support check path.
		/// </value>
		public string SupportCheckPath
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the target data rate in Mbps units.
		/// </summary>
		/// <value>
		/// The target data rate.
		/// </value>
		public int TargetDataRateMbps
		{
			get;
			set;
		}

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
		/// Gets or sets the timeout in seconds.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		public int TimeoutSeconds
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the transfer credential.
		/// </summary>
		/// <value>
		/// The <see cref="Credential"/> instance.
		/// </value>
		public Credential TransferCredential
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the transfer log directory.
		/// </summary>
		/// <value>
		/// The directory.
		/// </value>
		public string TransferLogDirectory
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the cookie container.
		/// </summary>
		/// <value>
		/// The <see cref="CookieContainer"/> instance.
		/// </value>
		public CookieContainer WebCookieContainer
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the amount of time, in seconds, to wait wait between retry attempts.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		public int WaitTimeBetweenRetryAttempts
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the web service URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		public string WebServiceUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the workspace artifact identifier.
		/// </summary>
		/// <value>
		/// The artifact identifier.
		/// </value>
		public int WorkspaceId
		{
			get;
			set;
		}
	}
}