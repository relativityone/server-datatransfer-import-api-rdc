// <copyright file="WebApiVsKepler.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.WebApiVsKeplerSwitch
{
	using Relativity.DataExchange.Logger;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models;
	using Relativity.Logging;
	using Relativity.Services.Exceptions;

	/// <summary>
	/// Toggle which provides methods to determine what service implementation (Kepler or WebApi) should be used.
	/// </summary>
	public class WebApiVsKepler : IWebApiVsKepler
	{
		private readonly IServiceAvailabilityChecker serviceAvailabilityChecker;
		private readonly ILog logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebApiVsKepler"/> class.
		/// </summary>
		/// <param name="serviceAvailabilityChecker">WebApi and Kepler service availability checker.</param>
		/// <param name="logger">Logger.</param>
		public WebApiVsKepler(IServiceAvailabilityChecker serviceAvailabilityChecker, ILog logger)
        {
            this.serviceAvailabilityChecker = serviceAvailabilityChecker;
            this.logger = logger ?? RelativityLogger.Instance;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="WebApiVsKepler"/> class.
		/// </summary>
		/// <param name="serviceAvailabilityChecker">WebApi and Kepler service availability checker.</param>
		public WebApiVsKepler(IServiceAvailabilityChecker serviceAvailabilityChecker)
			: this(serviceAvailabilityChecker, RelativityLogger.Instance)
		{
		}

		/// <summary>
		/// Determines if Kepler service should be used instead of WebApi service.
		/// </summary>
		/// <returns>Boolean flag indicating if Kepler service should be used.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "'WebApi' and 'Kepler' tokens are correct.")]
		public bool UseKepler()
	    {
		    var isKeplerAvailable = this.serviceAvailabilityChecker.IsKeplerAvailable();
		    if (isKeplerAvailable)
		    {
			    var useKepler = this.GetKeplerUsageByCommunicationModeAndAppSettings();
			    if (useKepler)
			    {
				    return true;
			    }
		    }

		    // Kepler service not available or should not be used
		    var isWebApiAvailable = this.serviceAvailabilityChecker.IsWebApiAvailable();
		    if (isWebApiAvailable)
		    {
			    return false;
		    }

		    // WebApi not available anymore (decommissioned), Kepler available but disabled by settings - log warning and use Kepler in this case.
		    if (isKeplerAvailable)
		    {
			    this.logger?.LogWarning("WebApi service not available. Kepler service will be used.");
			    return true;
		    }

		    // No WebApi nor Kepler available
		    var errorMessage = "No WebApi nor Kepler service found to perform the request.";
		    this.logger?.LogError(errorMessage);
		    throw new NotFoundException(errorMessage);
	    }

		private bool GetKeplerUsageByCommunicationModeAndAppSettings()
	    {
	        var iApiCommunicationMode = this.serviceAvailabilityChecker.ReadImportApiCommunicationMode();
	        var keplerUsageLocalSettingsValue = AppSettings.Instance.UseKepler;

	        // override local settings if 'ForceKepler'
	        if (iApiCommunicationMode == IAPICommunicationMode.ForceKepler)
	        {
	            return true;
	        }

	        // override local settings if 'ForceWebAPI'
	        if (iApiCommunicationMode == IAPICommunicationMode.ForceWebAPI)
	        {
	            return false;
	        }

	        // use local settings if specified
	        if (keplerUsageLocalSettingsValue.HasValue)
	        {
	            return keplerUsageLocalSettingsValue.Value;
	        }

	        // use instance settings if local settings not specified
	        if (iApiCommunicationMode == IAPICommunicationMode.Kepler)
	        {
	            return true;
	        }

	        // use instance settings if local settings not specified
	        if (iApiCommunicationMode == IAPICommunicationMode.WebAPI)
	        {
	            return false;
	        }

	        // Kepler endpoint available but user is not authorized yet. Even tough we should use Kepler service to be used when user is authorized.
	        return true;
	    }
	}
}