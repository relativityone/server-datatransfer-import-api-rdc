using System;
using System.Net;
using System.Net.Http;
using System.Text;
using kCura.NUnit.Integration;
using Platform.Keywords.Connection;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Helpers
{
	internal static class Rest
	{
		private const string _JSON_MIME = "application/json";

		public static string PostRequestAsJson(string serviceMethod, string parameter = null)
		{
			bool isHttps = SharedTestVariables.SERVER_BINDING_TYPE.Equals("https", StringComparison.InvariantCultureIgnoreCase);
			return PostRequestAsJsonInternal(serviceMethod, isHttps, SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD, parameter);
		}

		public static string GetRequest(string serviceMethod, bool isHttps, string username, string password)
		{
			Uri baseAddress = ServiceFactory.GetKeplerUrl();
			WebRequestHandler handler = new WebRequestHandler();

			if (isHttps)
			{
				handler.ServerCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => true;
			}

			using (HttpClient httpClient = new HttpClient(handler))
			{
				httpClient.BaseAddress = baseAddress;

				//Set header information
				string authorizationBase64 = GetBase64String($"{ username }:{ password }");
				string authorizationHeader = $"Basic { authorizationBase64 }";
				httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
				httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", string.Empty);

				//Send Get Request
				string output;
				try
				{
					using (HttpResponseMessage response = httpClient.GetAsync(serviceMethod).ConfigureAwait(false).GetAwaiter().GetResult())
					{
						if (!response.IsSuccessStatusCode)
						{
							string errorMessage = $"Failed submitting post request. Response Error: { response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult() }.";
#pragma warning disable RG1006 // System.Exception Rule
							throw new Exception(errorMessage);
#pragma warning restore RG1006 // System.Exception Rule
						}
						output = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
					}
				}
				catch (Exception ex)
				{
					string errorMessage = $"An error occurred when attempting to submit post request. { ex.Message }.";
#pragma warning disable RG1006 // System.Exception Rule
					throw new Exception(errorMessage);
#pragma warning restore RG1006 // System.Exception Rule
				}
				return output;
			}
		}

		public static string PostRequestAsJsonInternal(string serviceMethod, bool isHttps, string username, string password, string parameter)
		{
			Uri baseAddress = ServiceFactory.GetKeplerUrl();
			WebRequestHandler handler = new WebRequestHandler();

			if (isHttps)
			{
				handler.ServerCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => true;
			}

			using (HttpClient httpClient = new HttpClient(handler))
			{
				httpClient.BaseAddress = baseAddress;

				//Set header information
				string authorizationBase64 = GetBase64String($"{username}:{password}");
				string authorizationHeader = $"Basic {authorizationBase64}";
				httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
				httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", string.Empty);

				//Assign parameter if needed and send Post Request
				using (HttpContent content = parameter != null ? new StringContent(parameter, Encoding.UTF8, _JSON_MIME) : null)
				{
					string output;
					try
					{
						using (HttpResponseMessage response = httpClient.PostAsync(serviceMethod, content).Result)
						{
							if (!response.IsSuccessStatusCode)
							{
								string errorMessage = $"Failed submitting post request. Response Error: {response.Content.ReadAsStringAsync().Result}.";
#pragma warning disable RG1006 // System.Exception Rule
								throw new Exception(errorMessage);
#pragma warning restore RG1006 // System.Exception Rule
							}
							output = response.Content.ReadAsStringAsync().Result;
						}
					}
					catch (Exception ex)
					{
						string message = string.Join(
							Environment.NewLine,
							$@"An error occurred when attempting to submit post request. {ex.Message}.",
							$@"Username: {username}",
							$@"Password: {password}",
							$@"Param: {parameter}",
							$@"Base address: {baseAddress}",
							$@"Service: {serviceMethod}",
							$@"Auth header: {authorizationHeader}"
						);
#pragma warning disable RG1006 // System.Exception Rule
						throw new Exception(message, ex);
#pragma warning restore RG1006 // System.Exception Rule
					}
					return output;
				}
			}
		}

		public static string DeleteRequestAsJson(string restServer, string serviceMethod, string username, string password, bool isHttps)
		{
			Uri baseAddress = ServiceFactory.GetKeplerUrl();
			WebRequestHandler handler = new WebRequestHandler();

			if (isHttps)
			{
				handler.ServerCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => true;
			}

			using (HttpClient httpClient = new HttpClient(handler))
			{
				httpClient.BaseAddress = baseAddress;

				//Set header information
				string authorizationBase64 = GetBase64String(string.Format("{0}:{1}", username, password));
				string authorizationHeader = string.Format("Basic {0}", authorizationBase64);
				httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
				httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", String.Empty);

				string output;
				try
				{
					HttpResponseMessage response = httpClient.DeleteAsync(serviceMethod).Result;
					if (!response.IsSuccessStatusCode)
					{
						string errorMessage = string.Format("Failed submitting delete request. Response Error: {0}.", response.Content.ReadAsStringAsync());
#pragma warning disable RG1006 // System.Exception Rule
						throw new Exception(errorMessage);
#pragma warning restore RG1006 // System.Exception Rule
					}
					output = response.Content.ReadAsStringAsync().Result;
				}
				catch (Exception ex)
				{
					string errorMessage = string.Format("An error occurred when attempting to submit delete request. {0}.", ex.Message);
#pragma warning disable RG1006 // System.Exception Rule
					throw new Exception(errorMessage);
#pragma warning restore RG1006 // System.Exception Rule
				}
				return output;
			}
		}

		private static string GetBase64String(string stringToConvertToBase64)
		{
			string base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(stringToConvertToBase64));
			return base64String;
		}
	}
}
