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

	using Relativity.Services.Pipeline;

	/// <summary>
	/// Represents a class object to create Kepler-based web service proxy instances.
	/// </summary>
	internal class KeplerServiceProxyFactory : IServiceProxyFactory
	{
		private readonly IServiceConnectionInfo serviceConnectionInfo;
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
		{
			this.serviceConnectionInfo = connectionInfo.ThrowIfNull(nameof(connectionInfo));
			this.serviceFactory = new Lazy<KeplerServiceFactory>(this.CreateServiceFactory);
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