// ----------------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents extension methods for common dictionary-based operations.
	/// </summary>
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Gets the string value.
		/// </summary>
		/// <param name="dictionary">
		/// The dictionary.
		/// </param>
		/// <param name="name">
		/// The property name.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if not contained within this instance.
		/// </param>
		/// <returns>
		/// The value.
		/// </returns>
		public static string GetStringValue(this IDictionary<string, object> dictionary, string name, string defaultValue)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			if (ContainsNonNullValue(dictionary, name))
			{
				return dictionary[name].ToString();
			}

			return defaultValue;
		}

		/// <summary>
		/// Gets the 32-bit integer value.
		/// </summary>
		/// <param name="dictionary">
		/// The dictionary.
		/// </param>
		/// <param name="name">
		/// The property name.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if not contained within this instance.
		/// </param>
		/// <returns>
		/// The value.
		/// </returns>
		public static int GetInt32Value(this IDictionary<string, object> dictionary, string name, int defaultValue)
		{
			return GetInt32Value(dictionary, name, defaultValue, null);
		}

		/// <summary>
		/// Gets the 32-bit integer value.
		/// </summary>
		/// <param name="dictionary">
		/// The dictionary.
		/// </param>
		/// <param name="name">
		/// The property name.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if not contained within this instance.
		/// </param>
		/// <param name="minValue">
		/// The minimum value allowed for this setting. If the current value is invalid, the <paramref name="defaultValue"/> is used.
		/// </param>
		/// <returns>
		/// The value.
		/// </returns>
		public static int GetInt32Value(this IDictionary<string, object> dictionary, string name, int defaultValue, int? minValue)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			if (ContainsNonNullValue(dictionary, name))
			{
				int value;
				if (int.TryParse(dictionary[name].ToString(), out value))
				{
					if (minValue.HasValue && value < minValue)
					{
						return defaultValue;
					}

					return value;
				}
			}

			return defaultValue;
		}

		/// <summary>
		/// Gets the URI value.
		/// </summary>
		/// <param name="dictionary">
		/// The dictionary.
		/// </param>
		/// <param name="name">
		/// The property name.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if not contained within this instance.
		/// </param>
		/// <returns>
		/// The value.
		/// </returns>
		public static Uri GetUriValue(this IDictionary<string, object> dictionary, string name, Uri defaultValue)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			if (ContainsNonNullValue(dictionary, name))
			{
				try
				{
					return new Uri(dictionary[name].ToString());
				}
				catch (ArgumentNullException)
				{
					return null;
				}
				catch (UriFormatException)
				{
					return new Uri(dictionary[name].ToString(), UriKind.Relative);
				}
			}

			return defaultValue;
		}

		/// <summary>
		/// Gets the boolean value.
		/// </summary>
		/// <param name="dictionary">
		/// The dictionary.
		/// </param>
		/// <param name="name">
		/// The property name.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if not contained within this instance.
		/// </param>
		/// <returns>
		/// The value.
		/// </returns>
		public static bool GetBooleanValue(this IDictionary<string, object> dictionary, string name, bool defaultValue)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			if (ContainsNonNullValue(dictionary, name))
			{
				bool value;
				if (bool.TryParse(dictionary[name].ToString(), out value))
				{
					return value;
				}
			}

			return defaultValue;
		}

		/// <summary>
		/// Gets the enumeration value.
		/// </summary>
		/// <param name="dictionary">
		/// The dictionary.
		/// </param>
		/// <typeparam name="TEnum">
		/// The enumeration type.
		/// </typeparam>
		/// <param name="name">
		/// The property name.
		/// </param>
		/// <param name="defaultValue">
		/// The default value if not contained within this instance.
		/// </param>
		/// <returns>
		/// The value.
		/// </returns>
		public static TEnum GetEnumValue<TEnum>(this IDictionary<string, object> dictionary, string name, TEnum defaultValue)
			where TEnum : struct
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			if (ContainsNonNullValue(dictionary, name))
			{
				TEnum value;
				if (Enum.TryParse(dictionary[name].ToString(), out value))
				{
					return value;
				}
			}

			return defaultValue;
		}

		/// <summary>
		/// Determines whether the dictionary contains a non-null value.
		/// </summary>
		/// <param name="dictionary">
		/// The dictionary.
		/// </param>
		/// <param name="name">
		/// The name.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the dictionary contains a non-null value; otherwise, <see langword="false" />.
		/// </returns>
		private static bool ContainsNonNullValue(IDictionary<string, object> dictionary, string name)
		{
			return dictionary.ContainsKey(name) && dictionary[name] != null;
		}
	}
}