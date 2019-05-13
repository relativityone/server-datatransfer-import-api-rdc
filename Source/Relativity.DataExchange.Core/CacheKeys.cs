// ----------------------------------------------------------------------------
// <copyright file="CacheKeys.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	/// <summary>
	/// Provides static methods to create memory cache keys used by import/export class objects.
	/// </summary>
	internal static class CacheKeys
	{
		/// <summary>
		/// The compatibility check cache identifier.
		/// </summary>
		public const string CompatibilityCheckCacheId = "2619DD28-5BA7-4774-8D8C-278DE295254F";

		/// <summary>
		/// The Relativity URL cache identifier.
		/// </summary>
		public const string RelativityUrlCacheId = "F659E4EE-9B41-46A6-9D77-76C828A2D0A7";

		/// <summary>
		/// Creates a cache key for the compatibility check. A trailing slash is automatically appended if one isn't found.
		/// </summary>
		/// <param name="host">
		/// The Relativity host.
		/// </param>
		/// <returns>
		/// The key.
		/// </returns>
		public static string CreateCompatibilityCheckCacheKey(string host)
		{
			host = host.AppendTrailingSlashToUrl();
			return $"{CompatibilityCheckCacheId}.{host}".ToUpperInvariant();
		}

		/// <summary>
		/// Creates a cache key for the Relativity URL. A trailing slash is automatically appended if one isn't found.
		/// </summary>
		/// <param name="host">
		/// The Relativity host.
		/// </param>
		/// <returns>
		/// The key.
		/// </returns>
		public static string CreateRelativityUrlCacheKey(string host)
		{
			host = host.AppendTrailingSlashToUrl();
			return $"{RelativityUrlCacheId}.{host}".ToUpperInvariant();
		}
	}
}