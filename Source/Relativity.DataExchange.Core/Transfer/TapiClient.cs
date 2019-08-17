// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiClient.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents the supported transfer API clients.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;

	/// <summary>
	/// Represents the supported Transfer API clients. These can be combined to represent multiple transfer clients.
	/// </summary>
	/// <remarks>
	/// This is provided purely for backwards compatibility with import/export components.
	/// </remarks>
	[Flags]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Naming",
		"CA1714:FlagsEnumsShouldHavePluralNames",
		Justification = "This is only used internally.")]
	public enum TapiClient
    {
        /// <summary>
        /// The client plugin is not set or is undefined.
        /// </summary>
        None = 0,

        /// <summary>
        /// The File Share or direct client.
        /// </summary>
        Direct = 1,

        /// <summary>
        /// The Aspera client.
        /// </summary>
        Aspera = 2,

        /// <summary>
        /// The HTTP or web client.
        /// </summary>
        Web = 4,
	}
}