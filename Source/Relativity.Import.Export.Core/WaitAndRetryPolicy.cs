// --------------------------------------------------------------------------------------------------------------------
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
    internal class WaitAndRetryPolicy : IWaitAndRetryPolicy
    {
		/// <summary>
		/// The cached application settings.
		/// </summary>
		private readonly AppSettingsDto cachedAppSettings;

		/// <summary>
		/// Initializes a new instance of the <see cref="WaitAndRetryPolicy"/> class.
		/// </summary>
		public WaitAndRetryPolicy()
		    : this(AppSettings.Instance)
	    {
	    }

	    /// <summary>
		/// Initializes a new instance of the <see cref="WaitAndRetryPolicy"/> class.
		/// </summary>
		/// <param name="settings">
		/// The application settings.
		/// </param>
		public WaitAndRetryPolicy(IAppSettings settings)
        {
	        if (settings == null)
	        {
				throw new ArgumentNullException(nameof(settings));
	        }

	        this.cachedAppSettings = settings.DeepCopy();
        }

        /// <inheritdoc />
        public void WaitAndRetry<TException>(
	        Func<int, TimeSpan> retryDuration,
	        Action<Exception, TimeSpan> retryAction,
	        Action<CancellationToken> execFunc,
	        CancellationToken token)
	        where TException : Exception
        {
	        this.WaitAndRetry<TException>(
		        this.cachedAppSettings.IoErrorNumberOfRetries,
		        retryDuration,
		        retryAction,
		        execFunc,
		        token);
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
	        return Policy.Handle(exceptionPredicate).WaitAndRetry(
		        this.cachedAppSettings.IoErrorNumberOfRetries,
		        retryDuration,
		        retryAction).Execute(execFunc, token);
        }

        /// <inheritdoc />
        public TResult WaitAndRetry<TResult, TException>(
	        Func<int, TimeSpan> retryDuration,
	        Action<Exception, TimeSpan> retryAction,
	        Func<CancellationToken, TResult> execFunc,
	        CancellationToken token)
	        where TException : Exception
        {
	        return Policy.Handle<TException>()
		        .WaitAndRetry(this.cachedAppSettings.IoErrorNumberOfRetries, retryDuration, retryAction)
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
