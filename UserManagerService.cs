// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserManagerService2.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to provide a Transfer API bridge to existing WinEDDS code for downloading.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;

	using Polly;

	/// <summary>
	/// Represents a class object to provide a UserManager WebAPI wrapper.
	/// </summary>
	/// <remarks>
	/// This class exists in this assembly to provide minimal WebAPI functionality.
	/// </remarks>
	internal class UserManagerService : WebApiServiceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:kCura.WinEDDS.TApi.UserManagerService2"/> class.
		/// </summary>
		/// <param name="parameters">
		/// The TAPI bridge parameters.
		/// </param>
		public UserManagerService(TapiBridgeParameters parameters)
			: base(parameters)
		{
		}

		/// <summary>
		/// Attempts to login with the specified configuration and connection information.
		/// </summary>
		public void Login()
		{
			this.Initialize();
			var policy = Policy.Handle<Exception>().WaitAndRetry(
				this.MaxRetryAttempts,
				retryAttempt => TimeSpan.FromSeconds(this.WaitTimeBetweenRetryAttempts),
				(exception, span) => { this.LogError(exception, $"Login - retry span: {span}"); });
			policy.Execute(
				() =>
				{
					using (var serviceInstance = new UserManager())
					{
						serviceInstance.Credentials = this.Credential;
						serviceInstance.CookieContainer = this.CookieContainer;
						serviceInstance.Timeout = (int) TimeSpan.FromSeconds(this.TimeoutSeconds).TotalMilliseconds;
						serviceInstance.Url = Combine(this.WebServiceUrl, "UserManager.asmx");
						serviceInstance.ClearCookiesBeforeLogin();

						// Never perform a login when using integrated security.
						if (!string.IsNullOrEmpty(this.Credential.UserName) &&
						    !string.IsNullOrEmpty(this.Credential.Password))
						{
							if (!serviceInstance.Login(this.Credential.UserName, this.Credential.Password))
							{
								throw new InvalidOperationException(
									"The login request using the distributed API failed.");
							}
						}
					}
				});
		}

		[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.6.1055.0")]
		[System.Diagnostics.DebuggerStepThroughAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Web.Services.WebServiceBindingAttribute(Name = "UserManagerSoap", Namespace = "http://www.kCura.com/EDDS/UserManager")]
		private class UserManager : System.Web.Services.Protocols.SoapHttpClientProtocol
		{
			[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/UserManager/ClearCookiesBeforeLogin", RequestNamespace = "http://www.kCura.com/EDDS/UserManager", ResponseNamespace = "http://www.kCura.com/EDDS/UserManager", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
			public void ClearCookiesBeforeLogin()
			{
				this.Invoke("ClearCookiesBeforeLogin", new object[0]);
			}

			[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/UserManager/GenerateDistributedAuthenticationToken", RequestNamespace = "http://www.kCura.com/EDDS/UserManager", ResponseNamespace = "http://www.kCura.com/EDDS/UserManager", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
			public string GenerateDistributedAuthenticationToken()
			{
				object[] results = this.Invoke("GenerateDistributedAuthenticationToken", new object[0]);
				return ((string)(results[0]));
			}

			[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/UserManager/Login", RequestNamespace = "http://www.kCura.com/EDDS/UserManager", ResponseNamespace = "http://www.kCura.com/EDDS/UserManager", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
			public bool Login(string emailAddress, string password)
			{
				object[] results = this.Invoke("Login", new object[] {
					emailAddress,
					password});
				return ((bool)(results[0]));
			}
		}
	}
}