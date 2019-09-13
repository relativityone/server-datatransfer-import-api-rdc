// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITapiFileStorageSearchResults.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract object that provides a list of all file storage search results.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System.Collections.Generic;

	using Relativity.Transfer;

	/// <summary>
	/// Represents an abstract object that provides a list of all file storage search results.
	/// </summary>
	public interface ITapiFileStorageSearchResults
	{
		/// <summary>
		/// Gets a value indicating whether the Relativity is configured for a cloud instance.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if configured for a cloud instance; otherwise, <see langword="false" />.
		/// </value>
		bool CloudInstance
		{
			get;
		}

		/// <summary>
		/// Gets a read-only collection of the valid file shares.
		/// </summary>
		/// <value>
		/// The <see cref="RelativityFileShare"/> instances.
		/// </value>
		IReadOnlyCollection<RelativityFileShare> FileShares
		{
			get;
		}

		/// <summary>
		/// Gets a read-only collection of invalid file shares.
		/// </summary>
		/// <value>
		/// The <see cref="RelativityFileShare"/> instances.
		/// </value>
		IReadOnlyCollection<RelativityFileShare> InvalidFileShares
		{
			get;
		}
	}
}