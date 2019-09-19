// <copyright file="NullAuthTokenProvider.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	using Relativity.Transfer;

	/// <summary>
	/// fwef.
	/// </summary>
	public class NullAuthTokenProvider : IAuthenticationTokenProvider
	{
		/// <summary>
		///  ewq w.
		/// </summary>
		/// <returns>www.</returns>
		public string GenerateToken()
		{
			return string.Empty;
		}
	}
}
