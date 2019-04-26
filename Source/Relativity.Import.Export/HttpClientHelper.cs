// ----------------------------------------------------------------------------
// <copyright file="HttpClientHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Threading.Tasks;

	/// <summary>
	/// Helper class for Http calls (eg: Kepler).
	/// </summary>
	public class HttpClientHelper
	{
		private const int DefTimeoutInSecs = 60;

		private readonly int serviceCallTimeoutInSecs;

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpClientHelper"/> class.
		/// object constructor.
		/// </summary>
		public HttpClientHelper()
			: this(DefTimeoutInSecs)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpClientHelper"/> class.
		/// object constructor.
		/// </summary>
		/// <param name="timeoutInSecs">timeout in secs set fot http call.</param>
		public HttpClientHelper(int timeoutInSecs)
		{
			if (timeoutInSecs == 0)
			{
				timeoutInSecs = DefTimeoutInSecs;
			}

			this.serviceCallTimeoutInSecs = timeoutInSecs;
		}

		/// <summary>
		/// It does Http Post to web service. It throws exception on returned status different than OK.
		/// </summary>
		/// <param name="queryUrl">http query url.</param>
		/// <param name="authorizationHeader">authentication header (Basic/Token).</param>
		/// <param name="content">http content.</param>
		/// <returns>Http Response Message on successful call.</returns>
		public async Task<HttpResponseMessage> DoPost(Uri queryUrl, string authorizationHeader, string content)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

			using (var client = new HttpClient())
			{
				// client.BaseAddress = new Uri(baseAddress);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Add("X-CSRF-Header", string.Empty);
				client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

				client.Timeout = TimeSpan.FromSeconds(this.serviceCallTimeoutInSecs);

				using (var stringContent = new StringContent(
					!string.IsNullOrEmpty(content) ? content : string.Empty,
					Encoding.UTF8,
					"application/json"))
				{
					return await client.PostAsync(queryUrl, stringContent);
				}
			}
		}
	}
}
