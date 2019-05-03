// ----------------------------------------------------------------------------
// <copyright file="ClaimsExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Service
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;

	/// <summary>
	/// Defines claims security related extension methods.
	/// </summary>
	internal static class ClaimsExtensions
	{
		/// <summary>
		/// Retrieve the access token from the list of claims.
		/// </summary>
		/// <param name="claims">
		/// The list of claims.
		/// </param>
		/// <returns>
		/// System.String.
		/// </returns>
		public static string AccessToken(this IEnumerable<Claim> claims)
		{
			const string AccessTokenIdentifier = "access_token";
			string retVal = string.Empty;
			Claim previewId = claims.FirstOrDefault(x => x.Type == AccessTokenIdentifier);
			if (previewId != null)
			{
				retVal = previewId.Value;
			}

			return retVal;
		}
	}
}