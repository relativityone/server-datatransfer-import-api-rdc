// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativityLogger.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines a static property to get the registered Relativity log instance.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	/// <summary>
	/// Defines a static property to get the registered Relativity log instance.
	/// </summary>
	internal static class RelativityLogger
	{
		/// <summary>
		/// Gets the registered Relativity logging instance. If not defined, returns the <see cref="Relativity.Logging.NullLogger"/> instance.
		/// </summary>
		/// <value>
		/// The <see cref="Relativity.Logging.ILog"/> instance.
		/// </value>
		public static Relativity.Logging.ILog Instance =>
			Relativity.Logging.Log.Logger ?? new Relativity.Logging.NullLogger();
	}
}