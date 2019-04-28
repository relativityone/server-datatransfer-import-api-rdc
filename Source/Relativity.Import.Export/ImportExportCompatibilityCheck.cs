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
		private readonly RelativityInstanceInfo instanceInfo;
		private readonly ILog log;
		private readonly IApplicationVersionService applicationVersionService;
		private readonly Version minRelativityVersion;
		private readonly Version minWebApiVersion;

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
		public ImportExportCompatibilityCheck(RelativityInstanceInfo instanceInfo, IApplicationVersionService applicationVersionService, ILog log)
			: this(
				instanceInfo,
				applicationVersionService,
				log,
				VersionConstants.MinRelativityVersion,
				VersionConstants.MinWebApiVersion)
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
		/// <param name="minWebApiVersion">
		/// The minimum WebAPI version.
		/// </param>
		public ImportExportCompatibilityCheck(
			RelativityInstanceInfo instanceInfo,
			IApplicationVersionService applicationVersionService,
			ILog log,
			Version minRelativityVersion,
			Version minWebApiVersion)
		{
			this.instanceInfo = instanceInfo.ThrowIfNull(nameof(instanceInfo));
			this.log = log.ThrowIfNull(nameof(log));
			this.applicationVersionService = applicationVersionService.ThrowIfNull(nameof(applicationVersionService));
			this.minRelativityVersion = minRelativityVersion.ThrowIfNull(nameof(minRelativityVersion));
			this.minWebApiVersion = minWebApiVersion.ThrowIfNull(nameof(minWebApiVersion));
		}

		/// <inheritdoc />
		public async Task ValidateAsync(CancellationToken token)
		{
			this.log.LogInformation(
				"Retrieving the Relativity version for Relativity instance {RelativityHost}...",
				this.instanceInfo.Host);
			Version relativityVersion = await this.applicationVersionService
				                            .GetRelativityVersionAsync(token).ConfigureAwait(false);
			this.log.LogInformation(
				"Successfully retrieved Relativity version {RelativityVersion} for Relativity instance {RelativityHost}.",
				relativityVersion,
				this.instanceInfo.Host);

			// Make sure the version is logically sound.
			if (relativityVersion.Major <= 0)
			{
				this.log.LogError(
					"The Relativity version {0} is invalid and import/export service cannot be used for Relativity instance {RelativityHost}.",
					relativityVersion,
					this.instanceInfo.Host);
				string message = string.Format(
					CultureInfo.CurrentCulture,
					Relativity.Import.Export.Resources.Strings.RelativtyMinVersionInvalidExceptionMessage,
					this.instanceInfo.Host,
					relativityVersion,
					this.minRelativityVersion);
				throw new RelativityNotSupportedException(message, relativityVersion);
			}

			try
			{
				this.log.LogInformation(
					"Retrieving the import/export back-end API version for Relativity instance {RelativityHost}...",
					this.instanceInfo.Host);
				Version importExportWebApiVersion =
					await this.applicationVersionService.GetImportExportWebApiVersionAsync(token).ConfigureAwait(false);
				this.log.LogInformation(
					"Successfully retrieved the import/export back-end API version {ImportExportWebApiVersion} for Relativity instance {RelativityHost}.",
					importExportWebApiVersion,
					this.instanceInfo.Host);
				this.log.LogInformation(
					"Preparing to perform the client/server API version compatibility check for Relativity instance {RelativityHost}...",
					this.instanceInfo.Host);
				this.ValidImportExportWebApiVersion(relativityVersion, importExportWebApiVersion);
				this.log.LogInformation(
					"Successfully performed the client/server API version compatibility check for Relativity instance {RelativityHost}.",
					this.instanceInfo.Host);
			}
			catch (HttpServiceException e)
			{
				// TODO: Remove this check when preparing the Fall 2020 release.
				if (e.StatusCode == HttpStatusCode.NotFound)
				{
					this.ValidateRelativityVersion(relativityVersion);
				}
				else
				{
					// For all other HTTP errors, just throw.
					throw;
				}
			}
		}

		private void ValidImportExportWebApiVersion(Version relativityVersion, Version importExportWebApiVersion)
		{
			// Make sure the version is logically sound.
			if (importExportWebApiVersion.Major <= 0)
			{
				this.log.LogError(
					"The import/export back-end API version {0} is invalid and import/export service cannot be used.",
					importExportWebApiVersion);
				string message = string.Format(
					CultureInfo.CurrentCulture,
					Relativity.Import.Export.Resources.Strings.ImportExportWebApiVersionInvalidExceptionMessage,
					this.instanceInfo.Host);
				throw new RelativityNotSupportedException(message, relativityVersion);
			}

			if (importExportWebApiVersion.Major != this.minWebApiVersion.Major)
			{
				string message = string.Format(
					CultureInfo.CurrentCulture,
					Relativity.Import.Export.Resources.Strings.ImportExportWebApiVersionNotSupportedExceptionMessage,
					this.instanceInfo.Host);
				throw new RelativityNotSupportedException(message, relativityVersion);
			}
		}

		private void ValidateRelativityVersion(Version relativityVersion)
		{
			// For older version of Relativity that don't support the back-end API version, perform a min-version check.
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
	}
}