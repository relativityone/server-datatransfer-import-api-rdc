// ----------------------------------------------------------------------------
// <copyright file="ServiceHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Net;

	using Relativity.Services.ServiceProxy;

	/// <summary>
	/// Defines static helper methods to retrieve Relativity services.
	/// </summary>
	public static class ServiceHelper
	{
		/// <summary>
		/// The maximum number of query manager items to fetch.
		/// </summary>
		public const int MaxItemsToFetch = 5000;

		/// <summary>
		/// Retrieve a service proxy.
		/// </summary>
		/// <typeparam name="T">
		/// The type of service to retrieve.
		/// </typeparam>
		/// <param name="parameters">
		/// The integration test parameters.
		/// </param>
		/// <returns>
		/// The <typeparamref name="T"/> instance.
		/// </returns>
		public static T GetServiceProxy<T>(IntegrationTestParameters parameters)
			where T : class, IDisposable
		{
			return CreateServiceProxy<T>(parameters, CreateServiceFactorySettingsForKepler);
		}

		/// <summary>
		/// Retrieve a service proxy.
		/// </summary>
		/// <typeparam name="T">
		/// The type of service to retrieve.
		/// </typeparam>
		/// <param name="parameters">
		/// The integration test parameters.
		/// </param>
		/// <returns>
		/// The <typeparamref name="T"/> instance.
		/// </returns>
		/// <remarks>See
		/// <see href="https://platform.relativity.com/RelativityOne/index.htm#What_s_new/RSAPI_deprecation_process.htm">RSAPI deprecation process</see>
		/// page for a API migration matrix.</remarks>
		[Obsolete("RSAPI is deprecated, use Kepler instead.")]
		public static T GetRSAPIServiceProxy<T>(IntegrationTestParameters parameters)
			where T : class, IDisposable
		{
			return CreateServiceProxy<T>(parameters, CreateServiceFactorySettingsForRSAPI);
		}

		private static T CreateServiceProxy<T>(
			IntegrationTestParameters parameters,
			Func<IntegrationTestParameters, Credentials, ServiceFactorySettings> serficeFactorySettingsProvider)
			where T : class, IDisposable
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			Credentials credentials = new UsernamePasswordCredentials(parameters.RelativityUserName, parameters.RelativityPassword);

			ServiceFactorySettings serviceFactorySettings = serficeFactorySettingsProvider(parameters, credentials);
			serviceFactorySettings.ProtocolVersion = Services.Pipeline.WireProtocolVersion.V2;

			ServiceFactory serviceFactory = new ServiceFactory(serviceFactorySettings);
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls
																			 | SecurityProtocolType.Tls11
																			 | SecurityProtocolType.Tls12;
			T proxy = serviceFactory.CreateProxy<T>();
			return proxy;
		}

		private static ServiceFactorySettings CreateServiceFactorySettingsForKepler(
			IntegrationTestParameters parameters,
			Credentials credentials)
		{
			return new ServiceFactorySettings(parameters.RelativityRestUrl, credentials);
		}

		[Obsolete("RSAPI is deprecated, use Kepler instead.")]
		private static ServiceFactorySettings CreateServiceFactorySettingsForRSAPI(
			IntegrationTestParameters parameters,
			Credentials credentials)
		{
			return new ServiceFactorySettings(parameters.RelativityServicesUrl, parameters.RelativityRestUrl, credentials);
		}
	}
}