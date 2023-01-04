// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiFileStorageSearchResults.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object that provides a list of all file storage search results.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Collections.Generic;

	using Relativity.Transfer;

	/// <summary>
	/// Represents a class object that provides a list of all file storage search results.
	/// </summary>
	public class TapiFileStorageSearchResults : ITapiFileStorageSearchResults
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TapiFileStorageSearchResults"/> class.
		/// </summary>
		/// <param name="results">
		/// The results.
		/// </param>
		public TapiFileStorageSearchResults(FileStorageSearchResults results)
		{
			if (results == null)
			{
				throw new ArgumentNullException(nameof(results));
			}

			this.CloudInstance = results.CloudInstance;
			this.FileShares = results.FileShares;
			this.InvalidFileShares = results.InvalidFileShares;
		}

		/// <inheritdoc />
		public bool CloudInstance
		{
			get;
		}

		/// <inheritdoc />
		public IReadOnlyCollection<RelativityFileShare> FileShares
		{
			get;
		}

		/// <inheritdoc />
		public IReadOnlyCollection<RelativityFileShare> InvalidFileShares
		{
			get;
		}
	}
}