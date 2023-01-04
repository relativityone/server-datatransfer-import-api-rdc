// <copyright file="IAPICommunicationModeManager.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.WebApiVsKeplerSwitch
{
	using System;
	using System.Threading.Tasks;

	using Polly;
	using Polly.Retry;

	using Relativity.DataExchange.Logger;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models;
	using Relativity.Kepler.Exceptions;
	using Relativity.Logging;

	/// <inheritdoc/>
	public class IAPICommunicationModeManager : IIAPICommunicationModeManager
	{
		private const int DefaultRetryCount = 1;
		private const int DefaultSleepDurationTimeInSeconds = 1;

		private const string GetRelativityVersionUrl = "/Relativity.Rest/api/Relativity.Services.InstanceDetails.IInstanceDetailsModule/InstanceDetailsService/GetRelativityVersionAsync";

		private readonly IKeplerProxy keplerProxy;
		private readonly IAppSettings settings;
		private readonly ILog logger;
		private readonly Func<string> correlationIdFunc;

		/// <summary>
		/// Initializes a new instance of the <see cref="IAPICommunicationModeManager"/> class.
		/// </summary>
		/// <param name="keplerProxy">Kepler proxy.</param>
		/// <param name="correlationIdFunc">Function retrieving correlation id related with Kepler request.</param>
		/// <param name="settings">Application settings.</param>
		/// <param name="logger">Logger.</param>
		public IAPICommunicationModeManager(IKeplerProxy keplerProxy, Func<string> correlationIdFunc, IAppSettings settings, ILog logger)
		{
			this.keplerProxy = keplerProxy;
			this.correlationIdFunc = correlationIdFunc;
			this.settings = settings;
			this.logger = logger ?? RelativityLogger.Instance;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IAPICommunicationModeManager"/> class.
		/// </summary>
		/// <param name="keplerProxy">Kepler proxy.</param>
		/// <param name="correlationIdFunc">Function retrieving correlation id related with Kepler request.</param>
		public IAPICommunicationModeManager(IKeplerProxy keplerProxy, Func<string> correlationIdFunc)
			: this(keplerProxy, correlationIdFunc, AppSettings.Instance, RelativityLogger.Instance)
		{
		}

		/// <inheritdoc/>
		public async Task<IAPICommunicationMode> ReadImportApiCommunicationMode()
		{
			Version currentVersion = null;
			RetryPolicy retryPolicy = this.GetRetryPolicy();
			try
			{
				string currentRelativityVersionString = await this.keplerProxy.ExecutePostAsync(GetRelativityVersionUrl, string.Empty).ConfigureAwait(false);
				currentRelativityVersionString = currentRelativityVersionString.Replace("\"", string.Empty);
				currentVersion = new Version(currentRelativityVersionString);
			}
			catch (Exception e)
			{
				this.logger.LogWarning("Could not retrieve current Relativity Version - failed with exception: {e.Message}", e.Message);
				return await GetIAPICommunicationMode(retryPolicy).ConfigureAwait(false);
			}

			if (currentVersion.CompareTo(VersionConstants.PrairieSmokeVersion) >= 0)
			{
				return await GetIAPICommunicationMode(retryPolicy).ConfigureAwait(false);
			}

			return IAPICommunicationMode.ForceWebAPI;
		}

		private Task<IAPICommunicationMode> GetIAPICommunicationMode(RetryPolicy retryPolicy)
		{
			return retryPolicy.ExecuteAsync(
				       () => this.keplerProxy.ExecuteAsync(
					       async serviceProxyFactory =>
						       {
							       using (var service =
								       serviceProxyFactory.CreateProxyInstance<IIAPICommunicationModeService>())
							       {
								       return await service.GetIAPICommunicationModeAsync(this.correlationIdFunc?.Invoke())
									              .ConfigureAwait(false);
							       }
						       }));
		}

		private RetryPolicy GetRetryPolicy()
		{
			var maxRetryCount = this.settings?.ReadCommunicationModeErrorNumberOfRetries ?? DefaultRetryCount;
			var sleepDurationTime = TimeSpan.FromSeconds(this.settings?.ReadCommunicationModeErrorWaitTimeInSeconds ?? DefaultSleepDurationTimeInSeconds);

			return Policy
				.Handle<ServiceNotFoundException>()
				.WaitAndRetryAsync(
					maxRetryCount,
					i => sleepDurationTime,
					(exception, timeSpan, retryCount, context) =>
						{
							this.logger?.LogWarning(
								"Service connection failed. Retry policy triggered... Attempt #{retryCount} out of {maxRetryCount}. Wait time between retries: {waitTime}",
								retryCount,
								maxRetryCount,
								sleepDurationTime);
						});
		}
	}
}
