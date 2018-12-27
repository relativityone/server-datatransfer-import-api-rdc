// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativityManagerService2.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to provide a Transfer API bridge to existing WinEDDS code for downloading.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.Caching;

	using Polly;

	/// <summary>
	/// Represents a class object to provide a RelativityManager WebAPI wrapper.
	/// </summary>
	/// <remarks>
	/// This class exists in this assembly to provide minimal WebAPI functionality.
	/// </remarks>
	internal class RelativityManagerService : WebApiServiceBase
	{
		private const string RelativityUrlKey = "REL-URL-7480CAB5-A1C5-414B-BC05-512EADC5BCA3";

		/// <summary>
		/// The user manager service backing.
		/// </summary>
		private readonly UserManagerService userManagerService;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:kCura.WinEDDS.TApi.WebApiService2"/> class.
		/// </summary>
		/// <param name="parameters">
		/// The TAPI bridge parameters.
		/// </param>
		/// <param name="userManagerService">
		/// The user manager service.
		/// </param>
		public RelativityManagerService(TapiBridgeParameters parameters, UserManagerService userManagerService)
			: base(parameters)
		{
			this.userManagerService = userManagerService;
		}

		/// <summary>
		/// Retrieves the Relativity URL from the cache or from a WebAPI service.
		/// </summary>
		/// <returns>
		/// The Relativity URL.
		/// </returns>
		public Uri GetRelativityUrl()
		{
			string key = $"{RelativityUrlKey}-{this.WebServiceUrl.GetHashCode()}";
			Uri relativityUrl = MemoryCache.Default.Get(key) as Uri;
			if (relativityUrl != null)
			{
				return relativityUrl;
			}

			relativityUrl = this.GetWebApiRelativityUrl();
			var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(this.ExpirationMinutes) };
			MemoryCache.Default.Set(key, relativityUrl, policy);
			return relativityUrl;
		}

		private Uri GetWebApiRelativityUrl()
		{
			this.Initialize();
			var policy = Policy.Handle<Exception>().WaitAndRetry(
				this.MaxRetryAttempts,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
				(exception, span) =>
				{
					this.LogError(exception, $"Get BCP share path failed - retry span: {span}");
					this.CheckLogin(exception);
				});
			return policy.Execute(() =>
			{
				using (var serviceInstance = new RelativityManager())
				{
					serviceInstance.Url = Combine(this.WebServiceUrl, "RelativityManager.asmx");
					serviceInstance.CookieContainer = this.CookieContainer;
					serviceInstance.Credentials = this.Credential;
					serviceInstance.Timeout = (int) TimeSpan.FromSeconds(this.TimeoutSeconds).TotalMilliseconds;
					return new Uri(serviceInstance.GetRelativityUrl());
				}
			});
		}

		/// <summary>
		/// Attempts to perform a distributed login if the exception indicates an authentication error.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		private void CheckLogin(Exception exception)
		{
			var candidates = new List<string>
			{
				exception.ToString(),
				exception.InnerException?.ToString() ?? string.Empty
			};

			var accessDenied = candidates.Any(
				x => x.IndexOf("kcuraaccessdeniedmarker", StringComparison.OrdinalIgnoreCase) != -1);
			var tokenExpired = candidates.Any(
				x => x.IndexOf("NeedToReLoginException", StringComparison.OrdinalIgnoreCase) != -1);
			if (accessDenied)
			{
				this.LogInformation(
					"An access denied exception occurred and requesting a new login token.");
				this.userManagerService.Login();
			}
			else if (tokenExpired)
			{
				this.LogInformation("The login token has expired and requesting a new login token.");
				this.userManagerService.Login();
			}
		}

		[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
		[System.Diagnostics.DebuggerStepThroughAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Web.Services.WebServiceBindingAttribute(Name = "RelativityManagerSoap", Namespace = "http://www.kCura.com/EDDS/RelativityManager")]
		private class RelativityManager : System.Web.Services.Protocols.SoapHttpClientProtocol
		{
			[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/RelativityManager/GetRelativityUrl", RequestNamespace = "http://www.kCura.com/EDDS/RelativityManager", ResponseNamespace = "http://www.kCura.com/EDDS/RelativityManager", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
			public string GetRelativityUrl()
			{
				object[] results = this.Invoke("GetRelativityUrl", new object[0]);
				return ((string)(results[0]));
			}
		}
	}
}