// <copyright file="IServiceAvailabilityChecker.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.WebApiVsKeplerSwitch
{
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models;

	/// <summary>
	/// Retrieves information about endpoints availability for WebApi and Kepler services.
	/// </summary>
	public interface IServiceAvailabilityChecker
	{
		/// <summary>
		/// Checks if WebApi service is available.
		/// </summary>
		/// <returns>WebApi availability flag.</returns>
		bool IsWebApiAvailable();

		/// <summary>
		/// Checks if Kepler service is available.
		/// </summary>
		/// <returns>Kepler availability flag.</returns>
		bool IsKeplerAvailable();

		/// <summary>
		/// Retrieves information about import api communication mode <see cref="IAPICommunicationMode"/>.
		/// </summary>
		/// <returns>Import api communication mode <see cref="IAPICommunicationMode"/>.</returns>
		IAPICommunicationMode? ReadImportApiCommunicationMode();
	}
}
