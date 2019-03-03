﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiClientEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines the transfer client event arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
{
    using System;

    /// <summary>
    /// Represents the Transfer API client event arguments data.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public sealed class TapiClientEventArgs : EventArgs
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
        public TapiClientEventArgs(string name, TapiClient client)
        {
            this.Name = name;
            this.Client = client;
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
        /// Gets the current transfer client.
        /// </summary>
        /// <value>
        /// The name
        /// <see cref="TapiClient"/> value.
        /// </value>
        public TapiClient Client
        {
            get;
        }
    }
}