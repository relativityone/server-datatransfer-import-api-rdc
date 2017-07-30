// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiClientEventArgs.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Defines the transfer client event arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;

    /// <summary>
    /// Represents the TAPI client event arguments data.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TapiClientEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TapiClientEventArgs"/> class.
        /// </summary>
        /// <param name="name">
        /// The current transfer client name.
        /// </param>
        /// <param name="client">
        /// The current transfer client type.
        /// </param>
        /// <param name="isBulkEnabled">
        /// Specify whether the bulk feature is enabled.
        /// </param>
        public TapiClientEventArgs(string name, TransferClient client, bool isBulkEnabled)
        {
            this.Name = name;
            this.ClientType = client;
            this.IsBulkEnabled = isBulkEnabled;
        }

        /// <summary>
        /// Gets the current client name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets the current transfer client type.
        /// </summary>
        /// <value>
        /// The name
        /// <see cref="TransferClient"/> value.
        /// </value>
        public TransferClient ClientType
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the bulk feature is enabled.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the bulk feature is enabled; otherwise, <see langword="false" />.
        /// </value>
        public bool IsBulkEnabled
        {
            get;
        }
    }
}