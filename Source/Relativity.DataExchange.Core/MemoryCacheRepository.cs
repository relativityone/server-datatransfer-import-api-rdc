// ----------------------------------------------------------------------------
// <copyright file="MemoryCacheRepository.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object used to provide memory-based cache.
// </summary>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Linq;
	using System.Runtime.Caching;
	using System.Text;

	/// <summary>
	/// Represents a class object used to provide memory-based cache.
	/// </summary>
#if !DEBUG
	[System.Diagnostics.DebuggerStepThrough]
#endif
	internal class MemoryCacheRepository : IObjectCacheRepository
	{
		/// <summary>
		/// The default expiration time span.
		/// </summary>
		private static readonly TimeSpan DefaultExpirationTimeSpan = TimeSpan.FromHours(1);

		/// <summary>
		/// The memory cache backing.
		/// </summary>
		private readonly MemoryCache cache;

		/// <summary>
		/// The disposed backing.
		/// </summary>
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryCacheRepository"/> class.
		/// </summary>
		public MemoryCacheRepository()
			: this(DefaultExpirationTimeSpan)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryCacheRepository"/> class.
		/// </summary>
		/// <param name="expiration">
		/// The cache expiration.
		/// </param>
		public MemoryCacheRepository(TimeSpan expiration)
			: this(MemoryCache.Default, expiration)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryCacheRepository"/> class.
		/// </summary>
		/// <param name="cache">
		/// The memory cache instance.
		/// </param>
		/// <param name="expiration">
		/// The cache expiration.
		/// </param>
		public MemoryCacheRepository(MemoryCache cache, TimeSpan expiration)
		{
			this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
			this.Expiration = expiration;
		}

		/// <inheritdoc />
		public long Count => this.cache.GetCount();

		/// <inheritdoc />
		public TimeSpan Expiration
		{
			get;
			set;
		}

		/// <inheritdoc />
		public void Clear()
		{
			this.cache.ToList().ForEach(a => this.cache.Remove(a.Key));
		}

		/// <inheritdoc />
		public bool Contains(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			var result = this.cache.Contains(key);
			return result;
		}

		/// <inheritdoc />
		public void Delete(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (this.cache.Contains(key))
			{
				this.cache.Remove(key);
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		public TEntity SelectByKey<TEntity>(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			var result = (TEntity)this.cache.Get(key);
			return result;
		}

		/// <inheritdoc />
		public void Upsert(string key, object value)
		{
			this.Upsert(key, value, this.Expiration);
		}

		/// <inheritdoc />
		public void Upsert(string key, object value, TimeSpan expiration)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			var policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.Add(expiration) };
			this.cache.Set(key, value, policy);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			var stringBuilder = new StringBuilder();
			foreach (var item in this.cache)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(";");
				}

				stringBuilder.AppendFormat("{0}={1}", item.Key, item.Value);
			}

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing">
		/// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}

			if (disposing)
			{
				// Nothing to dispose.
			}

			// Note: Release unmanaged resources here.
			this.disposed = true;
		}
	}
}