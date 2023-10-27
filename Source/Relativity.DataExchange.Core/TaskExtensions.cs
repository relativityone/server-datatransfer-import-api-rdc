// <copyright file="TaskExtensions.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// Represents extension methods for common task operations.
	/// </summary>
	public static class TaskExtensions
	{
		/// <summary>
		/// Awaits the task and will return if the timeout is exceeded.
		/// </summary>
		/// <param name="source">Main task.</param>
		/// <param name="timeout">Timeout to throw TimeoutException.</param>
		/// <param name="operation">Name of the operation.</param>
		/// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
		public static async Task WithTimeout(this Task source, TimeSpan timeout, string operation)
		{
			using (var timeoutCancellationTokenSource = new CancellationTokenSource())
			{
				var completedTask = await Task.WhenAny(source, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).ConfigureAwait(false);
				if (completedTask == source)
				{
					timeoutCancellationTokenSource.Cancel();
					await source.ConfigureAwait(false);
				}
				else
				{
					throw new TimeoutException($"The operation: {operation} has timed out after: {timeout}.");
				}
			}
		}

		/// <summary>
		/// Awaits the task and will return if the timeout is exceeded.
		/// </summary>
		/// <typeparam name="TResult">Generic Result type.</typeparam>
		/// <param name="source">Main task.</param>
		/// <param name="timeout">Timeout to throw TimeoutException.</param>
		/// <param name="operation">Name of the operation.</param>
		/// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
		public static async Task<TResult> WithTimeout<TResult>(this Task<TResult> source, TimeSpan timeout, string operation)
		{
			using (var timeoutCancellationTokenSource = new CancellationTokenSource())
			{
				var completedTask = await Task.WhenAny(source, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).ConfigureAwait(false);
				if (completedTask == source)
				{
					timeoutCancellationTokenSource.Cancel();
					return await source.ConfigureAwait(false);
				}
				else
				{
					throw new TimeoutException($"The operation: {operation} has timed out after: {timeout}.");
				}
			}
		}
	}
}
