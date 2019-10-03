// <copyright file="NullAuthTokenProvider.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	using Relativity.Transfer;

	/// <summary>
	/// This class represents the authentication provider for not agent based scenarios.
	/// </summary>
	internal class NullAuthTokenProvider : IAuthenticationTokenProvider
	{
		/// <summary>
		///  It returns empty string token.
		/// </summary>
		/// <returns>empty string.</returns>
		public string GenerateToken()
		{
			return string.Empty;
		}
	}
}
