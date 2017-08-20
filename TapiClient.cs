// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiClient.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents the supported transfer API clients.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Represents the supported transfer API clients.
    /// </summary>
    /// <remarks>
    /// This is provided purely for backwards compatibility with WinEDDS.
    /// </remarks>
    public enum TapiClient
    {
        /// <summary>
        /// The client plugin is not set or is undefined.
        /// </summary>
        None,

        /// <summary>
        /// The HTTP or web client.
        /// </summary>
        Web,

        /// <summary>
        /// The File Share or direct client.
        /// </summary>
        Direct,

        /// <summary>
        /// The Aspera client.
        /// </summary>
        Aspera
    }
}