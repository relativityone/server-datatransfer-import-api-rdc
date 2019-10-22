// ----------------------------------------------------------------------------
// <copyright file="UrlHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;

	/// <summary>
	/// Defines static methods to perform common URL operations.
	/// </summary>
	/// <remarks>
	/// REL-369504: these methods address URI/URL limitations found within the original VB.NET URI class.
	/// </remarks>
	internal static class UrlHelper
	{
		/// <summary>
		/// Gets the base URL including the scheme, host, and possible port number from the supplied URL.
		/// </summary>
		/// <param name="url">
		/// The URL.
		/// </param>
		/// <returns>
		/// The base URL.
		/// </returns>
		/// <exception cref="UriFormatException">
		/// Thrown when <paramref name="url"/> is not well-formed.
		/// </exception>
		public static string GetBaseUrl(string url)
		{
			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
			{
				throw new UriFormatException($"The base URL cannot be retrieved from {url} because it's not well-formed.");
			}

			Uri baseUri = new Uri(new Uri(url).GetLeftPart(UriPartial.Authority));
			string baseUrlString = baseUri.ToString();
			return baseUrlString;
		}

		/// <summary>
		/// Combines the base URL and relative URL parts into a single URL. Leading and trailing slashes are automatically added.
		/// </summary>
		/// <param name="baseUrl">
		/// The base URL. Trailing paths are automatically removed.
		/// </param>
		/// <param name="relativePath">
		/// The relative path to add to the <paramref name="baseUrl"/>.
		/// </param>
		/// <returns>
		/// The combined URL.
		/// </returns>
		/// <exception cref="UriFormatException">
		/// Thrown when <paramref name="baseUrl"/> is not well-formed.
		/// </exception>
		public static string GetBaseUrlAndCombine(string baseUrl, string relativePath)
		{
			return Combine(GetBaseUrl(baseUrl), relativePath);
		}

		/// <summary>
		/// Combines the absolute URL and relative URL parts into a single URL. Leading and trailing slashes are automatically added.
		/// </summary>
		/// <param name="absoluteUrl">
		/// The absolute URL.
		/// </param>
		/// <param name="relativePath">
		/// The relative path to add to the <paramref name="absoluteUrl"/>.
		/// </param>
		/// <returns>
		/// The combined URL.
		/// </returns>
		/// <exception cref="UriFormatException">
		/// Thrown when <paramref name="absoluteUrl"/> is not well-formed.
		/// </exception>
		public static string Combine(string absoluteUrl, string relativePath)
		{
			if (!Uri.IsWellFormedUriString(absoluteUrl, UriKind.Absolute))
			{
				throw new UriFormatException($"The URL {absoluteUrl} cannot be combined with '{relativePath}' because it's not well-formed.");
			}

			const string Separator = "/";
			Uri combinedUri = new Uri(new Uri(absoluteUrl), relativePath);
			string combinedUriString = combinedUri.ToString();
			if (!combinedUriString.EndsWith(Separator, StringComparison.OrdinalIgnoreCase))
			{
				combinedUriString += Separator;
			}

			return combinedUriString;
		}
	}
}