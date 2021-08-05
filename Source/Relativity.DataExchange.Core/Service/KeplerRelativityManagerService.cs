// ----------------------------------------------------------------------------
// <copyright file="KeplerRelativityManagerService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
    using System;

    /// <inheritdoc />
    internal sealed class KeplerRelativityManagerService : IRelativityManagerService
    {
        private readonly RelativityInstanceInfo relativityInstanceInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeplerRelativityManagerService"/> class.
        /// </summary>
        /// <param name="relativityInstanceInfo">
        /// The Relativity instance information.
        /// </param>
        public KeplerRelativityManagerService(RelativityInstanceInfo relativityInstanceInfo)
        {
            this.relativityInstanceInfo = relativityInstanceInfo;
        }

        /// <inheritdoc/>
        public Uri GetRelativityUrl()
        {
            UriBuilder builder = new UriBuilder
                                     {
                                         Scheme = this.relativityInstanceInfo.WebApiServiceUrl.Scheme, Host = this.relativityInstanceInfo.WebApiServiceUrl.Host, Port = this.relativityInstanceInfo.WebApiServiceUrl.Port, Path = "Relativity",
                                     };
            return builder.Uri;
        }
    }
}