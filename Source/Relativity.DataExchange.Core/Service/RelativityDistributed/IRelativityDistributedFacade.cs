// <copyright file="IRelativityDistributedFacade.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.RelativityDistributed
{
	/// <summary>
	/// This is a facade for Relativity.Distributed service which provides methods for downloading error file.
	/// </summary>
	internal interface IRelativityDistributedFacade
	{
		/// <summary>
		/// It downloads an error file using Relativity.Distributed service.
		/// </summary>
		/// <param name="request">Request.</param>
		/// <returns>Response.</returns>
		FileDownloadResponse DownloadFile(FileDownloadRequest request);
	}
}
