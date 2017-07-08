// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferClientEventArgs.cs" company="kCura Corp">
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
    /// Represents the transfer client event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TransferClientEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferClientEventArgs"/> class.
        /// </summary>
        /// <param name="name">
        /// The client name.
        /// </param>
        /// <param name="isBulkEnabled">
        /// Specify whether the bulk feature is enabled.
        /// </param>
        public TransferClientEventArgs(string name, bool isBulkEnabled)
        {
            this.Name = name;
            this.IsBulkEnabled = isBulkEnabled;
        }

        /// <summary>
        /// Gets the client name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
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