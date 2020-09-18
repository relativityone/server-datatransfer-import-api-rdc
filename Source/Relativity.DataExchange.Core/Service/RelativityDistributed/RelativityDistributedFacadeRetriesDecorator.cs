// <copyright file="RelativityDistributedFacadeRetriesDecorator.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.RelativityDistributed
{
	using System;
	using System.Threading;

	using Relativity.Logging;

	/// <summary>
	/// This decorated handles retryable exceptions which were thrown from the decorated <see cref="IRelativityDistributedFacade"/>.
	/// </summary>
	internal class RelativityDistributedFacadeRetriesDecorator : IRelativityDistributedFacade
	{
		private readonly IRelativityDistributedFacade relativityDistributedFacade;

		private readonly ILog logger;

		private readonly IAppSettings settings;

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityDistributedFacadeRetriesDecorator"/> class.
		/// </summary>
		/// <param name="logger">logger.</param>
		/// <param name="settings">Application settings.</param>
		/// <param name="relativityDistributedFacade">Decorated facade.</param>
		public RelativityDistributedFacadeRetriesDecorator(
			ILog logger,
			IAppSettings settings,
			IRelativityDistributedFacade relativityDistributedFacade)
		{
			this.logger = logger;
			this.relativityDistributedFacade = relativityDistributedFacade;

			this.settings = settings;
		}

		/// <inheritdoc/>
		public FileDownloadResponse DownloadFile(FileDownloadRequest request)
		{
			FileDownloadResponse response;
			int maxNumberOfTries = 1 + this.settings.HttpErrorNumberOfRetries;
			int tries = 0;
			do
			{
				tries += 1;

				response = this.relativityDistributedFacade.DownloadFile(request);

				if (response.IsSuccess)
				{
					return response;
				}

				if (!IsErrorRetryable(response.ErrorType))
				{
					this.logger.LogError(response.Exception, "A non-retryable error occured. Error type: {errorType}", response.ErrorType);
					return response;
				}

				this.logger.LogWarning(
					response.Exception,
					"Download Manager credentials failed.  Attempting to retry ({tries} of {maxNumberOfRetries}) in {waitTimeBeforeRetryInSeconds} seconds.",
					tries,
					maxNumberOfTries,
					this.settings.HttpErrorWaitTimeInSeconds);

				Thread.Sleep(TimeSpan.FromSeconds(this.settings.HttpErrorWaitTimeInSeconds));
			}
			while (tries < maxNumberOfTries);

			this.logger.LogError(response.Exception, "Error Downloading File: Unable to authenticate against Distributed server.");
			return response;
		}

		private static bool IsErrorRetryable(RelativityDistributedErrorType errorType)
		{
			return errorType == RelativityDistributedErrorType.InternalServerError
				   || errorType == RelativityDistributedErrorType.DataCorrupted;
		}
	}
}
