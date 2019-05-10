// ----------------------------------------------------------------------------
// <copyright file="IObjectCacheRepository.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
    using System;

    /// <summary>
    /// Represents an abstract object cache.
    /// </summary>
    internal interface IObjectCacheRepository : IDisposable
    {
        /// <summary>
        /// Gets the total number of items in the cache.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        long Count
        {
            get;
        }

        /// <summary>
        /// Gets or sets the cache expiration applied to each item.
        /// </summary>
        /// <value>
        /// The <see cref="TimeSpan"/> value.
        /// </value>
        TimeSpan Expiration
        {
            get;
            set;
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determine whether the specified cached object associated with the <paramref name="key"/> is contained within the cache.
        /// </summary>
        /// <param name="key">
        /// The object cache key.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the object exists within the cache; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="key"/> is <see langword="null"/> or empty.
        /// </exception>
        bool Contains(string key);

        /// <summary>
        /// Deletes the object contained within the cache having the specified key.
        /// </summary>
        /// <param name="key">
        /// The object cache key.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="key"/> is <see langword="null"/> or empty.
        /// </exception>
        void Delete(string key);

        /// <summary>
        /// Selects a single cached object associated with the <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="TEntity">
        /// The entity type.
        /// </typeparam>
        /// <param name="key">
        /// The object cache key.
        /// </param>
        /// <returns>
        /// The object cache entity.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="key"/> is <see langword="null"/> or empty.
        /// </exception>
        TEntity SelectByKey<TEntity>(string key);

        /// <summary>
        /// Inserts or updates the specified cached object whose name matches the <paramref name="key"/>. If the object already exists within the cache, the value is replaced and the expiration period is updated with <see cref="Expiration"/>.
        /// </summary>
        /// <param name="key">
        /// The cached object key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="key"/> is <see langword="null"/> or empty.
        /// </exception>
        void Upsert(string key, object value);

        /// <summary>
        /// Inserts or updates the specified cached object whose name matches the <paramref name="key"/>. If the object already exists within the cache, the value is replaced and the expiration period is updated with the specified value.
        /// </summary>
        /// <param name="key">
        /// The cached object key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="expiration">
        /// The cache expiration timespan.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="key"/> is <see langword="null"/> or empty.
        /// </exception>
        void Upsert(string key, object value, TimeSpan expiration);
    }
}