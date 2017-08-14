// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeFileTransferParameters.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents the generic parameters to setup a native file transfer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System.Net;

    /// <summary>
    /// Represents the generic parameters to setup a native file transfer.
    /// </summary>
    public class NativeFileTransferParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NativeFileTransferParameters"/> class.
        /// </summary>
        public NativeFileTransferParameters()
        {
            this.Credentials = null;
            this.ForceAsperaClient = false;
            this.ForceHttpClient = false;            
            this.ForceFileShareClient = false;
            this.IsBulkEnabled = true;
            this.LargeFileProgressEnabled = false;
            this.MaxFilesPerFolder = 1000;
            this.MaxJobParallelism = 10;
            this.MaxJobRetryAttempts = 3;

            // FileUploader.vb defaults this parameters to true.
            this.SortIntoVolumes = true;
            this.TargetPath = null;
            this.TimeoutSeconds = 300;
            this.WaitTimeBetweenRetryAttempts = 30;
            this.WebServiceUrl = null;
            this.WebCookieContainer = null;
            this.WorkspaceId = 0;
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
        /// Gets or sets a value indicating whether bulk mode is enabled.
        /// </summary>
        /// <value>
        /// The bulk mode enabled value.
        /// </value>
        public bool IsBulkEnabled
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
        /// Gets or sets a value indicating whether the TAPI log is enabled.
        /// </summary>
        /// <value>
        /// The log enabled value.
        /// </value>
        public bool LogEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum files per folder setting.
        /// </summary>
        /// <value>
        /// The maximum files per folder.
        /// </value>
        public int MaxFilesPerFolder
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
        /// Gets or sets a value indicating whether to sort all transfers into a volumes folder.
        /// </summary>
        /// <value>
        /// <see langword="true" /> to sort all transfers into a volumes folder; otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>
        /// This is always <see langword="true" /> unless transferring BCP files.
        /// </remarks>
        public bool SortIntoVolumes
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