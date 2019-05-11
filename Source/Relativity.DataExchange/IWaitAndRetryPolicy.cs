// ----------------------------------------------------------------------------
// <copyright file="IWaitAndRetryPolicy.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract object to support resiliency and retry policies.
// </summary>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Threading;

	/// <summary>
	/// Represents an abstract object to support resiliency and retry policies.
	/// </summary>
	public interface IWaitAndRetryPolicy
	{
		/// <summary>
		/// Performs the synchronous retry operation using the specified retry duration function.
		/// </summary>
		/// <typeparam name="TException">
		/// The exception type to handle.
		/// </typeparam>
		/// <param name="retryDuration">
		/// The duration between retry attempts.
		/// </param>
		/// <param name="retryAction">
		/// The action performed when a retry occurs.
		/// </param>
		/// <param name="execFunc">
		/// The main function executed.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		void WaitAndRetry<TException>(
			Func<int, TimeSpan> retryDuration,
			Action<Exception, TimeSpan> retryAction,
			Action<CancellationToken> execFunc,
			CancellationToken token)
			where TException : Exception;

		/// <summary>
		/// Performs the synchronous retry operation for the specified exception type and retry duration function.
		/// </summary>
		/// <typeparam name="TException">
		/// The exception type to handle.
		/// </typeparam>
		/// <param name="maxRetryCount">
		/// The maximum retry count.
		/// </param>
		/// <param name="retryDuration">
		/// The duration between retry attempts.
		/// </param>
		/// <param name="retryAction">
		/// The action performed when a retry occurs.
		/// </param>
		/// <param name="execFunc">
		/// The main function executed.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		void WaitAndRetry<TException>(
			int maxRetryCount,
			Func<int, TimeSpan> retryDuration,
			Action<Exception, TimeSpan> retryAction,
			Action<CancellationToken> execFunc,
			CancellationToken token)
			where TException : Exception;

		/// <summary>
		/// Performs the synchronous retry operation for the specified exception type and retry duration function and return a value.
		/// </summary>
		/// <typeparam name="TResult">
		/// The result or return value.
		/// </typeparam>
		/// <typeparam name="TException">
		/// The exception type to handle.
		/// </typeparam>
		/// <param name="retryDuration">
		/// The duration between retry attempts.
		/// </param>
		/// <param name="retryAction">
		/// The action performed when a retry occurs.
		/// </param>
		/// <param name="execFunc">
		/// The main function executed.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		/// <returns>
		/// The <typeparamref name="TResult"/> value.
		/// </returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1004:GenericMethodsShouldProvideTypeParameter",
			Justification = "This is required to support return value.")]
		TResult WaitAndRetry<TResult, TException>(
			Func<int, TimeSpan> retryDuration,
			Action<Exception, TimeSpan> retryAction,
			Func<CancellationToken, TResult> execFunc,
			CancellationToken token)
			where TException : Exception;

		/// <summary>
		/// Performs the synchronous retry operation for the specified exception and retry duration function and return a value.
		/// </summary>
		/// <typeparam name="TResult">
		/// The result or return value.
		/// </typeparam>
		/// <param name="exceptionPredicate">
		/// The function called to determine whether to retry the specified exception.
		/// </param>
		/// <param name="retryDuration">
		/// The duration between retry attempts.
		/// </param>
		/// <param name="retryAction">
		/// The action performed when a retry occurs.
		/// </param>
		/// <param name="execFunc">
		/// The main function executed.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		/// <returns>
		/// The <typeparamref name="TResult"/> value.
		/// </returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1004:GenericMethodsShouldProvideTypeParameter",
			Justification = "This is required to support return value.")]
		TResult WaitAndRetry<TResult>(
			Func<Exception, bool> exceptionPredicate,
			Func<int, TimeSpan> retryDuration,
			Action<Exception, TimeSpan> retryAction,
			Func<CancellationToken, TResult> execFunc,
			CancellationToken token);

		/// <summary>
		/// Performs the synchronous retry operation for the specified exception type and retry duration function and return a value.
		/// </summary>
		/// <typeparam name="TResult">
		/// The result or return value.
		/// </typeparam>
		/// <typeparam name="TException">
		/// The exception type to handle.
		/// </typeparam>
		/// <param name="maxRetryCount">
		/// The maximum retry count.
		/// </param>
		/// <param name="retryDuration">
		/// The duration between retry attempts.
		/// </param>
		/// <param name="retryAction">
		/// The action performed when a retry occurs.
		/// </param>
		/// <param name="execFunc">
		/// The main function executed.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		/// <returns>
		/// The <typeparamref name="TResult"/> value.
		/// </returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1004:GenericMethodsShouldProvideTypeParameter",
			Justification = "This is required to support return value.")]
		TResult WaitAndRetry<TResult, TException>(
			int maxRetryCount,
			Func<int, TimeSpan> retryDuration,
			Action<Exception, TimeSpan> retryAction,
			Func<CancellationToken, TResult> execFunc,
			CancellationToken token)
			where TException : Exception;
	}
}
