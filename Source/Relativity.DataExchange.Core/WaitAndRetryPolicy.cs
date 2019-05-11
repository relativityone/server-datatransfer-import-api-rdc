﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitAndRetryPolicy.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to support resiliency and retry policies.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Threading;

	using Polly;

	using Relativity.DataExchange.Io;

	/// <summary>
	/// Represents a wait and retry policy class objects with a default back-off time strategy.
	/// </summary>
	internal class WaitAndRetryPolicy : IWaitAndRetryPolicy
	{
		/// <summary>
		/// The application settings.
		/// </summary>
		private readonly IAppSettings appSettings;

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

			this.appSettings = settings;
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
				this.appSettings.IoErrorNumberOfRetries,
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
				this.appSettings.IoErrorNumberOfRetries,
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
				.WaitAndRetry(this.appSettings.IoErrorNumberOfRetries, retryDuration, retryAction)
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