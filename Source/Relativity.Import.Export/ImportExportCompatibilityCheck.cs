// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportExportCompatibilityCheck.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to perform compatibility checks between the client API and Relativity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Globalization;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;

	using Relativity.Logging;

	/// <summary>
	/// Represents a class object to perform compatibility checks between the client API and Relativity.
	/// </summary>
	internal class ImportExportCompatibilityCheck : IImportExportCompatibilityCheck
	{
		private static readonly IObjectCacheRepository DefaultObjectCacheRepository = new MemoryCacheRepository(TimeSpan.FromMinutes(15));
		private readonly RelativityInstanceInfo instanceInfo;
		private readonly ILog log;
		private readonly IApplicationVersionService applicationVersionService;
		private readonly Version minRelativityVersion;
		private readonly Version requiredWebApiVersion;
		private readonly Version webApiStartFromRelativityVersion;
		private readonly IObjectCacheRepository objectCacheRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImportExportCompatibilityCheck"/> class.
		/// </summary>
		/// <param name="instanceInfo">
		/// The Relativity info information.
		/// </param>
		/// <param name="applicationVersionService">
		/// The application version service object.
		/// </param>
		/// <param name="log">
		/// the log object.
		/// </param>
		public ImportExportCompatibilityCheck(
			RelativityInstanceInfo instanceInfo,
			IApplicationVersionService applicationVersionService,
			ILog log)
			: this(
				instanceInfo,
				applicationVersionService,
				log,
				VersionConstants.MinRelativityVersion,
				VersionConstants.RequiredWebApiVersion,
				VersionConstants.WebApiStartFromRelativityVersion,
				DefaultObjectCacheRepository)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImportExportCompatibilityCheck"/> class.
		/// </summary>
		/// <param name="instanceInfo">
		/// The Relativity info information.
		/// </param>
		/// <param name="applicationVersionService">
		/// The application version service object.
		/// </param>
		/// <param name="log">
		/// The Relativity Logging logger instance.
		/// </param>
		/// <param name="minRelativityVersion">
		/// The minimum Relativity version.
		/// </param>
		/// <param name="requiredWebApiVersion">
		/// The required WebAPI version.
		/// </param>
		/// <param name="webApiStartFromRelativityVersion">
		/// The required Relativity version from which it support WebAPI version.
		/// </param>
		/// <param name="cacheRepository">
		/// The object cache repository.
		/// </param>
		public ImportExportCompatibilityCheck(
			RelativityInstanceInfo instanceInfo,
			IApplicationVersionService applicationVersionService,
			ILog log,
			Version minRelativityVersion,
			Version requiredWebApiVersion,
			Version webApiStartFromRelativityVersion,
			IObjectCacheRepository cacheRepository)
		{
			this.webApiStartFromRelativityVersion = webApiStartFromRelativityVersion;
			this.instanceInfo = instanceInfo.ThrowIfNull(nameof(instanceInfo));
			this.log = log.ThrowIfNull(nameof(log));
			this.applicationVersionService = applicationVersionService.ThrowIfNull(nameof(applicationVersionService));
			this.minRelativityVersion = minRelativityVersion.ThrowIfNull(nameof(minRelativityVersion));
			this.requiredWebApiVersion = requiredWebApiVersion.ThrowIfNull(nameof(requiredWebApiVersion));
			this.objectCacheRepository = cacheRepository.ThrowIfNull(nameof(cacheRepository));
		}

		/// <inheritdoc />
		public async Task ValidateAsync(CancellationToken token)
		{
			InstanceCheckCacheItem item =
				new InstanceCheckCacheItem { Exception = null, Validated = false, RelativityVersion = null };
			string cacheKey = this.CreateCacheKey();
			if (this.objectCacheRepository.Contains(cacheKey))
			{
				item = this.objectCacheRepository.SelectByKey<InstanceCheckCacheItem>(cacheKey);
				this.log.LogDebug(
					"Instance check cache item cache hit. RelativityVersion={RelativityVersion}, ImportExportWebApiVersion={ImportExportWebApiVersion}",
					item.RelativityVersion,
					item.ImportExportWebApiVersion);
				if (item.Validated)
				{
					return;
				}

				throw item.Exception;
			}

			this.log.LogInformation(
				"Retrieving the Relativity version for Relativity instance {RelativityHost}...",
				this.instanceInfo.Host);
			Version relativityVersion = await this.applicationVersionService
				                            .GetRelativityVersionAsync(token).ConfigureAwait(false);
			this.log.LogInformation(
				"Successfully retrieved Relativity version {RelativityVersion} for Relativity instance {RelativityHost}.",
				relativityVersion,
				this.instanceInfo.Host);
			item.RelativityVersion = relativityVersion;

			this.VerifyRelativityVersionFormat(relativityVersion, item, cacheKey);

			try
			{
				await this.PerformValidationAsync(token, relativityVersion, item, cacheKey);
			}
			catch (RelativityNotSupportedException e)
			{
				item.Validated = false;
				item.Exception = e;
				throw;
			}
			catch (HttpServiceException e)
			{
				item.Validated = false;
				item.Exception = e;
				throw;
			}
			finally
			{
				this.objectCacheRepository.Upsert(cacheKey, item);
			}
		}

		private async Task PerformValidationAsync(
			CancellationToken token,
			Version relativityVersion,
			InstanceCheckCacheItem item,
			string cacheKey)
		{
			if (relativityVersion < this.webApiStartFromRelativityVersion)
			{
				// Perfrom validation for older Relativity Version - this code should be removed on compatibility break change
				this.ValidateRelativityVersion(relativityVersion);
			}
			else
			{
				this.log.LogInformation(
					"Retrieving the import/export WebAPI version for Relativity instance {RelativityHost}...",
					this.instanceInfo.Host);
				Version importExportWebApiVersion = await this.applicationVersionService
					                                    .GetImportExportWebApiVersionAsync(token).ConfigureAwait(false);
				this.log.LogInformation(
					"Successfully retrieved the import/export WebAPI version {ImportExportWebApiVersion} for Relativity instance {RelativityHost}.",
					importExportWebApiVersion,
					this.instanceInfo.Host);
				item.ImportExportWebApiVersion = importExportWebApiVersion;
				this.log.LogInformation(
					"Preparing to perform the client/server API version compatibility check for Relativity instance {RelativityHost}...",
					this.instanceInfo.Host);
				this.VerifyImportExportWebApiVersion(relativityVersion, importExportWebApiVersion);
				this.log.LogInformation(
					"Successfully performed the client/server API version compatibility check for Relativity instance {RelativityHost}.",
					this.instanceInfo.Host);
				item.Validated = true;
				item.Exception = null;
				this.objectCacheRepository.Upsert(cacheKey, item);
			}
		}

		private void VerifyRelativityVersionFormat(Version relativityVersion, InstanceCheckCacheItem item, string cacheKey)
		{
			// Make sure the version is logically sound.
			if (relativityVersion.Major <= 0)
			{
				this.log.LogError(
					"The Relativity version {RelativityVersion} is invalid and import/export service cannot be used for Relativity instance {RelativityHost}.",
					relativityVersion,
					this.instanceInfo.Host);
				string message = string.Format(
					CultureInfo.CurrentCulture,
					Relativity.Import.Export.Resources.Strings.RelativtyMinVersionInvalidExceptionMessage,
					this.instanceInfo.Host,
					relativityVersion,
					this.minRelativityVersion);
				RelativityNotSupportedException exception = new RelativityNotSupportedException(message, relativityVersion);
				item.Exception = exception;
				item.Validated = false;
				this.objectCacheRepository.Upsert(cacheKey, item);
				throw exception;
			}
		}

		private void VerifyImportExportWebApiVersion(Version relativityVersion, Version importExportWebApiVersion)
		{
			// Make sure the version is logically sound.
			if (importExportWebApiVersion.Major <= 0)
			{
				this.log.LogError(
					"The import/export WebAPI version {ImportExportWebApiVersion} is invalid and import/export service cannot be used.",
					importExportWebApiVersion);
				string message = string.Format(
					CultureInfo.CurrentCulture,
					Relativity.Import.Export.Resources.Strings.ImportExportWebApiVersionInvalidExceptionMessage,
					this.instanceInfo.Host);
				throw new RelativityNotSupportedException(message, relativityVersion);
			}

			if (this.requiredWebApiVersion.Major != importExportWebApiVersion.Major)
			{
				this.log.LogError(
					"The import/export WebAPI version {ImportExportWebApiVersion} isn't compatible with the required client API version {RequiredWebApiVersion}.",
					importExportWebApiVersion,
					this.requiredWebApiVersion);
				string message = string.Format(
					CultureInfo.CurrentCulture,
					Relativity.Import.Export.Resources.Strings.ImportExportWebApiVersionNotSupportedExceptionMessage,
					this.instanceInfo.Host);
				throw new RelativityNotSupportedException(message, relativityVersion);
			}
		}

		private void ValidateRelativityVersion(Version relativityVersion)
		{
			// For older version of Relativity that don't support the WebAPI version, perform a min-version check.
			if (relativityVersion < this.minRelativityVersion)
			{
				string message = string.Format(
					CultureInfo.CurrentCulture,
					Relativity.Import.Export.Resources.Strings.RelativtyMinVersionExceptionMessage,
					this.instanceInfo.Host,
					relativityVersion,
					this.minRelativityVersion);
				throw new RelativityNotSupportedException(message, relativityVersion);
			}
		}

		private string CreateCacheKey()
		{
			return $"2619DD28-5BA7-4774-8D8C-278DE295254F.{this.instanceInfo.Host}".ToUpperInvariant();
		}

		private class InstanceCheckCacheItem
		{
			public bool Validated
			{
				get;
				set;
			}

			public Exception Exception
			{
				get;
				set;
			}

			public Version RelativityVersion
			{
				get;
				set;
			}

			public Version ImportExportWebApiVersion
			{
				get;
				set;
			}
		}
	}
}