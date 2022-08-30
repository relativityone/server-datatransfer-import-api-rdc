// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISecureLogFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Provides fabric methods to create Relativity.Logging.ILog instances having in mind security requirements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Logger
{
	using Relativity.Logging;

	/// <summary>
	/// Provides factory method to create Relativity.Logging.ILog instances having in mind security requirements.
	/// </summary>
	internal interface ISecureLogFactory
	{
		/// <summary>
		/// Create Relativity.Logging.ILog instance having in mind security requirements.
		/// </summary>
		/// <returns>A Relativity.Logging.ILog instance implementing security requirements.</returns>
		ILog CreateSecureLogger();

		/// <summary>
		/// Create Relativity.Logging.ILog instance having in mind security requirements.
		/// </summary>
		/// <param name="logger">External optional logger provided by the user. </param>
		/// <returns>A Relativity.Logging.ILog instance implementing security requirements.</returns>
		ILog CreateSecureLogger(ILog logger);
	}
}
