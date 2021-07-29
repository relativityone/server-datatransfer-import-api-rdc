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
	using System.Security;

	using Relativity.Services.ServiceProxy;

	/// <summary>
	/// Represents a class object that contains Kepler service connection information.
	/// </summary>
	public class KeplerServiceConnectionInfo : IServiceConnectionInfo
	{
		private readonly NetworkCredential originalCredentials;
		private SecureString currentPassword;

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
			this.originalCredentials = credential;
			this.currentPassword = credential.SecurePassword;
			this.Credentials = ConvertCredentials(credential);
		}

		/// <inheritdoc />
		public Credentials Credentials
		{
			get;
			private set;
		}

		/// <inheritdoc />
		public Uri WebServiceBaseUrl
		{
			get;
		}

		/// <inheritdoc/>
		public void UpdateCredentials(NetworkCredential credential)
		{
			if (credential == null)
			{
				throw new ArgumentNullException(nameof(credential));
			}

			this.originalCredentials.SecurePassword = credential.SecurePassword;
			this.RefreshCredentials();
		}

		/// <inheritdoc/>
		public bool RefreshCredentials()
		{
			bool IsPasswordEqual(NetworkCredential credentials, SecureString expectedPassword)
			{
				var credentialWithExpectedPassword = new NetworkCredential { SecurePassword = expectedPassword };
				return credentials.Password == credentialWithExpectedPassword.Password;
			}

			if (IsPasswordEqual(this.originalCredentials, this.currentPassword))
			{
				return false;
			}

			this.Credentials = ConvertCredentials(this.originalCredentials);
			this.currentPassword = this.originalCredentials.SecurePassword;

			return true;
		}

		private static Credentials ConvertCredentials(NetworkCredential credential)
		{
			if (string.Compare(credential.UserName, "XxX_BearerTokenCredentials_XxX", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (string.IsNullOrEmpty(credential.Password))
				{
					throw new ArgumentException("Bearer token should not be null or empty.");
				}

				return new BearerTokenCredentials(credential.Password);
			}
			else
			{
				return new UsernamePasswordCredentials(credential.UserName, credential.Password);
			}
		}
	}
}