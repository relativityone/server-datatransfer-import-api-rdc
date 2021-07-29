// ----------------------------------------------------------------------------
// <copyright file="IRelativityManagerServiceFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	/// <summary>
	/// Factory to create Relativity Manager Service.
	/// </summary>
	public interface IRelativityManagerServiceFactory
	{
		/// <summary>
		/// Creates Relativity Manager Service.
		/// </summary>
		/// <param name="relativityInstanceInfo">RelativityInstanceInfo.</param>
		/// <param name="useLegacyWebApi">If true use WebApi, otherwise use Kepler.</param>
		/// <returns>Relativity Manager Service.</returns>
		IRelativityManagerService Create(RelativityInstanceInfo relativityInstanceInfo, bool useLegacyWebApi);
	}
}