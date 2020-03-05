// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportApiSecureLogFactory.cs" company="Relativity ODA LLC">
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
	/// Provides fabric methods to create Relativity.Logging.ILog instances having in mind security requirements.
	/// </summary>
	public class ImportApiSecureLogFactory : ISecureLogFactory
	{
		/// <inheritdoc/>
		public ILog CreateSecureLogger()
		{
			ILog currentLogger = Log.Logger;
			if (!AppSettings.Instance.LogHashingEnabled)
			{
				return currentLogger;
			}

			return new HashingLoggerDecorator(currentLogger);
		}
	}
}
