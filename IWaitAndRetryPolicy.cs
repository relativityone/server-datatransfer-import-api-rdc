using System;
using System.Threading;

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// The retry policy class object.
    /// </summary>
    public interface IWaitAndRetryPolicy
    {
        /// <summary>
        /// Number of retries
        /// </summary>
        int NumberOfRetries { get; }
        /// <summary>
        /// Wait time in seconds between retry attempts
        /// </summary>
        int WaitTimeSecondsBetweenRetryAttempts { get; }

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
            Action<CancellationToken> execFunc, CancellationToken token) where TException : Exception;

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
		/// A token which is used to cancell current task.
		/// </param>
		void WaitAndRetry<TException>(
            int maxRetryCount,
            Func<int, TimeSpan> retryDuration,
            Action<Exception, TimeSpan> retryAction,
            Action<CancellationToken> execFunc, CancellationToken token) where TException : Exception;

    }
}
