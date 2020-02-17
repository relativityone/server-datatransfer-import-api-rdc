// <copyright file="AuthTokenProviderAdapter.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	using Relativity.Transfer;

	/// <summary>
	/// This class represents method of refreshing the token credentials on the expiration event
	/// Instance of this class is injected to Tapi.
	/// </summary>
	internal class AuthTokenProviderAdapter : IAuthenticationTokenProvider
	{
		private readonly IRelativityTokenProvider _relativityTokenProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthTokenProviderAdapter"/> class.
		/// </summary>
		/// <param name="relativityTokenProvider">IRelativityTokenProvider token instance.</param>
		public AuthTokenProviderAdapter(IRelativityTokenProvider relativityTokenProvider)
		{
			relativityTokenProvider.ThrowIfNull(nameof(relativityTokenProvider));

			this._relativityTokenProvider = relativityTokenProvider;
		}

		/// <summary>
		/// It delegated call to <see cref="IRelativityTokenProvider"/> method.
		/// </summary>
		/// <returns>Relativity authentication token value.</returns>
		public string GenerateToken()
		{
			return this._relativityTokenProvider.GetToken();
		}
	}
}
