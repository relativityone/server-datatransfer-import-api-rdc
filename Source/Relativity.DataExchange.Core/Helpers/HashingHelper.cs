// <copyright file="HashingHelper.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.Helpers
{
	using System.Security.Cryptography;
	using System.Text;

	/// <summary>
	/// It hashes string.
	/// </summary>
	public static class HashingHelper
	{
		private static readonly SHA256 Crypt = SHA256.Create();

		/// <summary>
		/// Calculate SHA256 hash of given value.
		/// </summary>
		/// <param name="value"> Value to hash.</param>
		/// <returns>Return SHA256 hash of given value.</returns>
		public static string CalculateSHA256Hash(string value)
		{
			value.ThrowIfNull(nameof(value));

			var hash = new System.Text.StringBuilder();
			byte[] crypto = Crypt.ComputeHash(Encoding.UTF8.GetBytes(value));
			foreach (byte theByte in crypto)
			{
				hash.Append(theByte.ToString("x2"));
			}

			return hash.ToString();
		}
	}
}
