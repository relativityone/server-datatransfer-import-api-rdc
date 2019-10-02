// <copyright file="TapiCredentialsProvider.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	using System.Net;

	using Relativity.Transfer;

	/// <summary>
	/// It represents the credential specific data that will be passed to Tapi.
	/// </summary>
	public class TapiCredentialsProvider
	{
		/// <summary>
		/// Gets or sets relativity credentials.
		/// </summary>
		public NetworkCredential Credential { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="IAuthenticationTokenProvider"/> instance that will be used by Tapi to refresh tokens on long running jobs.
		/// </summary>
		public IAuthenticationTokenProvider TokenProvider { get; set; }
	}
}
