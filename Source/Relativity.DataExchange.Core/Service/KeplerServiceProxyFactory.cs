// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeplerServiceProxyFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to create Kepler-based web service proxy instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;

	using Relativity.Logging;
	using Relativity.Services.Pipeline;

	/// <summary>
	/// Represents a class object to create Kepler-based web service proxy instances.
	/// </summary>
	internal class KeplerServiceProxyFactory : IServiceProxyFactory
	{
		private readonly IServiceConnectionInfo serviceConnectionInfo;
		private readonly ILog logger;
		private readonly IAppSettings appSettings;
		private Lazy<KeplerServiceFactory> serviceFactory;
		private HttpClientHandler httpClientHandler;
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="KeplerServiceProxyFactory"/> class.
		/// </summary>
		/// <param name="connectionInfo">
		/// The service connection information.
		/// </param>
		public KeplerServiceProxyFactory(IServiceConnectionInfo connectionInfo)
		 : this(connectionInfo, Relativity.DataExchange.Logger.RelativityLogger.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KeplerServiceProxyFactory"/> class.
		/// </summary>
		/// <param name="connectionInfo"> The service connection information. </param>
		/// <param name="log"> Logger. </param>
		public KeplerServiceProxyFactory(IServiceConnectionInfo connectionInfo, ILog log)
		 : this(connectionInfo, log, Relativity.DataExchange.AppSettings.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KeplerServiceProxyFactory"/> class.
		/// </summary>
		/// <param name="connectionInfo"> The service connection information. </param>
		/// <param name="appSettings"> Application settings. </param>
		public KeplerServiceProxyFactory(IServiceConnectionInfo connectionInfo, IAppSettings appSettings)
			: this(connectionInfo, Relativity.DataExchange.Logger.RelativityLogger.Instance, appSettings)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KeplerServiceProxyFactory"/> class.
		/// </summary>
		/// <param name="connectionInfo"> The service connection information. </param>
		/// <param name="log"> Logger. </param>
		/// <param name="appSettings"> Application settings. </param>
		public KeplerServiceProxyFactory(IServiceConnectionInfo connectionInfo, ILog log, IAppSettings appSettings)
		{
			this.serviceConnectionInfo = connectionInfo.ThrowIfNull(nameof(connectionInfo));
			this.serviceFactory = new Lazy<KeplerServiceFactory>(this.CreateServiceFactory);
			this.logger = log;
			this.appSettings = appSettings;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <inheritdoc />
		public TProxy CreateProxyInstance<TProxy>()
			where TProxy : class, IDisposable
		{
			ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

			if (this.serviceConnectionInfo.RefreshCredentials())
			{
				this.serviceFactory = new Lazy<KeplerServiceFactory>(this.CreateServiceFactory);
			}

			return this.serviceFactory.Value.GetClient<TProxy>();
		}

		/// <inheritdoc/>
		public void UpdateCredentials(NetworkCredential credential)
		{
			this.serviceConnectionInfo.UpdateCredentials(credential);
			this.serviceFactory = new Lazy<KeplerServiceFactory>(this.CreateServiceFactory);
		}

		/// <inheritdoc/>
		public Task<string> ExecutePostAsync(string endpointAddress, string body)
		{
			var instanceInfo = new RelativityInstanceInfo()
				                   {
					                   Credentials = this.serviceConnectionInfo.NetworkCredential,
					                   CookieContainer = new CookieContainer(),
					                   WebApiServiceUrl = new Uri(AppSettings.Instance.WebApiServiceUrl),
				                   };
			var client = new RestClient(instanceInfo, this.logger, this.appSettings.HttpTimeoutSeconds, this.appSettings.HttpErrorNumberOfRetries);
			using (var token = new System.Threading.CancellationTokenSource())
			{
				return client.RequestPostStringAsync(
					endpointAddress,
					body,
					(retryAttempt) => TimeSpan.FromSeconds(this.appSettings.HttpErrorWaitTimeInSeconds),
					(exception, timespan, context) =>
						{
							this.logger.LogError(
								exception,
								"Retry - {Timespan} - Failed to call Relativity endpoint.",
								timespan);
						},
					(code) => "query Relativity endpoint ",
					(code) => "Error while calling Relativity endpoint.",
					token.Token);
			}
		}

		private KeplerServiceFactory CreateServiceFactory()
		{
			var keplerSettings = new KeplerServiceFactorySettings(
				new Uri(this.serviceConnectionInfo.WebServiceBaseUrl, "/Relativity.Rest/api"),
				this.serviceConnectionInfo.Credentials.GetAuthenticationHeaderValue(),
				WireProtocolVersion.V2,
				this.httpClientHandler);
			return new KeplerServiceFactory(keplerSettings);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				if (this.httpClientHandler != null)
				{
					this.httpClientHandler.Dispose();
					this.httpClientHandler = null;
				}

				this.disposed = true;
			}
		}
	}
}