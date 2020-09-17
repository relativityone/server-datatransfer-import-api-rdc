// <copyright file="RelativityDistributedFacadeAuthenticationDecorator.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.RelativityDistributed
{
	using Relativity.Logging;

	/// <summary>
	/// This types handles authentication errors which were thrown from the decorated <see cref="IRelativityDistributedFacade"/>.
	/// </summary>
	internal class RelativityDistributedFacadeAuthenticationDecorator : IRelativityDistributedFacade
	{
		private readonly IRelativityDistributedFacade relativityDistributedFacade;

		private readonly ILog logger;
		private readonly IAppSettings settings;
		private readonly IReLoginService reLoginService;

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityDistributedFacadeAuthenticationDecorator"/> class.
		/// </summary>
		/// <param name="logger">logger.</param>
		/// <param name="settings">Application settings.</param>
		/// <param name="reLoginService">ReLogin service.</param>
		/// <param name="relativityDistributedFacade">Decorated facade.</param>
		public RelativityDistributedFacadeAuthenticationDecorator(
			ILog logger,
			IAppSettings settings,
			IReLoginService reLoginService,
			IRelativityDistributedFacade relativityDistributedFacade)
		{
			this.logger = logger;
			this.settings = settings;
			this.reLoginService = reLoginService;

			this.relativityDistributedFacade = relativityDistributedFacade;
		}

		/// <inheritdoc/>
		public FileDownloadResponse DownloadFile(FileDownloadRequest request)
		{
			FileDownloadResponse response;
			int maxNumberOfTries = 1 + this.settings.MaxReloginTries;
			int tries = 0;
			do
			{
				tries += 1;

				if (tries > 1)
				{
					this.reLoginService.AttemptReLogin();
				}

				response = this.relativityDistributedFacade.DownloadFile(request);

				if (response.IsSuccess || response.ErrorType != RelativityDistributedErrorType.Authentication)
				{
					return response;
				}

				this.logger.LogWarning(response.Exception, "Download Manager credentials failed.  Attempting to re-login ({tries} of {maxReLoginTries})", tries, maxNumberOfTries);
			}
			while (tries < maxNumberOfTries);

			this.logger.LogError(response.Exception, "Error Downloading File: Unable to authenticate against Distributed server.");
			return response;
		}
	}
}