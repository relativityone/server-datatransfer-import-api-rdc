// ----------------------------------------------------------------------------
// <copyright file="IKeplerProxy.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;
	using System.Threading.Tasks;

	/// <summary>
	/// Proxy for Kepler services.
	/// </summary>
	public interface IKeplerProxy
	{
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
		/// <param name="func">Function calling Kepler service.</param>
		/// <returns>Task to await.</returns>
		Task ExecuteAsync(Func<IServiceProxyFactory, Task> func);
	}
}