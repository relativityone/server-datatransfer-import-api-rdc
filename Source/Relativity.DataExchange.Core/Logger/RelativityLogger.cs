// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativityLogger.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines a static property to get the registered Relativity log instance.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Logger
{
	using Relativity.Logging;

	/// <summary>
	/// Defines a static property to get the registered Relativity log instance.
	/// </summary>
	internal static class RelativityLogger
	{
		private static ILog instance;

		static RelativityLogger()
		{
			Instance = new NullLogger();
		}

		/// <summary>
		/// Gets or sets the registered Relativity.Logging.ILog instance with security requirements. If not defined, returns the <see cref="Relativity.Logging.NullLogger"/> instance.
		/// This should only be used by class objects where the <see cref="Relativity.Logging.ILog"/> instance isn't already constructor injected or by existing constructors marked with <see cref="System.ObsoleteAttribute"/>.
		/// </summary>
		/// <value>
		/// The <see cref="ILog"/> instance.
		/// </value>
		public static ILog Instance
		{
			get
			{
				if (instance == null)
				{
					Instance = new NullLogger();
				}

				return instance;
			}

			set
			{
				instance = value;
			}
		}
	}
}