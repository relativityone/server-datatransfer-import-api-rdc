// ----------------------------------------------------------------------------
// <copyright file="ImportStatusHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using Microsoft.VisualBasic;

	/// <summary>
	/// Defines static helper methods to perform common import status operations.
	/// </summary>
	internal static class ImportStatusHelper
	{
		/// <summary>
		/// Converts to message line in cell.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		/// <returns>
		/// The new message.
		/// </returns>
		public static string ConvertToMessageLineInCell(string message)
		{
			return string.Format(" - {0}{1}", message, Strings.ChrW(10));
		}
	}
}