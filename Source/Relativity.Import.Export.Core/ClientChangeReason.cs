// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientChangeReason.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents an enumation of reasons why a transfer client has changed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Represents an enumation of reasons why a transfer client has changed.
    /// </summary>
    internal enum ClientChangeReason
    {
        /// <summary>
        /// The client changed due to an initial force configuration setting.
        /// </summary>
        ForceConfig,

        /// <summary>
        /// The client changed due to a best-fit initial assignment.
        /// </summary>
        BestFit,

        /// <summary>
        /// The client changed due to an HTTP fallback after a different client already failed.
        /// </summary>
        HttpFallback
    }
}
