// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeplerServiceConnectionInfo.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object that contains Kepler service connection information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;
	using System.Net;
	using Relativity.Services.ServiceProxy;

	/// <summary>
	/// Represents a class object that contains Kepler service connection information.
	/// </summary>
	public class KeplerServiceConnectionInfo : IServiceConnectionInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="KeplerServiceConnectionInfo"/> class.
		/// </summary>
		/// <param name="webApiServiceUrl">
		/// The WebAPI service URL.
		/// </param>
		/// <param name="credential">
		/// The network credential.
		/// </param>
		public KeplerServiceConnectionInfo(Uri webApiServiceUrl, NetworkCredential credential)
		{
			webApiServiceUrl.ThrowIfNull(nameof(webApiServiceUrl));
			credential.ThrowIfNull(nameof(credential));
			if (!webApiServiceUrl.IsAbsoluteUri)
			{
				throw new ArgumentException(
					"The passed url is a relative url, while this function needs an absolute url to work correctly",
					nameof(webApiServiceUrl));
			}

			this.WebServiceBaseUrl = new Uri(webApiServiceUrl.GetLeftPart(UriPartial.Authority));
			if (string.IsNullOrEmpty(credential.UserName))
			{
				this.Credentials = new IntegratedAuthCredentials();
			}
			else if (string.Compare(credential.UserName, "XxX_BearerTokenCredentials_XxX", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (string.IsNullOrEmpty(credential.Password))
				{
					throw new ArgumentException("Bearer token should not be null or empty.");
				}

				this.Credentials = new BearerTokenCredentials(credential.Password);
			}
			else
			{
				this.Credentials = new UsernamePasswordCredentials(credential.UserName, credential.Password);
			}
		}

		/// <inheritdoc />
		public Credentials Credentials
		{
			get;
		}

		/// <inheritdoc />
		public Uri WebServiceBaseUrl
		{
			get;
		}
	}
}