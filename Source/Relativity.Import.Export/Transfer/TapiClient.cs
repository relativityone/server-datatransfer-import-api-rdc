// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiClient.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents the supported transfer API clients.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
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
        Aspera,
    }
}