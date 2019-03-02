// ----------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Defines static <see cref="string"/> extension methods.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Converts the input string to replace the system newline characters with the provided new line proxy; doubles any existing bound strings in the input.
		/// </summary>
		/// <param name="input">
		/// The input to convert.
		/// </param>
		/// <param name="bound">
		/// The bound string to find.
		/// </param>
		/// <param name="newlineProxy">
		/// The new line proxy string with which to replace system new line characters.
		/// </param>
		/// <returns>
		/// A manipulated representation of <see param="input"/> with new line proxies and doubled bounds.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown when <paramref name="bound"/> is empty.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="bound"/> is <see langword="null" />.
		/// </exception>
		/// <exception cref="NullReferenceException">
		/// Thrown when <paramref name="input"/> is <see langword="null" />.
		/// </exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1062:Validate arguments of public methods",
			MessageId = "0",
			Justification = "The original implementation was explicitly designed to throw NullReferenceException.")]
		public static string ToDelimitedFileCellContents(this string input, string bound, string newlineProxy)
		{
			string returnValue = input.Replace(VisualBasicConstants.VbNewLine, newlineProxy);
			returnValue = returnValue.Replace(VisualBasicConstants.VbCr, newlineProxy);
			returnValue = returnValue.Replace(VisualBasicConstants.VbLf, newlineProxy);
			returnValue = returnValue.Replace(bound, bound + bound);
			return returnValue;
		}

		/// <summary>
		/// Converts the input string to a CSV cell content representation. This method uses double-quotes for the bound
		/// and the newline character for the newline proxy.
		/// </summary>
		/// <param name="input">
		/// The input to convert.
		/// </param>
		/// <returns>
		/// A manipulated representation of <see param="input"/> with new line characters replaced with a line-feed character.
		/// </returns>
		/// <exception cref="NullReferenceException">
		/// Thrown when <paramref name="input"/> is <see langword="null" />.
		/// </exception>
		public static string ToCsvCellContents(this string input)
		{
			return ToDelimitedFileCellContents(input, "\"", "\n");
		}

		/// <summary>
		/// Converts the input string to a SQL friendly name by removing all non-word characters.
		/// </summary>
		/// <param name="input">
		/// The input to convert.
		/// </param>
		/// <returns>
		/// A manipulated representation of <see param="input"/> with non-word characters removed.
		/// </returns>
		public static string ToSqlFriendlyName(this string input)
		{
			return Regex.Replace(input ?? string.Empty, "[\\W]+", string.Empty);
		}
	}
}