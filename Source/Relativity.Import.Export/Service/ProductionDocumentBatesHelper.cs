// ----------------------------------------------------------------------------
// <copyright file="ProductionDocumentBatesHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Service
{
	using System;

	/// <summary>
	/// Defines static helper methods to perform common production document bates operations.
	/// </summary>
	internal static class ProductionDocumentBatesHelper
	{
		/// <summary>
		/// Performs serialization cleanup on all items within the supplied input array.
		/// </summary>
		/// <param name="input">
		/// The input to cleanup.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="input"/> is <see langword="null" />.
		/// </exception>
		public static void CleanupSerialization(object[][] input)
		{
			if (input == null)
			{
				throw new ArgumentNullException(nameof(input));
			}

			foreach (object[] item in input)
			{
				item[1] = System.Text.Encoding.Unicode.GetString((byte[])item[1]);
			}
		}
	}
}