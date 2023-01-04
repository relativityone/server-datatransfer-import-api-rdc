// <copyright file="JwtAuthToken.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	/// <summary>
	/// It represents Relativity token claims.
	/// </summary>
	public sealed class JwtAuthToken
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JwtAuthToken"/> class.
		/// </summary>
		/// <param name="relativityInstanceId">sets "rel_ins" claim.</param>
		/// <param name="relativityUserId">sets "rel_uai" claim.</param>
		public JwtAuthToken(string relativityInstanceId, string relativityUserId)
		{
			RelativityInstanceId = relativityInstanceId;
			RelativityUserId = relativityUserId;
		}

		/// <summary>
		/// Gets "rel_ins" claim.
		/// </summary>
		public string RelativityInstanceId { get; }

		/// <summary>
		/// Gets "rel_uai" claim.
		/// </summary>
		public string RelativityUserId { get; }
	}
}
