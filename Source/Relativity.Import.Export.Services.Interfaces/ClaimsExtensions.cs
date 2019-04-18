using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Relativity.Import.Export.Services
{
	public static class ClaimsLookUp
	{
		public static Int32 UserArtifactID(this IEnumerable<Claim> userClaims)
		{
			return LookUpInt32Claim(userClaims, Claims.USER_ID);
		}

		public static string UserEmailAddress(this IEnumerable<Claim> userClaims)
		{
			return LookUpClaim(userClaims, ClaimTypes.NameIdentifier);
		}

		public static string UserFirstName(this IEnumerable<Claim> userClaims)
		{
			return LookUpClaim(userClaims, Claims.USER_FIRST_NAME);
		}

		public static string UserLastName(this IEnumerable<Claim> userClaims)
		{
			return LookUpClaim(userClaims, Claims.USER_LAST_NAME);
		}

		public static string UserUserName(this IEnumerable<Claim> userClaims)
		{
			return LookUpClaim(userClaims, Claims.USERNAME);
		}

		public static Int32 PreviewUserID(this IEnumerable<Claim> userClaims)
		{
			return LookUpInt32Claim(userClaims, Claims.PREVIEW_USER_ID);
		}

		public static string PreviewUserFirstName(this IEnumerable<Claim> userClaims)
		{
			return LookUpClaim(userClaims, Claims.PREVIEW_USER_FIRST_NAME);
		}

		public static string PreviewUserLastName(this IEnumerable<Claim> userClaims)
		{
			return LookUpClaim(userClaims, Claims.PREVIEW_USER_LAST_NAME);
		}

		public static string PreviewUserUserName(this IEnumerable<Claim> userClaims)
		{
			return LookUpClaim(userClaims, Claims.PREVIEW_USERNAME);
		}

		public static string RequestOrigin(this IEnumerable<Claim> userClaims)
		{
			return LookUpClaim(userClaims, Claims.RELATIVITY_REQUEST_ORIGIN);
		}

		public static string RelativityAccess(this IEnumerable<Claim> userClaims)
		{
			return LookUpClaim(userClaims, Claims.RELATIVITY_ACCESS);
		}

		public static string AccessToken(this IEnumerable<Claim> userClaims)
		{
			return LookUpClaim(userClaims, Claims.ACCESS_TOKEN_IDENTIFIER);
		}

		public static bool UserCanChangePassword(this IEnumerable<Claim> userClaims)
		{
			return LookUpBooleanClaim(userClaims, Claims.USER_CAN_CHANGE_PASSWORD, false);
		}

		public static Guid AuthenticationID(this IEnumerable<Claim> userClaims)
		{
			string authID = LookUpClaim(userClaims, Claims.AUTHENTICATION_ID);
			Guid retval = Guid.Empty;
			if ((!string.IsNullOrWhiteSpace(authID)))
				retval = new Guid(authID);
			return retval;
		}

		public static DateTime ExpirationTime(this IEnumerable<Claim> userClaims)
		{
			return new DateTime(LookUpInt64Claim(userClaims, Claims.EXPIRES));
		}

		public static DateTime IssuedOn(this IEnumerable<Claim> userClaims)
		{
			Int64 unixTime = 0;
			string longString = LookUpClaim(userClaims, Claims.IAT);
			if (!string.IsNullOrEmpty(longString))
				unixTime = Convert.ToInt64(longString);
			return FromUnixTime(unixTime);
		}

		public static bool OverrideLockbox(this IEnumerable<Claim> userClaims)
		{
			return LookUpBooleanClaim(userClaims, Claims.OVERRIDE_LOCKBOX, false);
		}

		public static Claim FindClaim(this IEnumerable<Claim> userClaims, string claimType)
		{
			return userClaims.FirstOrDefault(x => x.Type == claimType);
		}


		private static Int32 LookUpInt32Claim(IEnumerable<Claim> userClaims, string claimType)
		{
			Int32 retVal = 0;
			string value = LookUpClaim(userClaims, claimType);
			if (!string.IsNullOrEmpty(value))
				retVal = Convert.ToInt32(value);
			return retVal;
		}

		private static Int64 LookUpInt64Claim(IEnumerable<Claim> userClaims, string claimType)
		{
			Int64 retVal = 0;
			string value = LookUpClaim(userClaims, claimType);
			if (!string.IsNullOrEmpty(value))
				retVal = Convert.ToInt64(value);
			return retVal;
		}

		private static bool LookUpBooleanClaim(IEnumerable<Claim> userClaims, string claimType, bool defaultValue)
		{
			bool retVal = defaultValue;
			string value = LookUpClaim(userClaims, claimType);
			if (!string.IsNullOrEmpty(value))
				retVal = Convert.ToBoolean(value);
			return retVal;
		}

		private static string LookUpClaim(IEnumerable<Claim> userClaims, string claimType)
		{
			string retVal = string.Empty;
			Claim previewID = FindClaim(userClaims, claimType);
			if (previewID != null)
				retVal = previewID.Value;
			return retVal;
		}

		private static DateTime FromUnixTime(Int64 unixTime)
		{
			DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddSeconds(unixTime);
		}
	}
}
