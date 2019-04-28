// ----------------------------------------------------------------------------
// <copyright file="RelativityInstanceInfo.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Net;

	/// <summary>
	/// Represents a class object that contains the Relativity instance information. This class cannot be inherited.
	/// </summary>
	internal sealed class RelativityInstanceInfo
	{
		private Uri host;
		private Uri webApiServiceUrl;

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityInstanceInfo"/> class.
		/// </summary>
		public RelativityInstanceInfo()
		{
			this.CookieContainer = new CookieContainer();
			this.Credentials = null;
			this.Host = null;
			this.WebApiServiceUrl = null;
		}

		/// <summary>
		/// Gets or sets the cookie container.
		/// </summary>
		/// <value>
		/// The <see cref="CookieContainer"/> instance.
		/// </value>
		public CookieContainer CookieContainer
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the credentials used to authenticate to Relativity.
		/// </summary>
		/// <value>
		/// The <see cref="ICredentials"/> instance.
		/// </value>
		public ICredentials Credentials
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity host URL.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		public Uri Host
		{
			get
			{
				string url = this.host != null ? this.host.ToString() : null;
				if (string.IsNullOrEmpty(url) && this.WebApiServiceUrl != null)
				{
					// This is a safe assumption.
					url = this.WebApiServiceUrl.GetLeftPart(UriPartial.Authority);
				}

				if (string.IsNullOrEmpty(url))
				{
					return null;
				}

				url = AppendTrailingSlash(url);
				return new Uri(url);
			}

			set
			{
				this.host = value;
			}
		}

		/// <summary>
		/// Gets or sets the Relativity Web API service URL. If this is not specified, the URL is formed by combining <see cref="Host"/> with <c>RelativityWebApi</c>.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		public Uri WebApiServiceUrl
		{
			get
			{
				string url = this.webApiServiceUrl != null ? this.webApiServiceUrl.ToString() : null;
				if (string.IsNullOrEmpty(url))
				{
					return null;
				}

				url = AppendTrailingSlash(url);
				return new Uri(url);
			}

			set
			{
				this.webApiServiceUrl = value;
			}
		}

		private static string AppendTrailingSlash(string value)
		{
			if (value != null &&
			    value.LastIndexOf('/') != value.Length - 1)
			{
				value += '/';
			}

			return value;
		}
	}
}