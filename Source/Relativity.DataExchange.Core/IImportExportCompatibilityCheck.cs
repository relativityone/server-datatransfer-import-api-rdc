// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImportExportCompatibilityCheck.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract object to perform compatibility checks between the client API and Relativity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// Represents an abstract object to perform compatibility checks between the client API and Relativity.
	/// </summary>
	public interface IImportExportCompatibilityCheck
	{
		/// <summary>
		/// Asynchronously checks whether the client API and target Relativity instance versions are compatible and throws <see cref="RelativityNotSupportedException"/> when the validation check fails.
		/// </summary>
		/// <param name="token">
		/// The token used to cancel the request.
		/// </param>
		/// <returns>
		/// The <see cref="Task"/> instance.
		/// </returns>
		/// <exception cref="HttpServiceException">
		/// The exception thrown when a serious HTTP failure occurs calling web-services.
		/// </exception>
		/// <exception cref="RelativityNotSupportedException">
		/// The exception thrown when Relativity isn't supported.
		/// </exception>
		Task ValidateAsync(CancellationToken token);
	}
}