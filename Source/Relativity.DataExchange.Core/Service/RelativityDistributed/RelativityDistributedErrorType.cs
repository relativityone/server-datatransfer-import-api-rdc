// <copyright file="RelativityDistributedErrorType.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.RelativityDistributed
{
	/// <summary>
	/// Those are different error types which can be returned from the Relativity.Distributed.
	/// </summary>
	internal enum RelativityDistributedErrorType
	{
		/// <summary>
		/// Undefined
		/// </summary>
		Undefined,

		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,

		/// <summary>
		/// Authentication
		/// </summary>
		Authentication,

		/// <summary>
		/// InternalServerError
		/// </summary>
		InternalServerError,

		/// <summary>
		/// NotFound
		/// </summary>
		NotFound,

		/// <summary>
		/// DataCorrupted
		/// </summary>
		DataCorrupted,
	}
}
