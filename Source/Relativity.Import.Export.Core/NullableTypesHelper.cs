// ----------------------------------------------------------------------------
// <copyright file="NullableTypesHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Globalization;

	using Microsoft.VisualBasic.CompilerServices;

	/// <summary>
	/// Defines static helper methods to convert nullable types.
	/// </summary>
	internal static class NullableTypesHelper
	{
		/// <summary>
		/// Casts the provided object, if it is not <see cref="F:System.DBNull.Value" />, to a nullable object of the specified type.
		/// </summary>
		/// <typeparam name="T">
		/// The type of the nullable result.
		/// </typeparam>
		/// <param name="value">
		/// The item to convert.
		/// </param>
		/// <returns>
		/// A nullable representation of <see paramref="value" /> that has been cast to <typeparamref name="T"/>.
		/// </returns>
		/// <exception cref="InvalidCastException">
		/// The cast of <paramref name="value"/> for type <typeparamref name="T"/> failed.
		/// </exception>
		/// <exception cref="NullReferenceException">
		/// The <paramref name="value"/> is <see langword="null" />.
		/// </exception>
		public static T? DBNullConvertToNullable<T>(object value)
			where T : struct
		{
			if (value == DBNull.Value)
			{
				return null;
			}

			return (T)value;
		}

		/// <summary>
		/// Casts the provided object, if it is not <see cref="F:System.DBNull.Value" />, to a string.
		/// </summary>
		/// <param name="item">
		/// The item to convert.
		/// </param>
		/// <returns>
		/// A string representation of <see param="Item" />.
		/// </returns>
		public static string DBNullString(object item)
		{
			return item == DBNull.Value ? null : Conversions.ToString(item);
		}

		/// <summary>
		/// Casts the provided string to a nullable boolean.
		/// </summary>
		/// <param name="value">
		/// The value to consider.
		/// </param>
		/// <returns>
		/// A nullable boolean representation of <see param="value"/>.
		/// </returns>
		public static bool? GetNullableBoolean(string value)
		{
			if (value == null || value.Trim().Length == 0)
			{
				return null;
			}

			switch (value.Trim().ToLowerInvariant())
			{
				case "yes":
				case "y":
				case "1":
				case "t":
					value = "True";
					break;

				case "no":
				case "n":
				case "0":
				case "f":
					value = "False";
					break;

				case "true":
					value = "True";
					break;

				case "false":
					value = "False";
					break;

				default:
					if (value != string.Empty)
					{
						int parsedValue;
						if (int.TryParse(value, out parsedValue))
						{
							value = parsedValue == 0 ? "False" : "True";
						}
						else
						{
							value = "False";
						}
					}

					break;
			}

			return ToNullableBoolean(value);
		}

		/// <summary>
		/// Casts the provided string to a nullable date, utilizing the provided format string.
		/// </summary>
		/// <param name="value">
		/// The value to convert.
		/// </param>
		/// <returns>
		/// A nullable boolean representation of <see paramref="value"/>.
		/// </returns>
		/// <exception cref="System.SystemException">
		/// Thrown when <paramref name="value"/> is not a valid string representation of a date and time.
		/// </exception>
		public static DateTime? GetNullableDateTime(string value)
		{
			if (value == null || value.Trim().Length == 0)
			{
				return null;
			}

			DateTime? nullableDateValue;

			try
			{
				nullableDateValue = ToNullableDateTime(value);
			}
			catch (Exception)
			{
				switch (value.Trim())
				{
					case "00/00/0000":
					case "0/0/0000":
					case "0/0/00":
					case "00/00/00":
					case "0/00":
					case "0/0000":
					case "00/00":
					case "00/0000":
					case "0":
						nullableDateValue = null;
						if (AppSettings.CreateErrorForInvalidDate)
						{
							throw new SystemException("Invalid date.");
						}

						break;

					default:

						try
						{
							if (System.Text.RegularExpressions.Regex.IsMatch(value.Trim(), @"\d\d\d\d\d\d\d\d"))
							{
								if (value.Trim() == "00000000")
								{
									nullableDateValue = null;
									throw new SystemException("Invalid date.");
								}

								string v = value.Trim();
								int year = int.Parse(v.Substring(0, 4));
								int month = int.Parse(v.Substring(4, 2));
								int day = int.Parse(v.Substring(6, 2));

								try
								{
									nullableDateValue = new System.DateTime(year, month, day);
								}
								catch (Exception e2)
								{
									throw new SystemException("Invalid date.", e2);
								}
							}
							else
							{
								throw new SystemException("Invalid date.");
							}
						}
						catch (Exception e2)
						{
							throw new SystemException("Invalid date.", e2);
						}

						break;
				}
			}

			try
			{
				if (nullableDateValue == null)
				{
					return null;
				}

				DateTime dateValue = nullableDateValue.Value;
				if (dateValue < System.DateTime.Parse("1/1/1753"))
				{
					throw new SystemException("Invalid date.");
				}

				return dateValue;
			}
			catch (Exception e)
			{
				throw new SystemException("Invalid date.", e);
			}
		}

		/// <summary>
		/// Casts the provided nullable boolean to a string, utilizing the provided format string.
		/// </summary>
		/// <param name="value">
		/// The nullable boolean to convert.
		/// </param>
		/// <returns>
		/// A string representation of <paramref name="value"/>.
		/// </returns>
		public static string ToEmptyStringOrValue(bool? value)
		{
			return value == null ? string.Empty : value.Value.ToString();
		}

		/// <summary>
		/// Casts the provided nullable decimal to a string, utilizing the provided format string.
		/// </summary>
		/// <param name="value">
		/// The nullable decimal to convert.
		/// </param>
		/// <returns>
		/// A string representation of <paramref name="value"/>.
		/// </returns>
		public static string ToEmptyStringOrValue(decimal? value)
		{
			return !value.HasValue ? string.Empty : value.Value.ToString();
		}

		/// <summary>
		/// Casts the provided nullable integer to a string, utilizing the provided format string.
		/// </summary>
		/// <param name="value">
		/// The nullable boolean to convert.
		/// </param>
		/// <returns>
		/// A string representation of <paramref name="value"/>.
		/// </returns>
		public static string ToEmptyStringOrValue(int? value)
		{
			return value == null ? string.Empty : value.Value.ToString();
		}

		/// <summary>
		/// Casts the provided nullable date to a string.
		/// </summary>
		/// <param name="value">
		/// The nullable date or time to convert.
		/// </param>
		/// <returns>
		/// <see cref="string.Empty"/> if <see paramref="value"/> is <see langword="null" />; otherwise, <see paramref="value"/>.
		/// </returns>
		public static string ToEmptyStringOrValue(DateTime? value)
		{
			const bool UseUniversalFormat = false;
			return ToEmptyStringOrValue(value, UseUniversalFormat);
		}

		/// <summary>
		/// Casts the provided nullable date to a string.
		/// </summary>
		/// <param name="value">
		/// The nullable date or time to convert.
		/// </param>
		/// <param name="useUniversalFormat">
		/// <see langword="true" /> to parse the string in a culture-independent way; otherwise, <see langword="false" />.
		/// </param>
		/// <returns>
		/// <see cref="string.Empty"/> if <see paramref="value"/> is <see langword="null" />; otherwise, <see paramref="value"/>.
		/// </returns>
		public static string ToEmptyStringOrValue(DateTime? value, bool useUniversalFormat)
		{
			if (value == null)
			{
				return string.Empty;
			}

			return !useUniversalFormat ? value.Value.ToString() : value.Value.ToSqlCultureNeutralString();
		}

		/// <summary>
		/// Casts the provided string to <see cref="string.Empty"/> if it is null.
		/// </summary>
		/// <param name="value">
		/// The nullable string to convert.
		/// </param>
		/// <returns>
		/// <see cref="string.Empty"/> if <see paramref="value"/> is <see langword="null" />; otherwise, <see paramref="value"/>.
		/// </returns>
		public static string ToEmptyStringOrValue(string value)
		{
			return string.IsNullOrWhiteSpace(value) ? string.Empty : value;
		}

		/// <summary>
		/// Casts the provided string to a nullable boolean.
		/// </summary>
		/// <param name="value">
		/// The string to consider.
		/// </param>
		/// <returns>
		/// A nullable <see cref="bool"/> representation of <see param="value" />.
		/// </returns>
		public static bool? ToNullableBoolean(string value)
		{
			return string.IsNullOrWhiteSpace(value) ? null : new bool?(bool.Parse(value));
		}

		/// <summary>
		/// Casts the provided string to a nullable date, utilizing the provided culture-independent format string.
		/// </summary>
		/// <param name="value">
		/// The string to consider.
		/// </param>
		/// <returns>
		/// A nullable <see cref="DateTime"/> representation of <see param="value" />.
		/// </returns>
		public static DateTime? ToNullableDateTime(string value)
		{
			const bool UseUniversalFormat = false;
			return ToNullableDateTime(value, UseUniversalFormat);
		}

		/// <summary>
		/// Casts the provided string to a nullable date, utilizing the provided format string.
		/// </summary>
		/// <param name="value">
		/// The string to consider.
		/// </param>
		/// <param name="useUniversalFormat">
		/// <see langword="true" /> to parse the string in a culture-independent way; otherwise, <see langword="false" />.
		/// </param>
		/// <returns>
		/// A nullable <see cref="DateTime"/> representation of <see param="value" />.
		/// </returns>
		public static DateTime? ToNullableDateTime(string value, bool useUniversalFormat)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return null;
			}

			return !useUniversalFormat
				       ? System.DateTime.Parse(value)
				       : System.DateTime.Parse(value, System.Globalization.DateTimeFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Casts the provided string to a nullable decimal, utilizing the provided format type.
		/// </summary>
		/// <param name="value">
		/// The string to convert.
		/// </param>
		/// <param name="style">
		/// The type of decimal number style to respect during parsing.
		/// </param>
		/// <returns>
		/// A nullable decimal representation of <see param="value" />.
		/// </returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1026:DefaultParametersShouldNotBeUsed",
			Justification = "For backwards compatibility concerns.")]
		public static decimal? ToNullableDecimal(string value, NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol)
		{
			return !string.IsNullOrWhiteSpace(value)
				       ? new decimal?(decimal.Parse(value, style, (IFormatProvider)CultureInfo.InvariantCulture))
				       : null;
		}

		/// <summary>
		/// Casts the provided string to a nullable decimal, utilizing the current culture.
		/// </summary>
		/// <param name="value">
		/// The string to convert.
		/// </param>
		/// <param name="style">
		/// The type of decimal number style to respect during parsing.
		/// </param>
		/// <returns>
		/// A nullable decimal representation of <see param="value" />.
		/// </returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1026:DefaultParametersShouldNotBeUsed",
			Justification = "For backwards compatibility concerns.")]
		public static decimal? ToNullableDecimalUsingCurrentCulture(
			string value,
			NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol)
		{
			return !string.IsNullOrWhiteSpace(value)
				       ? new decimal?(decimal.Parse(value, style, (IFormatProvider)CultureInfo.CurrentCulture))
				       : null;
		}

		/// <summary>
		/// Casts the provided string to a nullable integer.
		/// </summary>
		/// <param name="value">
		/// The string to convert.
		/// </param>
		/// <returns>
		/// A nullable integer representation of <see param="convertMe" />.
		/// </returns>
		/// <exception cref="T:System.OverflowException">
		/// Thrown when <paramref name="value"/> represents a value outside the bounds of
		/// <see cref="int.MinValue"/> and <see cref="int.MaxValue"/> or
		/// the format of <paramref name="value"/> does not comply with
		/// <see cref="NumberStyles.Integer"/> or <see cref="NumberStyles.AllowThousands"/>.
		/// </exception>
		public static int? ToNullableInt32(string value)
		{
			return !string.IsNullOrWhiteSpace(value)
				       ? int.Parse(value, NumberStyles.Integer | NumberStyles.AllowThousands)
				       : default(int?);
		}

		/// <summary>
		/// Gets the string representation of the provided string.
		/// </summary>
		/// <param name="value">
		/// The string to consider.
		/// </param>
		/// <returns>
		/// Nothing if <see param="convertMe" /> is null or empty; otherwise, <see param="convertMe" />.
		/// </returns>
		public static string ToString(string value)
		{
			return string.IsNullOrWhiteSpace(value) ? null : value;
		}
	}
}