// ----------------------------------------------------------------------------
// <copyright file="IRelativityVersionService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Relativity.Import.Export.Versioning
{
	using System;

	/// <summary>
	/// Represents abstract object that retrieve Relativity and ImportExportWebApi versions.
	/// </summary>
	public interface IRelativityVersionService
	{
		/// <summary>
		/// It retrieve Relativity instance version.
		/// </summary>
		/// <returns>Version object.</returns>
		Version RetrieveRelativityVersion();

		/// <summary>
		/// It retrieve Relativity instance version.
		/// </summary>
		/// <returns>Version object.</returns>
		Version RetrieveImportExportWebApiVersion();
	}
}
