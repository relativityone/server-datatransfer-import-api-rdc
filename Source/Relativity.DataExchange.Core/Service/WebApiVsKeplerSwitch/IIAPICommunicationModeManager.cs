// <copyright file="IIAPICommunicationModeManager.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.WebApiVsKeplerSwitch
{
	using System.Threading.Tasks;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models;

	/// <summary>
	/// Retrieves information about Import API communication mode <see cref="IAPICommunicationMode"/>.
	/// </summary>
	public interface IIAPICommunicationModeManager
	{
		/// <summary>
		/// Retrieves information about Import API communication mode <see cref="IAPICommunicationMode"/>.
		/// </summary>
		/// <returns>Import API communication mode <see cref="IAPICommunicationMode"/>.</returns>
		Task<IAPICommunicationMode> ReadImportApiCommunicationMode();
	}
}
