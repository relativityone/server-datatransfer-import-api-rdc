// <copyright file="ServiceAvailabilityChecker.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.WebApiVsKeplerSwitch
{
	using System;
	using System.Net;

	using Relativity.DataExchange.Logger;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models;
	using Relativity.Logging;
	using Relativity.Services.Exceptions;

	/// <inheritdoc/>
	public class ServiceAvailabilityChecker : IServiceAvailabilityChecker
	{
		private readonly IIAPICommunicationModeManager iApiCommunicationModeManager;
		private readonly ILog logger;

		private bool? isWebApiAvailable;
		private bool? isKeplerAvailable;
		private IAPICommunicationMode? iApiCommunicationMode;

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceAvailabilityChecker"/> class.
		/// </summary>
		/// <param name="iApiCommunicationModeManager">Import API communication mode service manager.</param>
		/// <param name="logger">Logger.</param>
		public ServiceAvailabilityChecker(IIAPICommunicationModeManager iApiCommunicationModeManager, ILog logger)
		{
			this.iApiCommunicationModeManager = iApiCommunicationModeManager;
			this.logger = logger ?? RelativityLogger.Instance;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceAvailabilityChecker"/> class.
		/// </summary>
		/// <param name="iApiCommunicationModeManager">Import API communication mode service manager.</param>
		public ServiceAvailabilityChecker(IIAPICommunicationModeManager iApiCommunicationModeManager)
			: this(iApiCommunicationModeManager, RelativityLogger.Instance)
		{
		}

		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "We catch general exception type to check if service is available. We are not interested in any specific exception type and we don't want to rethrow the exception.")]
		public bool IsWebApiAvailable()
		{
			// use local cache not to call service too often
			if (this.isWebApiAvailable.HasValue)
			{
				return this.isWebApiAvailable.Value;
			}

			// call WebApi service to check if available
			try
			{
				var relativityInstanceInfo = new RelativityInstanceInfo
				{
					Credentials = new NetworkCredential(),
					CookieContainer = new CookieContainer(),
					WebApiServiceUrl = new Uri(AppSettings.Instance.WebApiServiceUrl),
				};
				var relativityManager = new RelativityManagerService(relativityInstanceInfo);
				var relativityUrl = relativityManager.GetRelativityUrl();

				this.isWebApiAvailable = true;
			}
			catch (Exception ex)
			{
				this.logger?.LogError(ex, $"Error occurred when checking WebApi availability: {ex}");
				this.isWebApiAvailable = false;
			}

			return this.isWebApiAvailable.Value;
		}

		/// <inheritdoc/>
		public bool IsKeplerAvailable()
		{
			this.RefreshKeplerAvailabilityInfo();
			return this.isKeplerAvailable ?? false;
		}

		/// <inheritdoc/>
		public IAPICommunicationMode? ReadImportApiCommunicationMode()
		{
			this.RefreshKeplerAvailabilityInfo();
			return this.iApiCommunicationMode;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "We catch general exception type to check if service is available. We are not interested in any specific exception type and we don't want to rethrow the exception.")]
		private void RefreshKeplerAvailabilityInfo()
		{
			// use local cache not to call service too often
			if (this.isKeplerAvailable.HasValue && this.iApiCommunicationMode.HasValue)
			{
				return;
			}

			// call Kepler service to check if available and retrieve communication mode
			try
			{
				this.iApiCommunicationMode = this.iApiCommunicationModeManager.ReadImportApiCommunicationMode().GetAwaiter().GetResult();
				this.isKeplerAvailable = true;
			}
			catch (NotAuthorizedException)
			{
				// Before login to relativity there are few places where ManagerFactory is used and it checks if Kepler or WebApi should be used by calling Kepler service.
				// Because Kepler does not support anonymous calls we got NotAuthorizedException but even though we treat Kepler service as available.
				this.isKeplerAvailable = true;

				// In commit 2b6e09da4bc this behavior was changed and this function is not called before user is authenticated.
				this.logger?.LogWarning("NotAuthorizedException was thrown when reading communication mode.");
			}
			catch (Exception ex)
			{
				this.logger?.LogError(ex, $"Error occurred when checking Kepler availability: {ex}");
				this.isKeplerAvailable = false;
			}
		}
	}
}
