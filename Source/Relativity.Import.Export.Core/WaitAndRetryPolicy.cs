﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitAndRetryPolicy.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to support resiliency and retry policies.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export
{
    using System;
    using System.Threading;
    using Polly;

    using Relativity.Import.Export.Io;

    /// <summary>
    /// Represents a wait and retry policy class objects with a default back-off time strategy.
    /// </summary>
    public class WaitAndRetryPolicy : IWaitAndRetryPolicy
    {
        /// <summary>
        /// The default number of retries.
        /// </summary>
        private const int DefaultNumberOfRetries = 1;

        /// <summary>
        /// The default wait time in seconds.
        /// </summary>
        private const int DefaultWaitTimeSeconds = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitAndRetryPolicy"/> class.
        /// </summary>
        public WaitAndRetryPolicy()
            : this(DefaultNumberOfRetries, DefaultWaitTimeSeconds)
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="WaitAndRetryPolicy"/> class.
		/// </summary>
		/// <param name="maxRetryAttempts">
		/// The maximum number of retries.
		/// </param>
		/// <param name="waitTimeSecondsBetweenRetryAttempts">
		/// The number of seconds to wait between retry attempts.
		/// </param>
		public WaitAndRetryPolicy(int maxRetryAttempts, int waitTimeSecondsBetweenRetryAttempts)
        {
            this.MaxRetryAttempts = maxRetryAttempts;
            this.WaitTimeSecondsBetweenRetryAttempts = waitTimeSecondsBetweenRetryAttempts;
        }

        /// <inheritdoc />
        public int MaxRetryAttempts
        {
            get;
            set;
        }

        /// <inheritdoc />
        public int WaitTimeSecondsBetweenRetryAttempts
        {
            get;
            set;
        }

        /// <inheritdoc />
        public void WaitAndRetry<TException>(
	        Func<int, TimeSpan> retryDuration,
	        Action<Exception, TimeSpan> retryAction,
	        Action<CancellationToken> execFunc,
	        CancellationToken token)
	        where TException : Exception
        {
	        this.WaitAndRetry<TException>(this.MaxRetryAttempts, retryDuration, retryAction, execFunc, token);
        }

        /// <inheritdoc />
        public void WaitAndRetry<TException>(
	        int maxRetryCount,
	        Func<int, TimeSpan> retryDuration,
	        Action<Exception, TimeSpan> retryAction,
	        Action<CancellationToken> execFunc,
	        CancellationToken token)
	        where TException : Exception
        {
	        Policy.Handle<TException>(ex => !(ex is FileInfoInvalidPathException))
		        .WaitAndRetry(maxRetryCount, retryDuration, retryAction).Execute(execFunc, token);
        }

        /// <inheritdoc />
        public TResult WaitAndRetry<TResult>(
	        Func<Exception, bool> exceptionPredicate,
	        Func<int, TimeSpan> retryDuration,
	        Action<Exception, TimeSpan> retryAction,
	        Func<CancellationToken, TResult> execFunc,
	        CancellationToken token)
        {
	        return Policy.Handle(exceptionPredicate).WaitAndRetry(this.MaxRetryAttempts, retryDuration, retryAction)
		        .Execute(execFunc, token);
        }

        /// <inheritdoc />
        public TResult WaitAndRetry<TResult, TException>(
	        Func<int, TimeSpan> retryDuration,
	        Action<Exception, TimeSpan> retryAction,
	        Func<CancellationToken, TResult> execFunc,
	        CancellationToken token)
	        where TException : Exception
        {
	        return Policy.Handle<TException>().WaitAndRetry(this.MaxRetryAttempts, retryDuration, retryAction)
		        .Execute(execFunc, token);
        }

        /// <inheritdoc />
        public TResult WaitAndRetry<TResult, TException>(
	        int maxRetryCount,
	        Func<int, TimeSpan> retryDuration,
	        Action<Exception, TimeSpan> retryAction,
	        Func<CancellationToken, TResult> execFunc,
	        CancellationToken token)
	        where TException : Exception
        {
	        return Policy.Handle<TException>().WaitAndRetry(maxRetryCount, retryDuration, retryAction)
		        .Execute(execFunc, token);
        }
    }
}
