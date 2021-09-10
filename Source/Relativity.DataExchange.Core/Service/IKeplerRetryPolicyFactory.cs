// ----------------------------------------------------------------------------
// <copyright file="IKeplerRetryPolicyFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using Polly;

	/// <summary>
	/// Create retries policy for kepler's call.
	/// </summary>
	public interface IKeplerRetryPolicyFactory
	{
		/// <summary>
		/// Create retries policy for kepler's call.
		/// </summary>
		/// <returns>Retries policy for kepler's call.</returns>
		IAsyncPolicy CreateRetryPolicy();

		/// <summary>
		/// Create retries policy for kepler's call.
		/// </summary>
		/// /// <typeparam name="T">
		/// The type of result returned from Kepler's.
		/// </typeparam>
		/// <returns>Retries policy for kepler's call.</returns>
		IAsyncPolicy<T> CreateRetryPolicy<T>();
	}
}