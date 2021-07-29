// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceConnectionInfo.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract object that contains service connection information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;
	using System.Net;

	using Relativity.Services.ServiceProxy;

	/// <summary>
	/// Represents an abstract object that contains service connection information.
	/// </summary>
	public interface IServiceConnectionInfo
	{
		/// <summary>
		/// Gets the service credentials.
		/// </summary>
		/// <value>
		/// The <see cref="Credentials"/> instance.
		/// </value>
		Credentials Credentials
		{
			get;
		}

		/// <summary>
		/// Gets the base URL for the Relativity web services.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		Uri WebServiceBaseUrl
		{
			get;
		}

		/// <summary>
		/// Updates credentials used for authenticating in Kepler.
		/// </summary>
		/// <param name="credential">Credentials.</param>
		void UpdateCredentials(NetworkCredential credential);

		/// <summary>
		/// Checks if current credentials has changed and refreshes them if needed.
		/// </summary>
		/// <returns>True if credentials has changed, otherwise false.</returns>
		bool RefreshCredentials();
	}
}