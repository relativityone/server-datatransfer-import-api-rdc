// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportExportCompatibilityCheck.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to perform compatibility checks between the client API and Relativity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Globalization;
	using System.Threading;
	using System.Threading.Tasks;

	using Relativity.DataExchange.Logger;
	using Relativity.DataExchange.Resources;
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
		private readonly Version requiredWebApiVersion;
		private readonly Version webApiStartFromRelativityVersion;
		private readonly IAppSettings appSettings;
		private readonly IAppSettingsInternal appSettingsInternal;
		private readonly IRunningContext runningContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImportExportCompatibilityCheck"/> class.
		/// </summary>
		/// <param name="instanceInfo">
		/// The Relativity info information.
		/// </param>
		/// <param name="applicationVersionService">
		/// The application version service object.
		/// </param>
		/// <param name="runningContext">
		/// Contains information about the context in which jobs are executed.
		/// </param>
		/// <param name="log">
		/// the log object.
		/// </param>
		public ImportExportCompatibilityCheck(
			RelativityInstanceInfo instanceInfo,
			IApplicationVersionService applicationVersionService,
			IRunningContext runningContext,
			ILog log)
			: this(
				instanceInfo,
				applicationVersionService,
				log,
				VersionConstants.MinRelativityVersion,
				VersionConstants.RequiredWebApiVersion,
				VersionConstants.WebApiStartFromRelativityVersion,
				runningContext,
				AppSettings.Instance,
				AppSettings.Instance as IAppSettingsInternal)
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
		/// <param name="runningContext">
		/// Job's context allowing us to store information about versions.
		/// </param>
		/// <param name="appSettings">
		/// The application settings.
		/// </param>
		/// <param name="appSettingsInternal">
		/// The application internal settings.
		/// </param>
		public ImportExportCompatibilityCheck(
			RelativityInstanceInfo instanceInfo,
			IApplicationVersionService applicationVersionService,
			ILog log,
			Version minRelativityVersion,
			Version requiredWebApiVersion,
			Version webApiStartFromRelativityVersion,
			IRunningContext runningContext,
			IAppSettings appSettings,
			IAppSettingsInternal appSettingsInternal)
		{
			this.webApiStartFromRelativityVersion = webApiStartFromRelativityVersion;
			this.instanceInfo = instanceInfo.ThrowIfNull(nameof(instanceInfo));
			this.log = log.ThrowIfNull(nameof(log));
			this.applicationVersionService = applicationVersionService.ThrowIfNull(nameof(applicationVersionService));
			this.minRelativityVersion = minRelativityVersion.ThrowIfNull(nameof(minRelativityVersion));
			this.requiredWebApiVersion = requiredWebApiVersion.ThrowIfNull(nameof(requiredWebApiVersion));
			this.runningContext = runningContext;
			this.appSettings = appSettings.ThrowIfNull(nameof(appSettings));
			this.appSettingsInternal = appSettingsInternal.ThrowIfNull(nameof(appSettingsInternal));
		}

		/// <inheritdoc />
		public async Task ValidateAsync(CancellationToken token)
		{
			this.log.LogInformation(
				"Retrieving the Relativity version for Relativity instance {RelativityHost}...",
				this.instanceInfo.Host.Secure());
			Version relativityVersion = await this.applicationVersionService
											.GetRelativityVersionAsync(token).ConfigureAwait(false);
			this.runningContext.RelativityVersion = relativityVersion;
			this.log.LogInformation(
				"Successfully retrieved Relativity version {RelativityVersion} for Relativity instance {RelativityHost}.",
				relativityVersion,
				this.instanceInfo.Host.Secure());
			this.VerifyRelativityVersionFormat(relativityVersion);
			await this.PerformValidationAsync(token, relativityVersion).ConfigureAwait(false);
		}

		private async Task PerformValidationAsync(
			CancellationToken token,
			Version relativityVersion)
		{
			if (relativityVersion < this.webApiStartFromRelativityVersion)
			{
				// Perform validation for older Relativity Version - this code should be removed on compatibility break change
				this.ValidateRelativityVersion(relativityVersion);
			}
			else
			{
				await this.ValidateWebApiVersion(token, relativityVersion).ConfigureAwait(false);
			}
		}

		private async Task ValidateWebApiVersion(
			CancellationToken token,
			Version relativityVersion)
		{
			this.log.LogInformation(
				"Retrieving the import/export WebAPI version for Relativity instance {RelativityHost}...",
				this.instanceInfo.Host.Secure());
			Version importExportWebApiVersion = await this.applicationVersionService
													.GetImportExportWebApiVersionAsync(token).ConfigureAwait(false);
			this.runningContext.ImportExportWebApiVersion = importExportWebApiVersion;
			this.log.LogWarning(
				"Successfully retrieved the import/export WebAPI version {ImportExportWebApiVersion} for Relativity instance {RelativityHost}.",
				importExportWebApiVersion,
				this.instanceInfo.Host.Secure());
			this.log.LogInformation(
				"Preparing to perform the client/server API version compatibility check for Relativity instance {RelativityHost}...",
				this.instanceInfo.Host.Secure());
			this.VerifyImportExportWebApiVersion(relativityVersion, importExportWebApiVersion);
			this.log.LogInformation(
				"Successfully performed the client/server API version compatibility check for Relativity instance {RelativityHost}.",
				this.instanceInfo.Host.Secure());
		}

		private void VerifyRelativityVersionFormat(Version relativityVersion)
		{
			// Make sure the version is logically sound.
			if (relativityVersion.Major <= 0)
			{
				this.log.LogError(
					"The Relativity version {RelativityVersion} is invalid and import/export service cannot be used for Relativity instance {RelativityHost}.",
					relativityVersion,
					this.instanceInfo.Host.Secure());
				string message = Strings
					.RelativtyMinVersionInvalidNoAppNameExceptionMessage;
				if (!string.IsNullOrEmpty(this.appSettings.ApplicationName))
				{
					message = string.Format(
						CultureInfo.CurrentCulture,
						Strings.RelativtyMinVersionInvalidExceptionMessage,
						this.appSettings.ApplicationName);
				}

				if (this.ShouldThrowException(message))
				{
					throw new RelativityNotSupportedException(message, relativityVersion);
				}
			}
		}

		/// <summary>
		/// Inspect the application settings to determine whether to throw the exception. If disabled, the message is logged as a warning.
		/// </summary>
		/// <param name="message">
		/// The message to log.
		/// </param>
		/// <returns>
		/// <see langword="true" /> to throw the exception; otherwise, <see langword="false" />.
		/// </returns>
		private bool ShouldThrowException(string message)
		{
			bool enforce = this.appSettingsInternal.EnforceVersionCompatibilityCheck;
			if (!enforce)
			{
				this.log.LogWarning(
					"The version compatibility check failed but the application settings are configured to disable enforcement. Message: {VersionCompatibilityMessage}",
					message);
			}

			return enforce;
		}

		private void VerifyImportExportWebApiVersion(Version relativityVersion, Version importExportWebApiVersion)
		{
			// Make sure the version is logically sound.
			if (importExportWebApiVersion.Major <= 0)
			{
				this.log.LogError(
					"The import/export WebAPI version {ImportExportWebApiVersion} is invalid and import/export service cannot be used.",
					importExportWebApiVersion);
				string message = Strings
					.ImportExportWebApiVersionInvalidNoAppNameExceptionMessage;
				if (!string.IsNullOrEmpty(this.appSettings.ApplicationName))
				{
					message = string.Format(
						CultureInfo.CurrentCulture,
						Strings.ImportExportWebApiVersionInvalidExceptionMessage,
						this.appSettings.ApplicationName);
				}

				if (this.ShouldThrowException(message))
				{
					throw new RelativityNotSupportedException(message, relativityVersion);
				}
			}

			if (this.requiredWebApiVersion.Major != importExportWebApiVersion.Major)
			{
				this.log.LogError(
					"The import/export WebAPI version {ImportExportWebApiVersion} isn't compatible with the required client API version {RequiredWebApiVersion}.",
					importExportWebApiVersion,
					this.requiredWebApiVersion);
				string message = Strings
					.ImportExportWebApiVersionNotSupportedNoAppNameExceptionMessage;
				if (!string.IsNullOrEmpty(this.appSettings.ApplicationName))
				{
					message = string.Format(
						CultureInfo.CurrentCulture,
						Strings.ImportExportWebApiVersionNotSupportedExceptionMessage,
						this.appSettings.ApplicationName);
				}

				if (this.ShouldThrowException(message))
				{
					throw new RelativityNotSupportedException(message, relativityVersion);
				}
			}
		}

		private void ValidateRelativityVersion(Version relativityVersion)
		{
			// For older version of Relativity that don't support the WebAPI version, perform a min-version check.
			if (relativityVersion < this.minRelativityVersion)
			{
				string message = string.Format(
					CultureInfo.CurrentCulture,
					Strings.RelativtyMinVersionNoAppNameExceptionMessage,
					relativityVersion,
					this.minRelativityVersion);
				if (!string.IsNullOrEmpty(this.appSettings.ApplicationName))
				{
					message = string.Format(
						CultureInfo.CurrentCulture,
						Strings.RelativtyMinVersionExceptionMessage,
						this.appSettings.ApplicationName,
						relativityVersion,
						this.minRelativityVersion);
				}

				if (this.ShouldThrowException(message))
				{
					throw new RelativityNotSupportedException(message, relativityVersion);
				}
			}
		}
	}
}