// ----------------------------------------------------------------------------
// <copyright file="BatchInProgressErrorPolicyFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;
	using System.Threading.Tasks;
	using Polly;
	using Relativity.Logging;

	/// <inheritdoc />
	public class BatchInProgressErrorPolicyFactory : IKeplerRetryPolicyFactory
	{
		private readonly IAppSettings settings;

		private readonly ILog logger;
		private readonly Action<Exception, TimeSpan, int, Context> onRetry;

		/// <summary>
		/// Initializes a new instance of the <see cref="BatchInProgressErrorPolicyFactory"/> class.
		/// </summary>
		/// <param name="settings">App settings.</param>
		/// <param name="logger">Relativity logger.</param>
		public BatchInProgressErrorPolicyFactory(IAppSettings settings, ILog logger)
		{
			this.settings = settings;
			this.logger = logger;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BatchInProgressErrorPolicyFactory"/> class.
		/// </summary>
		/// <param name="settings">App settings.</param>
		/// <param name="logger">Relativity logger.</param>
		/// <param name="onRetry">Custom onRetry event logic.</param>
		public BatchInProgressErrorPolicyFactory(IAppSettings settings, ILog logger, Action<Exception, TimeSpan, int, Context> onRetry)
		{
			this.settings = settings;
			this.logger = logger;
			this.onRetry = onRetry;
		}

		/// <inheritdoc />
		public IAsyncPolicy CreateRetryPolicy()
		{
			return Policy
				.Handle((Func<Exception, bool>)this.IsRetryableException)
				.WaitAndRetryAsync(this.settings.BatchInProgressNumberOfRetries, retryAttempt => TimeSpan.FromSeconds(this.settings.BatchInProgressWaitTimeInSeconds), this.OnRetry);
		}

		/// <inheritdoc />
		public IAsyncPolicy<T> CreateRetryPolicy<T>()
		{
			return Policy<T>
				.Handle((Func<Exception, bool>)this.IsRetryableException)
				.WaitAndRetryAsync(this.settings.BatchInProgressNumberOfRetries, retryAttempt => TimeSpan.FromSeconds(this.settings.BatchInProgressWaitTimeInSeconds), this.OnRetry);
		}

		private bool IsRetryableException(Exception exception)
		{
			bool isBatchInProgressException = ExceptionHelper.IsBatchInProgressException(exception);
			bool isCancelException = exception.GetType() == typeof(TaskCanceledException);
			return isCancelException || isBatchInProgressException;
		}

		private Task OnRetry<TResult>(DelegateResult<TResult> result, TimeSpan timeSpan, int retryCount, Context context)
		{
			return this.OnRetry(result.Exception, timeSpan, retryCount, context);
		}

		private Task OnRetry(Exception exception, TimeSpan duration, int retryCount, Context context)
		{
			if (this.onRetry == null)
			{
				this.logger.LogWarning(
					exception,
					"BatchInProgressErrorPolicyFactory: Call to Kepler service failed due to {ExceptionType}. Currently on attempt {RetryCount} out of {MaxRetries} and waiting {WaitSeconds} seconds before the next retry attempt.",
					exception.GetType(),
					retryCount,
					this.settings.BatchInProgressNumberOfRetries,
					duration.TotalSeconds);
			}
			else
			{
				this.onRetry.Invoke(exception, duration, retryCount, context);
			}

			return Task.CompletedTask;
		}
	}
}