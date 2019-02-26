// --------------------------------------------------------------------------------------------------------------------
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract object to support resiliency and retry policies.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
	using System.Threading;

    /// <summary>
    /// Represents an abstract object to support resiliency and retry policies.
    /// </summary>
    public interface IWaitAndRetryPolicy
    {
        /// <summary>
        /// Gets or sets the maximum number of retry attempts.
        /// </summary>
        /// <value>
        /// The retry attempts.
        /// </value>
        int MaxRetryAttempts
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total wait time, in seconds, between retry attempts
        /// </summary>
        /// <value>
        /// The total seconds.
        /// </value>
        int WaitTimeSecondsBetweenRetryAttempts
        {
            get;
            set;
        }

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
		/// A token which is used to cancell current task.
		/// </param>
        void WaitAndRetry<TException>(
            Func<int, TimeSpan> retryDuration,
            Action<Exception, TimeSpan> retryAction,
            Action<CancellationToken> execFunc, 
			CancellationToken token) where TException : Exception;

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
		/// A token which is used to cancel current task.
		/// </param>
        void WaitAndRetry<TException>(
            int maxRetryCount,
            Func<int, TimeSpan> retryDuration,
            Action<Exception, TimeSpan> retryAction,
			Action<CancellationToken> execFunc, 
			CancellationToken token) where TException : Exception;

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
		/// A token which is used to cancel current task.</param>
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
		/// A token which is used to cancel current task.</param>
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
		/// A token which is used to cancell current task.
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
