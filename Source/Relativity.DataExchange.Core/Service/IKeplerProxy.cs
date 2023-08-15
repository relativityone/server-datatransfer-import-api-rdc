// ----------------------------------------------------------------------------
// <copyright file="IKeplerProxy.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Polly;

	/// <summary>
	/// Proxy for Kepler services.
	/// </summary>
	public interface IKeplerProxy
	{
		/// <summary>
		/// Provides <see cref="IServiceProxyFactory"/> and executes call to Kepler service.
		/// </summary>
		/// <param name="func">Function calling Kepler service.</param>
		/// <returns>Task to await.</returns>
		Task ExecuteAsync(Func<IServiceProxyFactory, Task> func);

		/// <summary>
		/// Provides <see cref="IServiceProxyFactory"/> and executes call to Kepler service without any Retry policy.
		/// </summary>
		/// <typeparam name="T">Type of the result.</typeparam>
		/// <param name="func">Function calling Kepler service.</param>
		/// <returns>Task to await.</returns>
		Task<T> ExecuteAsyncWithoutRetries<T>(Func<IServiceProxyFactory, Task<T>> func);

		/// <summary>
		/// Provides <see cref="IServiceProxyFactory"/> and executes call to Kepler service.
		/// </summary>
		/// <typeparam name="T">Type of the result.</typeparam>
		/// <param name="func">Function calling Kepler service.</param>
		/// <returns>Task to await.</returns>
		Task<T> ExecuteAsync<T>(Func<IServiceProxyFactory, Task<T>> func);

		/// <summary>
		/// Provides <see cref="IServiceProxyFactory"/> and executes call to Kepler service.
		/// </summary>
		/// <typeparam name="T">Type of the result.</typeparam>
		/// <param name="func">Function calling Kepler service.</param>
		/// <param name="waitTimeSeconds">Time before we assume no response for request. </param>
		/// <returns>Task to await.</returns>
		Task<T> ExecuteAsync<T>(Func<IServiceProxyFactory, Task<T>> func, int waitTimeSeconds);

		/// <summary>
		/// Provides <see cref="IServiceProxyFactory"/> and executes call to Kepler service.
		/// </summary>
		/// <typeparam name="T">Type of the result.</typeparam>
		/// <param name="context">polly context.</param>
		/// <param name="cancellationToken">cancellationToken.</param>
		/// <param name="func">Function calling Kepler service.</param>
		/// <returns>Task to await.</returns>
		Task<T> ExecuteAsync<T>(Context context, CancellationToken cancellationToken, Func<Context, CancellationToken, IServiceProxyFactory, Task<T>> func);

		/// <summary>
		/// Provides <see cref="IServiceProxyFactory"/> Used to provide method for specific api call.
		/// </summary>
		/// <param name="endpointAddress">Relativity kepler service address. E.g. '/Relativity.Rest/api/Relativity.Services.InstanceDetails.IInstanceDetailsModule/...'.</param>
		/// <param name="body">body of request.</param>
		/// <returns>Task to await.</returns>
		Task<string> ExecutePostAsync(string endpointAddress, string body);
	}
}