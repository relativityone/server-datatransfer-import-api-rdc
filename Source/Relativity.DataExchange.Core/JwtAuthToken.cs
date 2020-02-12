// <copyright file="JwtAuthToken.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	/// <summary>
	/// It represents Relativity token claims.
	/// </summary>
	public class JwtAuthToken
	{
		/// <summary>
		/// Gets or sets "rel_ins" claim.
		/// </summary>
		public string RelativityInstanceId { get; set; }

		/// <summary>
		/// Gets or sets "rel_uai" claim.
		/// </summary>
		public string RelativityUserId { get; set; }
	}
}
