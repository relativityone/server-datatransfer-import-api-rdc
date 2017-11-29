using System;
using Polly;
using Relativity.Logging;

namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Represents a wait and retry policy class objects with a default back-off time strategy.
    /// </summary>
    public class WaitAndRetryPolicy : IWaitAndRetryPolicy
    {
        private const int _DEFAULT_NUMBER_OF_RETRIES = 1;
        private const int _DEFAULT_WAIT_TIME_SECONDS_BETWEEN_RETRY_ATTEMPTS = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitAndRetryPolicy"/> class.
        /// </summary>
        public WaitAndRetryPolicy(int numberOfRetries, int waitTimeSecondsBetweenRetryAttempts)
        {
            NumberOfRetries = numberOfRetries;
            WaitTimeSecondsBetweenRetryAttempts = waitTimeSecondsBetweenRetryAttempts;
        }

        /// <inheritdoc />
        public WaitAndRetryPolicy()
            : this(_DEFAULT_NUMBER_OF_RETRIES, _DEFAULT_WAIT_TIME_SECONDS_BETWEEN_RETRY_ATTEMPTS)
        {
        }

        /// <inheritdoc />
        public int NumberOfRetries { get; }
        /// <inheritdoc />
        public int WaitTimeSecondsBetweenRetryAttempts { get; }

        /// <inheritdoc />
        public void WaitAndRetry<TException>(Func<int, TimeSpan> retryDuration, Action<Exception, TimeSpan> retryAction, Action execFunc) where TException : Exception
        {
            WaitAndRetry<TException>(this.NumberOfRetries, retryDuration, retryAction, execFunc);
        }

        /// <inheritdoc />
        public void WaitAndRetry<TException>(int maxRetryCount, Func<int, TimeSpan> retryDuration, Action<Exception, TimeSpan> retryAction, Action execFunc) where TException : Exception
        {
            Policy.Handle<TException>()
                .WaitAndRetry(maxRetryCount, retryDuration, retryAction)
                .Execute(execFunc);
        }
    }
}
