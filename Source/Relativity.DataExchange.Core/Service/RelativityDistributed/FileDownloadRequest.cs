// <copyright file="FileDownloadRequest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.RelativityDistributed
{
	/// <summary>
	/// This type represents file download request to Relativity.Distributed service.
	/// </summary>
	internal class FileDownloadRequest
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileDownloadRequest"/> class.
		/// </summary>
		/// <param name="destinationFilePath">Path where file will be downloaded.</param>
		/// <param name="workspaceId">Workspace Id.</param>
		/// <param name="remoteFileGuid">Guid of an error file.</param>
		public FileDownloadRequest(string destinationFilePath, string workspaceId, string remoteFileGuid)
		{
			this.DestinationFilePath = destinationFilePath;
			this.WorkspaceId = workspaceId;
			this.RemoteFileGuid = remoteFileGuid;
		}

		/// <summary>
		/// Gets DestinationFilePath.
		/// </summary>
		public string DestinationFilePath { get; }

		/// <summary>
		/// Gets WorkspaceId.
		/// </summary>
		public string WorkspaceId { get; }

		/// <summary>
		/// Gets RemoteFileGuid.
		/// </summary>
		public string RemoteFileGuid { get; }
	}
}
