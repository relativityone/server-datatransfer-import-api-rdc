// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferMessageEventArgs.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Defines the transfer message event arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;

    /// <summary>
    /// Represents the transfer message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TransferMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        public TransferMessageEventArgs(string message, int lineNumber)
        {
            this.Message = message;
            this.LineNumber = lineNumber;
        }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        public int LineNumber
        {
            get;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get;
        }
    }
}