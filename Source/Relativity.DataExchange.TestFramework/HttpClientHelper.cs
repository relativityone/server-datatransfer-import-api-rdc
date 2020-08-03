// ----------------------------------------------------------------------------
// <copyright file="HttpClientHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Net.Http;
	using System.Text;
	using System.Threading.Tasks;

	/// <summary>
	/// Defines static helper methods to call Relativity web services.
	/// </summary>
	public static class HttpClientHelper
	{
		/// <summary>
		/// Sends POST request to specified Uri using Relativity username and password for authorization.
		/// </summary>
		/// <param name="parameters">Test parameters with Relativity username and password used for authorization.</param>
		/// <param name="absoluteUri">Absolute web service Uri.</param>
		/// <param name="requestJson">Request string in JSON format.</param>
		/// <returns>Result returned by web service.</returns>
		public static async Task<string> PostAsync(
			IntegrationTestParameters parameters,
			Uri absoluteUri,
			string requestJson)
		{
			var content = new StringContent(
				requestJson,
				Encoding.UTF8,
				"application/json");

			return await HttpRequest(
						   parameters,
						   absoluteUri,
						   client => client.PostAsync(absoluteUri, content))
					   .ConfigureAwait(false);
		}

		/// <summary>
		/// Sends GET request to specified Uri using Relativity username and password for authorization.
		/// </summary>
		/// <param name="parameters">Test parameters with Relativity username and password used for authorization.</param>
		/// <param name="absoluteUri">Absolute web service Uri.</param>
		/// <returns>Result returned by web service.</returns>
		public static async Task<string> GetAsync(IntegrationTestParameters parameters, Uri absoluteUri)
		{
			return await HttpRequest(
						   parameters,
						   absoluteUri,
						   client => client.GetAsync(absoluteUri))
					   .ConfigureAwait(false);
		}

		/// <summary>
		/// Sends PUT request to specified Uri using Relativity username and password for authorization.
		/// </summary>
		/// <param name="parameters">Test parameters with Relativity username and password used for authorization.</param>
		/// <param name="absoluteUri">Absolute web service Uri.</param>
		/// <param name="requestJson">Request string in JSON format.</param>
		/// <returns>Result returned by web service.</returns>
		public static async Task<string> PutAsync(
			IntegrationTestParameters parameters,
			Uri absoluteUri,
			string requestJson)
		{
			var content = new StringContent(
				requestJson,
				Encoding.UTF8,
				"application/json");

			return await HttpRequest(
							   parameters,
							   absoluteUri,
							   client => client.PutAsync(absoluteUri, content))
						   .ConfigureAwait(false);
		}

		/// <summary>
		/// Sends DELETE request to specified Uri using Relativity username and password for authorization.
		/// </summary>
		/// <param name="parameters">Test parameters with Relativity username and password used for authorization.</param>
		/// <param name="absoluteUri">Absolute web service Uri.</param>
		/// <returns>Result returned by web service.</returns>
		public static async Task<string> DeleteAsync(IntegrationTestParameters parameters, Uri absoluteUri)
		{
			return await HttpRequest(
						   parameters,
						   absoluteUri,
						   client => client.DeleteAsync(absoluteUri))
					   .ConfigureAwait(false);
		}

		/// <summary>
		/// Sends GET request to specified Uri using Relativity username and password for authorization.
		/// </summary>
		/// <param name="parameters">Test parameters with Relativity username and password used for authorization.</param>
		/// <param name="absoluteUri">Absolute web service Uri.</param>
		/// <param name="httpRequestFunc">Http Request function.</param>
		/// <returns>Result returned by web service.</returns>
		private static async Task<string> HttpRequest(
			IntegrationTestParameters parameters,
			Uri absoluteUri,
			Func<HttpClient, Task<HttpResponseMessage>> httpRequestFunc)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", "-");
				string basicAuth = $"{parameters.RelativityUserName}:{parameters.RelativityPassword}";

				httpClient.DefaultRequestHeaders.Add(
					"Authorization",
					$"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes(basicAuth))}");

				HttpResponseMessage response = await httpRequestFunc(httpClient).ConfigureAwait(false);
				string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

				if (!response.IsSuccessStatusCode)
				{
					throw new HttpServiceException(
						$"HTTP {nameof(httpRequestFunc)} request failed" + Environment.NewLine
																		 + new
																		 {
																			 response,
																			 result,
																			 url = absoluteUri,
																			 basicAuth,
																			 httpClient.DefaultRequestHeaders,
																		 });
				}

				return result;
			}
		}
	}
}