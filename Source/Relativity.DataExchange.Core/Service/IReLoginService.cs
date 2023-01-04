// <copyright file="IReLoginService.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service
{
	/// <summary>
	/// This type provides a method for re-logging in Relativity WebAPI and Relativity.Distributed.
	/// </summary>
	public interface IReLoginService
	{
		/// <summary>
		/// Re-logging in Relativity WebAPI and Relativity.Distributed.
		/// </summary>
		/// <param name="retryOnFailure">retryOnFailure.</param>
		/// <returns>true if succeeded, false otherwise.</returns>
		bool AttemptReLogin(bool retryOnFailure = true);
	}
}
