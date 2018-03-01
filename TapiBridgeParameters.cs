// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiBridgeParameters.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents the generic parameters to setup a native file transfer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Relativity.Transfer;

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.Net;

    /// <summary>
    /// Represents the generic parameters to setup a Transfer API bridge.
    /// </summary>
    public class TapiBridgeParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TapiBridgeParameters"/> class.
        /// </summary>
        public TapiBridgeParameters()
        {
            this.AsperaDocRootLevels = 1;
            this.ClientRequestId = Guid.NewGuid();
            this.Credentials = null;
            this.FileShare = null;
            this.ForceAsperaClient = false;
            this.ForceClientCandidates = null;
            this.ForceHttpClient = false;
            this.ForceFileShareClient = false;
            this.LargeFileProgressEnabled = false;
            this.LogConfigFile = null;
            this.MaxJobParallelism = 10;
            this.MaxJobRetryAttempts = 3;
            this.MinDataRateMbps = 0;
            this.TargetDataRateMbps = 100;
            this.TargetPath = null;
            this.TransferLogDirectory = null;
            this.TimeoutSeconds = 300;
            this.WaitTimeBetweenRetryAttempts = 30;
            this.WebServiceUrl = null;
            this.WebCookieContainer = null;
            this.WorkspaceId = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TapiBridgeParameters"/> class.
        /// </summary>
        /// <param name="copy">
        /// The parameters to copy.
        /// </param>
        public TapiBridgeParameters(TapiBridgeParameters copy)
        {
            if (copy == null)
            {
                throw new ArgumentNullException(nameof(copy));
            }

            this.AsperaDocRootLevels = copy.AsperaDocRootLevels;
            this.ClientRequestId = copy.ClientRequestId;
            this.Credentials = copy.Credentials;
            this.FileShare = copy.FileShare;
            this.ForceAsperaClient = copy.ForceAsperaClient;
            this.ForceClientCandidates = copy.ForceClientCandidates;
            this.ForceHttpClient = copy.ForceHttpClient;
            this.ForceFileShareClient = copy.ForceFileShareClient;
            this.LargeFileProgressEnabled = copy.LargeFileProgressEnabled;
            this.TargetDataRateMbps = copy.TargetDataRateMbps;
            this.LogConfigFile = copy.LogConfigFile;
            this.MaxJobParallelism = copy.MaxJobParallelism;
            this.MaxJobRetryAttempts = copy.MaxJobRetryAttempts;
            this.MinDataRateMbps = copy.MinDataRateMbps;
            this.TargetPath = copy.TargetPath;
            this.TimeoutSeconds = copy.TimeoutSeconds;
            this.TransferLogDirectory = copy.TransferLogDirectory;
            this.WaitTimeBetweenRetryAttempts = copy.WaitTimeBetweenRetryAttempts;
            this.WebServiceUrl = copy.WebServiceUrl;
            this.WebCookieContainer = copy.WebCookieContainer;
            this.WorkspaceId = copy.WorkspaceId;
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
		/// Gets or sets the Aspera credentials
		/// </summary>
		/// <value>
		/// The <see cref="Credential"/> instance.
		/// </value>
		public Credential FileshareCredentials
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
        /// Gets or sets the semi-colon delimited list of client candidates. Native TAPI client identifiers must be used (IE FileShare;Aspera;Http).
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
        /// Gets or sets the target path
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
        /// The timeout.
        /// </value>
        public int TimeoutSeconds
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
        /// The wait time.
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