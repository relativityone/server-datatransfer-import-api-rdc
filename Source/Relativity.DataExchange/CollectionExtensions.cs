// ----------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Defines typed IEnumerable extension methods.
	/// </summary>
	internal static class CollectionExtensions
	{
		/// <summary>
		/// Transforms a sequence of values to a comma-delimited string.
		/// </summary>
		/// <typeparam name="T">
		/// The sequence type.
		/// </typeparam>
		/// <param name="sequence">
		/// The sequence to transform.
		/// </param>
		/// <returns>
		/// The delimited string representation of the sequence.
		/// </returns>
		public static string ToCsv<T>(this IEnumerable<T> sequence)
		{
			return ToCsv(sequence, arg => arg.ToString());
		}

		/// <summary>
		/// Transforms a sequence of values to a comma-delimited string, using the provided string transformation function.
		/// </summary>
		/// <typeparam name="T">
		/// The sequence type.
		/// </typeparam>
		/// <param name="sequence">
		/// The sequence to transform.
		/// </param>
		/// <param name="itemStringifier">
		/// The item stringifier.
		/// </param>
		/// <returns>
		/// The delimited string representation of the sequence.
		/// </returns>
		public static string ToCsv<T>(this IEnumerable<T> sequence, Func<T, string> itemStringifier)
		{
			// Pass in the value for backwards compatibility.
			return ToDelimitedString(sequence, ",", itemStringifier);
		}

		/// <summary>
		/// Transforms a sequence of values to a comma-delimited string, using the provided delimiter, bound and string transformation function.
		/// </summary>
		/// <typeparam name="T">
		/// The sequence type.
		/// </typeparam>
		/// <param name="sequence">
		/// The sequence to transform.
		/// </param>
		/// <param name="delimiter">
		/// The delimiter to use between values.
		/// </param>
		/// <param name="itemStringifier">
		/// The item stringifier.
		/// </param>
		/// <returns>
		/// The delimited string representation of the sequence.
		/// </returns>
		public static string ToDelimitedString<T>(
			this IEnumerable<T> sequence,
			string delimiter,
			Func<T, string> itemStringifier)
		{
			if (itemStringifier == null)
			{
				throw new ArgumentNullException(nameof(itemStringifier));
			}

			if (sequence == null)
			{
				return null;
			}

			// Do NOT optimize this with Resharper!
			if (!sequence.Any())
			{
				return string.Empty;
			}

			return string.Join(delimiter, sequence.Select(itemStringifier));
		}

		/// <summary>
		/// Transforms a sequence of values to a delimited string, using the provided delimiter, bound and format string.
		/// </summary>
		/// <typeparam name="T">
		/// The sequence type.
		/// </typeparam>
		/// <param name="sequence">
		/// The sequence to transform.
		/// </param>
		/// <param name="delimiter">
		/// The delimiter to use between values.
		/// </param>
		/// <param name="bound">
		/// Padding characters within which to wrap each value in the resulting string.
		/// </param>
		/// <param name="cellFormat">
		/// The format string for the ToString() method of each item.
		/// </param>
		/// <returns>
		/// A padded, comma-delimited string representation of the sequence.
		/// </returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1026:DefaultParametersShouldNotBeUsed",
			Justification = "For backwards compatibility concerns.")]
		public static string ToDelimitedString<T>(
			this IEnumerable<T> sequence,
			string delimiter = ",",
			string bound = "",
			string cellFormat = "{0}")
		{
			return sequence.ToDelimitedString(
				delimiter,
				x => string.Format(bound + cellFormat + bound, x.ToString()));
		}

		/// <summary>
		/// Determines whether or not the sequence is null or empty.
		/// </summary>
		/// <typeparam name="T">
		/// The sequence type.
		/// </typeparam>
		/// <param name="sequence">
		/// The sequence to check.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the sequence is null or empty; otherwise, <see langword="false" />.
		/// </returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence)
		{
			return sequence == null || !sequence.Any();
		}

		/// <summary>
		/// Determines whether the provided collection contains the provided value.
		/// </summary>
		/// <typeparam name="T">
		/// The sequence type.
		/// </typeparam>
		/// <param name="source">
		/// The value to find.
		/// </param>
		/// <param name="collection">
		/// The collection to search.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the collection contains the value item; otherwise, <see langword="false" />.
		/// </returns>
		public static bool In<T>(this T source, params T[] collection)
		{
			return collection.Contains(source);
		}

		/// <summary>
		/// Gets a subset of values from the provided enumerable.
		/// </summary>
		/// <typeparam name="T">
		/// The type of values contained within the enumerable.
		/// </typeparam>
		/// <param name="items">
		/// The enumerable from which to draw values.
		/// </param>
		/// <param name="beginIndex">
		/// The index from which to start taking values.
		/// </param>
		/// <param name="length">
		/// The number of values to take.
		/// </param>
		/// <returns>
		/// A subset of values from the provided enumerable.
		/// </returns>
		public static IEnumerable<T> GetRange<T>(this IEnumerable<T> items, int beginIndex, int length)
		{
			T[] returnItems = new T[length];
			List<T> itemsList = items.ToList();
			for (int i = beginIndex; i <= beginIndex + length - 1; i++)
			{
				returnItems[i] = itemsList[i];
			}

			return returnItems;
		}
	}
}