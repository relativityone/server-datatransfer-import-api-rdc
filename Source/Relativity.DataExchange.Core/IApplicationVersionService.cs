// ----------------------------------------------------------------------------
// <copyright file="IApplicationVersionService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// Represents abstract object that retrieve Relativity and import/export WebAPI version information.
	/// </summary>
	public interface IApplicationVersionService
	{
		/// <summary>
		/// Asynchronously retrieves the Relativity instance version.
		/// </summary>
		/// <param name="token">
		/// The token used to cancel the request.
		/// </param>
		/// <returns>
		/// The <see cref="Version"/> instance.
		/// </returns>
		/// <exception cref="HttpServiceException">
		/// The exception thrown when a serious failure occurs retrieving the version.
		/// </exception>
		Task<Version> GetRelativityVersionAsync(CancellationToken token);

		/// <summary>
		/// Asynchronously retrieves the import/export WebAPI version.
		/// </summary>
		/// <param name="token">
		/// The token used to cancel the request.
		/// </param>
		/// <returns>
		/// The <see cref="Version"/> instance.
		/// </returns>
		/// <exception cref="HttpServiceException">
		/// The exception thrown when a serious failure occurs retrieving the version or the web service doesn't exist.
		/// </exception>
		Task<Version> GetImportExportWebApiVersionAsync(CancellationToken token);
	}
}