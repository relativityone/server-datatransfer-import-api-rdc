// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiClientEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines the transfer client event arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;

	/// <summary>
	/// Represents the Transfer API client event arguments data. This class cannot be inherited.
	/// </summary>
	/// <seealso cref="System.EventArgs" />
	public sealed class TapiClientEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TapiClientEventArgs"/> class.
		/// </summary>
		/// <param name="instanceId">
		/// The current transfer bridge instance unique identifier.
		/// </param>
		/// <param name="name">
		/// The current transfer client name.
		/// </param>
		/// <param name="client">
		/// The current transfer client type.
		/// </param>
		public TapiClientEventArgs(Guid instanceId, string name, TapiClient client)
		{
			this.InstanceId = instanceId;
			this.Name = name;
			this.Client = client;
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

		/// <summary>
		/// Gets the transfer client instance unique identifier that raised this event.
		/// </summary>
		/// <value>
		/// The <see cref="Guid"/> value.
		/// </value>
		public Guid InstanceId
		{
			get;
		}

		/// <summary>
		/// Gets the current transfer client name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name
		{
			get;
		}
	}
}