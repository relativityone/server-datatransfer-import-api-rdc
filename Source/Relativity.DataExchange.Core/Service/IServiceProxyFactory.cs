﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceProxyFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract class object to create web service proxy instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;
	using System.Net;

	/// <summary>
	/// Represents an abstract class object to create web service proxy instances.
	/// </summary>
	public interface IServiceProxyFactory : IDisposable
	{
		/// <summary>
		/// Retrieve a service proxy instance.
		/// </summary>
		/// <typeparam name="TProxy">
		/// The type of proxy service to retrieve.
		/// </typeparam>
		/// <returns>
		/// The <typeparamref name="TProxy"/> instance.
		/// </returns>
		TProxy CreateProxyInstance<TProxy>()
			where TProxy : class, IDisposable;

		/// <summary>
		/// Updates credentials used for authenticating in Kepler.
		/// </summary>
		/// <param name="credential">Credential.</param>
		void UpdateCredentials(NetworkCredential credential);
	}
}