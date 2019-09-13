// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiMultiClientEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents the the Transfer API multiple client event arguments data. This class cannot be inherited.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents the the Transfer API multiple transfer client event arguments data. This class cannot be inherited.
	/// </summary>
	/// <seealso cref="System.EventArgs" />
	public sealed class TapiMultiClientEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TapiMultiClientEventArgs"/> class.
		/// </summary>
		/// <param name="client">
		/// The primary transfer client.
		/// </param>
		public TapiMultiClientEventArgs(TapiClient client)
			: this(new[] { client })
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiMultiClientEventArgs"/> class.
		/// </summary>
		/// <param name="clients">
		/// The collection of transfer clients.
		/// </param>
		public TapiMultiClientEventArgs(IEnumerable<TapiClient> clients)
		{
			clients.ThrowIfNull(nameof(clients));
			this.TransferClients = new List<TapiClient>(clients);
		}

		/// <summary>
		/// Gets the list of all transfer clients.
		/// </summary>
		/// <value>
		/// The <see cref="TapiClient"/> values.
		/// </value>
		public IList<TapiClient> TransferClients
		{
			get;
		}
	}
}