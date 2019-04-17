// // ----------------------------------------------------------------------------
// // <copyright file="Claims.cs" company="kCura Corp">
// //   kCura Corp (C) 2019
// // </copyright>
// // ----------------------------------------------------------------------------
namespace Relativity.Import.Export.Services
{
	using System.Collections.Generic;

	public static class Claims
	{
		public static IEnumerable<string> REQUIRED_CLAIMS = new List<string>() { USER_ID, USERNAME, USER_FIRST_NAME, USER_LAST_NAME };

		public const string RELATIVITY_ACCESS = "rel_access";
		public const string RELATIVITY_INSTANCE = "rel_ins";
		public const string RELATIVITY_REQUEST_ORIGIN = "rel_origin";
		public const string USER_ID = "rel_uai";
		public const string USERNAME = "rel_un";
		public const string USER_FIRST_NAME = "rel_ufn";
		public const string USER_LAST_NAME = "rel_uln";

		public const string PREVIEW_USER_ID = "rel_p_uai";
		public const string PREVIEW_USERNAME = "rel_p_un";
		public const string PREVIEW_USER_FIRST_NAME = "rel_p_ufn";
		public const string PREVIEW_USER_LAST_NAME = "rel_p_uln";
		public const string EXPIRES = "Expires";
		public const string AUTHENTICATION_ID = "rel_aid";
		public const string OVERRIDE_LOCKBOX = "or_lb";
		public const string USER_CAN_CHANGE_PASSWORD = "can_change_password";

		public const string ACCESS_TOKEN_IDENTIFIER = "access_token";

		public const string SERVICE_BUS_ACTION = "net.windows.servicebus.action";

		public const string IAT = "iat";
	}
}