// ----------------------------------------------------------------------------
// <copyright file="IRelativityVersionService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Relativity.Import.Export
{
	using System;

	/// <summary>
	/// Represents abstract object that retrieve Relativity and ImportExportWebApi versions.
	/// </summary>
	public interface IRelativityVersionService
	{
		/// <summary>
		/// It retrieves Relativity instance version.
		/// </summary>
		/// <returns>Version object.</returns>
		Version RetrieveRelativityVersion();

		/// <summary>
		/// It retrieves Web Api version.
		/// </summary>
		/// <returns>Version object.</returns>
		Version RetrieveImportExportWebApiVersion();
	}
}
