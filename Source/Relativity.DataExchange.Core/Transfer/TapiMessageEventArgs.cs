// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiMessageEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines Transfer API message event arguments data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
    using System;

    /// <summary>
    /// Represents Transfer API message event arguments data. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public sealed class TapiMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TapiMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        public TapiMessageEventArgs(string message, int lineNumber)
            : this(message, lineNumber, isMalwareError: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TapiMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <param name="isMalwareError">
        /// Information if this is a malware error.
        /// </param>
        public TapiMessageEventArgs(string message, int lineNumber, bool isMalwareError)
        {
            this.Message = message;
            this.LineNumber = lineNumber;
            this.IsMalwareError = isMalwareError;
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

        /// <summary>
        /// Gets a value indicating whether this message is malware exception.
        /// </summary>
        public bool IsMalwareError { get; }
    }
}