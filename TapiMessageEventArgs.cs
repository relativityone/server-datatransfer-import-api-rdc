// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiMessageEventArgs.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Defines TAPI message event arguments data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;

    /// <summary>
    /// Represents TAPI message event arguments data.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TapiMessageEventArgs : EventArgs
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