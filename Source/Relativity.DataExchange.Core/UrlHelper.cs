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
		/// Combines the absolute URL and relative URL into a single URL. Leading and trailing slashes are automatically handled.
		/// </summary>
		/// <param name="absoluteUrl">
		/// The absolute url. If you require the base URL, you're required to call <see cref="GetBaseUrl"/> separately.
		/// </param>
		/// <param name="relativeUrl">
		/// The relative URL to add to the <paramref name="absoluteUrl"/>.
		/// </param>
		/// <returns>
		/// The combined URL.
		/// </returns>
		/// <exception cref="UriFormatException">
		/// Thrown when <paramref name="absoluteUrl"/> is not well-formed.
		/// </exception>
		public static string Combine(string absoluteUrl, string relativeUrl)
		{
			if (!Uri.IsWellFormedUriString(absoluteUrl, UriKind.Absolute))
			{
				throw new UriFormatException($"The URL {absoluteUrl} cannot be combined with '{relativeUrl}' because it's not well-formed.");
			}

			const string Separator = "/";
			if (absoluteUrl.EndsWith(Separator, StringComparison.OrdinalIgnoreCase))
			{
				absoluteUrl = absoluteUrl.TrimEnd(Separator.ToCharArray());
			}

			if (!relativeUrl.StartsWith(Separator, StringComparison.OrdinalIgnoreCase))
			{
				relativeUrl = Separator + relativeUrl;
			}

			if (!relativeUrl.EndsWith(Separator, StringComparison.OrdinalIgnoreCase))
			{
				relativeUrl += Separator;
			}

			string combined = absoluteUrl + relativeUrl;
			return combined;
		}
	}
}