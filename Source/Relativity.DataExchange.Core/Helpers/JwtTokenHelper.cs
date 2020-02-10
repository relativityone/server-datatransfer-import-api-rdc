// <copyright file="JwtTokenHelper.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Helpers
{
	using System;
	using System.Linq;
	using System.Text;

	using Newtonsoft.Json.Linq;

	/// <summary>
	/// It encodes Relativity JWT token.
	/// </summary>
	public static class JwtTokenHelper
	{
		private const int PayloadIndex = 1;
		private const string UserIdKey = "rel_uai";
		private const string InstanceIdKey = "rel_ins";

		/// <summary>
		/// Try parse the token.
		/// </summary>
		/// <param name="token">
		/// input parameter to being parsed.
		/// </param>
		/// <param name="jwtAuthToken">
		/// output parameter as the result of parse operation. Fields will be not initialized on failure.
		/// </param>
		/// <returns>
		/// true if operation was successful otherwise false.
		/// </returns>
		public static bool TryParse(string token, out JwtAuthToken jwtAuthToken)
		{
			jwtAuthToken = new JwtAuthToken();

			if (!TryDecodePayloadPartFromBase64(token, out var payloadDecoded))
			{
				return false;
			}

			JObject payloadData;
			try
			{
				payloadData = JObject.Parse(payloadDecoded);
			}
			catch (Exception)
			{
				// this very pessimistic case when the decoded text is not in json format
				return false;
			}

			JToken userIdClaim = payloadData.SelectToken(UserIdKey);
			if (userIdClaim == null)
			{
				return false;
			}

			JToken instanceIdClaim = payloadData.SelectToken(InstanceIdKey);
			if (instanceIdClaim == null)
			{
				return false;
			}

			jwtAuthToken.RelativityInstanceId = instanceIdClaim.Value<string>();
			jwtAuthToken.RelativityUserId = userIdClaim.Value<string>();

			return true;
		}

		private static bool TryDecodePayloadPartFromBase64(string token, out string payloadDecoded)
		{
			payloadDecoded = null;
			if (token.IsNullOrEmpty())
			{
				return false;
			}

			string[] tokenParts = token.Split('.');

			// JWT token should be consists of three parts: Header, Payload, Signature
			if (!tokenParts.Any() || tokenParts.Length != 3)
			{
				return false;
			}

			// Get payload part
			string payloadEncoded = token.Split('.')[PayloadIndex];
			if (!TryDecode(payloadEncoded, out payloadDecoded))
			{
				return false;
			}

			return true;
		}

		private static bool TryDecode(string encoded, out string decoded)
		{
			decoded = null;
			string base64String = encoded.Replace('-', '+').Replace('_', '/');
			var base64 = Encoding.ASCII.GetBytes(base64String);
			int padding = (base64.Length * 3) % 4;

			if (padding != 0)
			{
				base64String = base64String.PadRight(base64String.Length + padding, '=');
			}

			try
			{
				decoded = Encoding.ASCII.GetString(Convert.FromBase64String(base64String));
			}
			catch (Exception)
			{
				// This is (Try...(..., out )) method so we don't expect to log here anything - the calling client should take care of it
				return false;
			}

			return true;
		}
	}
}