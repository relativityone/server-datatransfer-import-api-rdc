// ----------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;

	/// <summary>
	/// Defines static <see cref="DateTime"/> extension methods.
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// The SQL culture neutral format string.
		/// </summary>
		private const string SqlCultureNeutralFormatString = "yyyyMMdd HH:mm:ss.fff";

		/// <summary>
		/// Converts the culture-neutral sql-formatted string representation of the provided date.
		/// </summary>
		/// <param name="input">
		/// The date to convert.
		/// </param>
		/// <returns>
		/// A culture-neutral sql-formatted representation of <see param="input"/>.
		/// </returns>
		public static string ToSqlCultureNeutralString(this DateTime input)
		{
			return input.ToString(SqlCultureNeutralFormatString);
		}
	}
}