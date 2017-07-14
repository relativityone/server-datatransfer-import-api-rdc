// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferClientPlugin.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Defines the supported transfer client plugins.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Represents the supported transfer client plugins.
    /// </summary>
    public enum TransferClientPlugin
    {
        /// <summary>
        /// The client plugin has not been specified.
        /// </summary>
        None,

        /// <summary>
        /// The HTTP or web client plugin.
        /// </summary>
        Web,

        /// <summary>
        /// The File Share or direct client plugin.
        /// </summary>
        Direct,

        /// <summary>
        /// The Aspera client plugin.
        /// </summary>
        Aspera,

        /// <summary>
        /// The Third Party client plugin.
        /// </summary>
        ThirdParty
    }
}