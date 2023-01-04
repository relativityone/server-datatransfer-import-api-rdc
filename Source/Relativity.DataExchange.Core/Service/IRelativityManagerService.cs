// ----------------------------------------------------------------------------
// <copyright file="IRelativityManagerService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
    using System;

    /// <summary>
    /// Represents a class object to provide a RelativityManager wrapper.
    /// </summary>
    public interface IRelativityManagerService
    {
        /// <summary>
        /// Retrieves the Relativity URL from the cache or from a WebAPI service.
        /// </summary>
        /// <returns>
        /// The <see cref="Uri" /> instance.
        /// </returns>
        Uri GetRelativityUrl();
    }
}