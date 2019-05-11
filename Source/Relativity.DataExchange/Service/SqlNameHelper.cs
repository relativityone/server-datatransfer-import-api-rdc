// ----------------------------------------------------------------------------
// <copyright file="SqlNameHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	/// <summary>
	/// Defines static helper methods to perform common SQL name operations.
	/// </summary>
	internal static class SqlNameHelper
	{
		/// <summary>
		/// Gets the SQL friendly name.
		/// </summary>
		/// <param name="displayName">
		/// The display name.
		/// </param>
		/// <returns>
		/// The friendly name.
		/// </returns>
		public static string GetSqlFriendlyName(string displayName)
		{
			return System.Text.RegularExpressions.Regex.Replace(
				displayName == null ? string.Empty : displayName,
				@"[\W]+",
				string.Empty);
		}
	}
}