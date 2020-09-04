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

	using Relativity.Services.ServiceProxy;

	/// <summary>
	/// Represents a class object to create Kepler-based web service proxy instances.
	/// </summary>
	internal class KeplerServiceProxyFactory : IServiceProxyFactory
	{
		private readonly IServiceConnectionInfo serviceConnectionInfo;
		private readonly Lazy<ServiceFactory> serviceFactory;
		private HttpClientHandler httpClientHandler;
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="KeplerServiceProxyFactory"/> class.
		/// </summary>
		/// <param name="connectionInfo">
		/// The service connection information.
		/// </param>
		public KeplerServiceProxyFactory(IServiceConnectionInfo connectionInfo)
		{
			this.serviceConnectionInfo = connectionInfo.ThrowIfNull(nameof(connectionInfo));
			this.serviceFactory = new Lazy<ServiceFactory>(this.CreateServiceFactory);
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
			return this.serviceFactory.Value.CreateProxy<TProxy>();
		}

		private ServiceFactory CreateServiceFactory()
		{
			// The Relativity.Services URL should require a separate configurable base URL but RSAPI isn't used.
			this.httpClientHandler = new HttpClientHandler();
			ServiceFactorySettings factorySettings = new ServiceFactorySettings(
				new Uri(this.serviceConnectionInfo.WebServiceBaseUrl, "/Relativity.Rest/api"),
				this.serviceConnectionInfo.Credentials,
				this.httpClientHandler);
			return new ServiceFactory(factorySettings);
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