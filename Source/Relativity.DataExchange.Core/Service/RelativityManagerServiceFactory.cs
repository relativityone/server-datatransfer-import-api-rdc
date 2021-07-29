// <copyright file="RelativityManagerServiceFactory.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service
{
	using System;

	/// <summary>
	/// Factory.
	/// </summary>
	internal class RelativityManagerServiceFactory : IRelativityManagerServiceFactory
	{
		/// <summary>
		/// Creates Relativity Manager Service.
		/// </summary>
		/// <param name="relativityInstanceInfo">RelativityInstanceInfo.</param>
		/// <param name="useLegacyWebApi">If true use WebApi, otherwise use Kepler.</param>
		/// <returns>Relativity Manager Service.</returns>
		public IRelativityManagerService Create(
			RelativityInstanceInfo relativityInstanceInfo,
			bool useLegacyWebApi)
		{
			if (relativityInstanceInfo == null)
			{
				throw new ArgumentNullException(nameof(relativityInstanceInfo));
			}

			if (useLegacyWebApi)
			{
				return new RelativityManagerService(relativityInstanceInfo);
			}

			return new KeplerRelativityManagerService(relativityInstanceInfo);
		}
	}
}