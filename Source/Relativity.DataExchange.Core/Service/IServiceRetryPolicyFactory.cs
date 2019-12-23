// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceRetryPolicyFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract factory to create service-related retry policies.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;

	using Polly;

	/// <summary>
	/// Represents an abstract factory to create service-related retry policies.
	/// </summary>
	internal interface IServiceRetryPolicyFactory
	{
		/// <summary>
		/// Creates a standard synchronous policy to handle expected service-related errors.
		/// </summary>
		/// <param name="onRetry">
		/// The action called when performing the retry.
		/// </param>
		/// <returns>
		/// The <see cref="ISyncPolicy"/> instance.
		/// </returns>
		ISyncPolicy CreatePolicy(Action<Exception, TimeSpan, int, Context> onRetry);

		/// <summary>
		/// Creates a standard asynchronous policy to handle expected service-related errors.
		/// </summary>
		/// <param name="onRetry">
		/// The action called when performing the retry.
		/// </param>
		/// <returns>
		/// The <see cref="IAsyncPolicy"/> instance.
		/// </returns>
		IAsyncPolicy CreateAsyncPolicy(Action<Exception, TimeSpan, int, Context> onRetry);
	}
}