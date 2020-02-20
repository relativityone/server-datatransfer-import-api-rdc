// <copyright file="IRelativityTokenProvider.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	/// <summary>
	/// Provides method to obtain valid Relativity authentication access token.
	/// </summary>
	public interface IRelativityTokenProvider
	{
		/// <summary>
		/// It should return valid Relativity authentication token.
		/// </summary>
		/// <returns>token value.</returns>
		string GetToken();
	}
}
