// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientChangeReason.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an enumeration of reasons why a transfer client has changed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
{
	/// <summary>
	/// Represents an enumeration of reasons why a transfer client has changed.
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
