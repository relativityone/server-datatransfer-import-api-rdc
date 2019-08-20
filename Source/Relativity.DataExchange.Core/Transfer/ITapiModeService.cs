// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITapiModeService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract object to expose Transfer API transfer mode operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System.Collections.Generic;

	/// <summary>
	/// Represents an abstract object to expose Transfer API transfer mode operations.
	/// </summary>
	public interface ITapiModeService
	{
		/// <summary>
		/// Dynamically builds the file transfer mode documentation text.
		/// </summary>
		/// <param name="includeBulk">
		/// <see langword="true" /> to include bulk load details; otherwise, <see langword="false" />.
		/// </param>
		/// <returns>
		/// The help text.
		/// </returns>
		string BuildDocText(bool includeBulk);

		/// <summary>
		/// Builds the import file transfer mode status text from the native and metadata transfer clients.
		/// </summary>
		/// <param name="nativeFilesCopied">
		/// <see langword="true" /> to copy all natives; otherwise, <see langword="false" /> skips copying natives.
		/// </param>
		/// <param name="native">
		/// Specify the client used to transfer native files.
		/// </param>
		/// <param name="metadata">
		/// Specify the client used to transfer metadata files.
		/// </param>
		/// <returns>
		/// The status text.
		/// </returns>
		string BuildImportStatusText(bool nativeFilesCopied, TapiClient? native, TapiClient? metadata);

		/// <summary>
		/// Builds the import file transfer mode status text from th list of native transfer clients.
		/// </summary>
		/// <param name="natives">
		/// The list of native transfer clients.
		/// </param>
		/// <returns>
		/// The status text.
		/// </returns>
		string BuildExportStatusText(IEnumerable<TapiClient> natives);
	}
}