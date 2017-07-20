// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeFileTransferParameters.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents the generic parameters to setup a native file transfer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Relativity.Transfer;

namespace kCura.WinEDDS.TApi
{
    using System.Net;

    /// <summary>
    /// Represents the generic parameters to setup a native file transfer.
    /// </summary>
    public class NativeFileTransferParameters
    {
        /// <summary>
        /// The flag that signals whether to force using the HTTP client.
        /// </summary>
        private bool forceHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeFileTransferParameters"/> class.
        /// </summary>
        public NativeFileTransferParameters()
        {
            this.Credentials = null;
            this.ForceClientId = Guid.Empty;
            this.ForceClientName = null;
            this.ForceHttpClient = false;
            this.IsBulkEnabled = true;
            this.MaxFilesPerFolder = 1000;
            this.MaxRetryCount = 3;

            // FileUploader.vb defaults this parameters to true.
            this.SortIntoVolumes = true;
            this.TargetPath = null;
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
        /// Gets or sets the client unique identifier that should be forced for the current transfer.
        /// </summary>
        /// <value>
        /// The client unique identifier.
        /// </value>
        public Guid ForceClientId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the client name that should be forced for the current transfer.
        /// </summary>
        /// <value>
        /// The client name.
        /// </value>
        public string ForceClientName
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
            get { return this.forceHttpClient; }

            set
            {
                if (value)
                {
                    this.ForceClientId = new Guid(TransferClientConstants.HttpClientId);
                    this.ForceClientName = TransferClientConstants.HttpClientName;
                }
                else
                {
                    this.ForceClientId = Guid.Empty;
                    this.ForceClientName = null;
                }

                this.forceHttpClient = value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether bulk mode is enabled.
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
        /// Gets or sets the max retry count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int MaxRetryCount
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