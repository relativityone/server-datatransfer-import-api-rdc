using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Relativity.Import.Export.Services
{
	public static class ClaimsExtensions
	{
		public const string ACCESS_TOKEN_IDENTIFIER = "access_token";

		public static string AccessToken(this IEnumerable<Claim> userClaims)
		{
			string retVal = string.Empty;
			Claim previewId = userClaims.FirstOrDefault(x => x.Type == ACCESS_TOKEN_IDENTIFIER);
			if (previewId != null)
			{
				retVal = previewId.Value;
			}

			return retVal;
		}
	}
}