// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativityManagerService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to provide a Transfer API bridge to existing WinEDDS code for downloading.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Services
{
	using System;

	using Polly;

	/// <summary>
	/// Represents a class object to provide a RelativityManager WebAPI wrapper.
	/// </summary>
	/// <remarks>
	/// This class exists in this assembly to provide minimal WebAPI functionality.
	/// </remarks>
	internal class RelativityManagerService : WebApiServiceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityManagerService"/> class.
		/// </summary>
		/// <param name="instanceInfo">
		/// The Relativity instance information.
		/// </param>
		public RelativityManagerService(RelativityInstanceInfo instanceInfo)
			: base(instanceInfo)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityManagerService"/> class.
		/// </summary>
		/// <param name="instanceInfo">
		/// The Relativity instance information.
		/// </param>
		/// <param name="repository">
		/// The object cache repository.
		/// </param>
		/// <param name="appSettings">
		/// The application settings.
		/// </param>
		protected RelativityManagerService(
			RelativityInstanceInfo instanceInfo,
			IObjectCacheRepository repository,
			IAppSettings appSettings)
			: base(instanceInfo, repository, appSettings)
		{
		}

		/// <summary>
		/// Retrieves the Relativity URL from the cache or from a WebAPI service.
		/// </summary>
		/// <returns>
		/// The <see cref="Uri"/> instance.
		/// </returns>
		public Uri GetRelativityUrl()
		{
			string webApiServiceUrl = this.InstanceInfo.WebApiServiceUrl.ToString();
			string cacheKey = CacheKeys.CreateRelativityUrlCacheKey(webApiServiceUrl);
			Uri relativityUrl = this.CacheRepository.SelectByKey<Uri>(cacheKey);
			if (relativityUrl != null)
			{
				return relativityUrl;
			}

			this.Initialize();
			var policy = Policy
				.Handle<Exception>(exception => !ExceptionHelper.IsFatalException(exception)).WaitAndRetry(
					this.AppSettings.IoErrorNumberOfRetries,
					retryAttempt => TimeSpan.FromSeconds(this.AppSettings.IoErrorWaitTimeInSeconds),
					(exception, span) =>
						{
							this.LogError(exception, $"Get Relativity URL failed - retry span: {span}");
						});
			return policy.Execute(() =>
			{
				using (var serviceInstance = new RelativityManager())
				{
					serviceInstance.Url = webApiServiceUrl.CombineUrls("RelativityManager.asmx");
					serviceInstance.CookieContainer = this.InstanceInfo.CookieContainer;

					// REL-281370: by design, GetRelativityUrl does NOT require authentication.
					//             As a result, the UserManager Login service isn't required.
					serviceInstance.Credentials = System.Net.CredentialCache.DefaultCredentials;
					serviceInstance.Timeout =
						(int)TimeSpan.FromSeconds(this.AppSettings.WebApiOperationTimeout).TotalMilliseconds;
					string relativityUrlString = serviceInstance.GetRelativityUrl();
					relativityUrl = new Uri(relativityUrlString);
					this.CacheRepository.Upsert(cacheKey, relativityUrl);
					return relativityUrl;
				}
			});
		}

		[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
		[System.Diagnostics.DebuggerStepThroughAttribute]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Web.Services.WebServiceBindingAttribute(Name = "RelativityManagerSoap", Namespace = "http://www.kCura.com/EDDS/RelativityManager")]
		private class RelativityManager : System.Web.Services.Protocols.SoapHttpClientProtocol
		{
			[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/RelativityManager/GetRelativityUrl", RequestNamespace = "http://www.kCura.com/EDDS/RelativityManager", ResponseNamespace = "http://www.kCura.com/EDDS/RelativityManager", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
			public string GetRelativityUrl()
			{
				object[] results = this.Invoke("GetRelativityUrl", new object[0]);
				return (string)results[0];
			}
		}
	}
}