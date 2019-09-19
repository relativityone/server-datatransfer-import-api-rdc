// <copyright file="TapiCredentialsProvider.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	using System.Net;

	using Relativity.Transfer;

	/// <summary />
	public class TapiCredentialsProvider
	{
		/// <summary />
		public NetworkCredential Credential { get; set; }

		/// <summary />
		public IAuthenticationTokenProvider TokenProvider { get; set; }
	}
}
