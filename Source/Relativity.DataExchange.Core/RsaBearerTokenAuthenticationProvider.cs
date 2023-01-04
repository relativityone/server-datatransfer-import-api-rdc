// <copyright file="RsaBearerTokenAuthenticationProvider.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	using System;

	using Relativity.DataExchange.Service;

	/// <summary>
	/// This class obtains current claims principal token that can be used by Relativity Service Account hosted processes.
	/// </summary>
	internal class RsaBearerTokenAuthenticationProvider : IRelativityTokenProvider
	{
		/// <summary>
		/// Returns Service Account token.
		/// </summary>
		/// <returns>Relativity Service Account token value.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Handled by the ImportApi class")]
		public string GetToken()
		{
			string token = System.Security.Claims.ClaimsPrincipal.Current.Claims.AccessToken();
			if (string.IsNullOrEmpty(token))
			{
				throw new Exception("The current claims principal does not have a bearer token.");
			}

			return token;
		}
	}
}
