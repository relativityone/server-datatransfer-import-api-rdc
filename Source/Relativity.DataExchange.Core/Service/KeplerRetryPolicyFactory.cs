// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeplerRetryPolicyFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to create Kepler-based web service proxy instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;
	using System.Net.Http;

	using Polly;

	/// <summary>
	/// Represents a class object to create Kepler-based web service proxy instances.
	/// </summary>
	internal class KeplerRetryPolicyFactory : IServiceRetryPolicyFactory
	{
		private readonly IAppSettings settings;

		/// <summary>
		/// Initializes a new instance of the <see cref="KeplerRetryPolicyFactory"/> class.
		/// </summary>
		/// <param name="settings">
		/// The application settings.
		/// </param>
		public KeplerRetryPolicyFactory(IAppSettings settings)
		{
			this.settings = settings.ThrowIfNull(nameof(settings));
		}

		/// <inheritdoc />
		public ISyncPolicy CreatePolicy(Action<Exception, TimeSpan, int, Context> onRetry)
		{
			return Policy.Wrap(
				Policy.Handle<HttpRequestException>().WaitAndRetry(
					this.settings.HttpErrorNumberOfRetries,
					(retryCount, context) => TimeSpan.FromSeconds(this.settings.HttpErrorWaitTimeInSeconds),
					onRetry),
				Policy.Handle<Exception>(IsRetryableException).WaitAndRetry(
					this.settings.HttpErrorNumberOfRetries,
					retryAttempt => TimeSpan.FromSeconds(this.settings.HttpErrorWaitTimeInSeconds),
					onRetry));
		}

		/// <inheritdoc />
		public IAsyncPolicy CreateAsyncPolicy(Action<Exception, TimeSpan, int, Context> onRetry)
		{
			return Policy.WrapAsync(
				Policy.Handle<HttpRequestException>().WaitAndRetryAsync(
					this.settings.HttpErrorNumberOfRetries,
					(retryCount, context) => TimeSpan.FromSeconds(this.settings.HttpErrorWaitTimeInSeconds),
					onRetry),
				Policy.Handle<Exception>(IsRetryableException).WaitAndRetryAsync(
					this.settings.HttpErrorNumberOfRetries,
					retryAttempt => TimeSpan.FromSeconds(this.settings.HttpErrorWaitTimeInSeconds),
					onRetry));
		}

		private static bool IsRetryableException(Exception exception)
		{
			return !ExceptionHelper.IsFatalException(exception) && !ExceptionHelper.IsFatalKeplerException(exception)
			                                                    && !(exception is OperationCanceledException);
		}
	}
}