// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITapiObjectService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract object to provide Transfer API object services to the transfer bridges.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
{
	using System;
	using System.Threading.Tasks;

	using Relativity.Transfer;

	/// <summary>
	/// Represents an abstract object to provide Transfer API object services to the transfer bridges.
	/// </summary>
	public interface ITapiObjectService
	{
		/// <summary>
		/// Searches for all available clients and builds the documentation text from the discovered metadata.
		/// </summary>
		/// <returns>
		/// The documentation text.
		/// </returns>
		string BuildDocText();

		/// <summary>
		/// Creates the file system service.
		/// </summary>
		/// <returns>
		/// The <see cref="IFileSystemService"/> instance.
		/// </returns>
		IFileSystemService CreateFileSystemService();

		/// <summary>
		/// Creates a Relativity connection information object and validates the endpoints.
		/// </summary>
		/// <param name="parameters">
		/// The Transfer API bridge parameters.
		/// </param>
		/// <returns>
		/// The <see cref="RelativityConnectionInfo"/> instance.
		/// </returns>
		RelativityConnectionInfo CreateRelativityConnectionInfo(TapiBridgeParameters parameters);

		/// <summary>
		/// Creates the relativity transfer host object.
		/// </summary>
		/// <param name="connectionInfo">
		/// The Relativity connection information.
		/// </param>
		/// <param name="log">
		/// The transfer log.
		/// </param>
		/// <returns>
		/// The <see cref="IRelativityTransferHost"/> instance.
		/// </returns>
		IRelativityTransferHost CreateRelativityTransferHost(RelativityConnectionInfo connectionInfo, ITransferLog log);

		/// <summary>
		/// Gets the client display name associated with the specified transfer client identifier.
		/// </summary>
		/// <param name="clientId">
		/// The transfer client identifier.
		/// </param>
		/// <returns>
		/// The client display name.
		/// </returns>
		/// <exception cref="System.ArgumentException">
		/// Thrown when the client doesn't exist.
		/// </exception>
		string GetClientDisplayName(Guid clientId);

		/// <summary>
		/// Gets the client identifier.
		/// </summary>
		/// <param name="parameters">
		/// The parameters.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/> value.
		/// </returns>
		Guid GetClientId(TapiBridgeParameters parameters);

		/// <summary>
		/// Gets the transfer API client from the unique identifier.
		/// </summary>
		/// <param name="clientId">
		/// The client identifier.
		/// </param>
		/// <returns>
		/// The <see cref="TapiClient"/> value.
		/// </returns>
		TapiClient GetTapiClient(Guid clientId);

		/// <summary>
		/// Asynchronously gets the Transfer API client display name that will be used for the given workspace.
		/// </summary>
		/// <param name="parameters">
		/// The bridge connection parameters.
		/// </param>
		/// <returns>
		/// The client display name.
		/// </returns>
		Task<string> GetWorkspaceClientDisplayNameAsync(TapiBridgeParameters parameters);

		/// <summary>
		/// Asynchronously gets the Transfer API client Id that will be used for the given workspace.
		/// </summary>
		/// <param name="parameters">
		/// The bridge connection parameters.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/> value.
		/// </returns>
		Task<Guid> GetWorkspaceClientIdAsync(TapiBridgeParameters parameters);

		/// <summary>
		/// Sets the appropriate flags on <paramref name="parameters"/> to match the target Transfer API client.
		/// </summary>
		/// <param name="parameters">
		/// The parameters to set.
		/// </param>
		/// <param name="targetClient">
		/// The target Transfer API client.
		/// </param>
		void SetTapiClient(TapiBridgeParameters parameters, TapiClient targetClient);
	}
}