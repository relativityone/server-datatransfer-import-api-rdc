// ----------------------------------------------------------------------------
// <copyright file="IApplicationVersionService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Relativity.Import.Export
{
	using System;
	using System.Threading.Tasks;

	/// <summary>
	/// Represents abstract object that retrieve Relativity and ImportExportWebApi versions.
	/// </summary>
	public interface IApplicationVersionService
	{
		/// <summary>
		/// It retrieves Relativity instance version.
		/// </summary>
		/// <returns>Version object.</returns>
		Task<Version> RetrieveRelativityVersion();

		/// <summary>
		/// It retrieves Web Api version.
		/// </summary>
		/// <returns>Version object.</returns>
		Task<Version> RetrieveImportExportWebApiVersion();
	}
}
