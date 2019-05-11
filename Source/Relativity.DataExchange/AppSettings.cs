// ----------------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	/// <summary>
	/// Defines static properties to obtain application settings.
	/// </summary>
	/// <remarks>
	/// Always favor constructor injecting <see cref="IAppSettings"/>. This is intended for legacy code only.
	/// </remarks>
	public static class AppSettings
	{
		/// <summary>
		/// The thread synchronization object.
		/// </summary>
		private static readonly object SyncRoot = new object();

		/// <summary>
		/// The singleton backing field.
		/// </summary>
		private static IAppSettings instance;

		/// <summary>
		/// Gets the application settings singleton instance.
		/// </summary>
		/// <value>
		/// The <see cref="IAppSettings"/> instance.
		/// </value>
		public static IAppSettings Instance
		{
			get
			{
				if (instance == null)
				{
					lock (SyncRoot)
					{
						if (instance == null)
						{
							const bool Refresh = true;
							instance = AppSettingsManager.Create(Refresh);
						}
					}
				}

				return instance;
			}
		}
	}
}