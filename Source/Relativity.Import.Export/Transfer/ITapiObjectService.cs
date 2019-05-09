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

	/// <summary>
	/// Represents an abstract object to provide Transfer API object services to the transfer bridges.
	/// </summary>
	public interface ITapiObjectService
	{
		/// <summary>
		/// Dynamically builds the file transfer mode documentation text.
		/// </summary>
		/// <param name="includeBulk">
		/// Specify whether to include bulk load details.
		/// </param>
		/// <returns>
		/// The help text.
		/// </returns>
		string BuildFileTransferModeDocText(bool includeBulk);

		/// <summary>
		/// Creates the file system service.
		/// </summary>
		/// <returns>
		/// The <see cref="Relativity.Transfer.IFileSystemService"/> instance.
		/// </returns>
		Relativity.Transfer.IFileSystemService CreateFileSystemService();

		/// <summary>
		/// Creates a Relativity connection information object and validates the endpoints.
		/// </summary>
		/// <param name="parameters">
		/// The Transfer API bridge parameters.
		/// </param>
		/// <returns>
		/// The <see cref="Relativity.Transfer.RelativityConnectionInfo"/> instance.
		/// </returns>
		Relativity.Transfer.RelativityConnectionInfo CreateRelativityConnectionInfo(TapiBridgeParameters2 parameters);

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
		/// The <see cref="Relativity.Transfer.IRelativityTransferHost"/> instance.
		/// </returns>
		Relativity.Transfer.IRelativityTransferHost CreateRelativityTransferHost(Relativity.Transfer.RelativityConnectionInfo connectionInfo, Relativity.Transfer.ITransferLog log);

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
		Guid GetClientId(TapiBridgeParameters2 parameters);

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
		Task<string> GetWorkspaceClientDisplayNameAsync(TapiBridgeParameters2 parameters);

		/// <summary>
		/// Asynchronously gets the Transfer API client Id that will be used for the given workspace.
		/// </summary>
		/// <param name="parameters">
		/// The bridge connection parameters.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/> value.
		/// </returns>
		Task<Guid> GetWorkspaceClientIdAsync(TapiBridgeParameters2 parameters);

		/// <summary>
		/// Sets the appropriate flags on <paramref name="parameters"/> to match the target Transfer API client.
		/// </summary>
		/// <param name="parameters">
		/// The parameters to set.
		/// </param>
		/// <param name="targetClient">
		/// The target Transfer API client.
		/// </param>
		void SetTapiClient(TapiBridgeParameters2 parameters, TapiClient targetClient);
	}
}