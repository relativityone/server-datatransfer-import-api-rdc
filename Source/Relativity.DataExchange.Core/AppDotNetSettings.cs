// ----------------------------------------------------------------------------
// <copyright file="AppDotNetSettings.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	using Relativity.DataExchange.Io;

	/// <summary>
	/// Represents a class object that provide a thread-safe copy of all .NET application settings. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// Intentionally using string literals for all key names to ensure all published settings
	/// remain consistent but allow property names to be refactored with zero impact.
	/// </remarks>
	[Serializable]
	internal sealed class AppDotNetSettings : IAppSettings, IAppSettingsInternal
	{
		// All backing fields go here.
		private int exportErrorWaitTimeInSeconds;
		private int exportErrorNumberOfRetries;
		private int ioErrorNumberOfRetries;
		private int ioErrorWaitTimeInSeconds;
		private int httpTimeoutSeconds;
		private int maximumReloginTries;
		private int maxNumberOfFileExportTasks;
		private int tapiMinDataRateMbps;
		private int tapiMaxInactivitySeconds;
		private int tapiMaxJobParallelism;
		private int tapiTargetDataRateMbps;
		private int webBasedFileDownloadChunkSize;
		private string webApiServiceMappedUrl;
		private string programmaticWebApiServiceUrl;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppDotNetSettings"/> class.
		/// </summary>
		public AppDotNetSettings()
		{
			this.EnforceMinWaitTime = true;
			this.EnforceMinRetryCount = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppDotNetSettings"/> class.
		/// </summary>
		/// <param name="settings">
		/// The settings.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="settings"/> is <see langword="null" />.
		/// </exception>
		public AppDotNetSettings(IAppSettings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			foreach (PropertyInfo pi in typeof(AppDotNetSettings).GetProperties(
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
			{
				if (pi.SetMethod != null)
				{
					object value = pi.GetValue(settings, null);
					pi.SetValue(this, value, null);
				}
			}

			IAppSettings thisSettings = this;
			if (settings.ObjectFieldIdListContainsArtifactId != null)
			{
				thisSettings.ObjectFieldIdListContainsArtifactId = new List<int>(settings.ObjectFieldIdListContainsArtifactId);
			}
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.ApplicationNameKey,
			AppSettingsConstants.ApplicationNameDefaultValue)]
		string IAppSettings.ApplicationName
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.AuditLevelKey,
			AppSettingsConstants.AuditLevelDefaultValue)]
		string IAppSettings.AuditLevel
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.CreateErrorForEmptyNativeFileKey,
			AppSettingsConstants.CreateErrorForEmptyNativeFileDefaultValue)]
		bool IAppSettings.CreateErrorForEmptyNativeFile
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.CreateErrorForInvalidDateKey,
			AppSettingsConstants.CreateErrorForInvalidDateDefaultValue)]
		bool IAppSettings.CreateErrorForInvalidDate
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.CreateFoldersInWebApiKey,
			AppSettingsConstants.CreateFoldersInWebApiDefaultValue)]
		bool IAppSettings.CreateFoldersInWebApi
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.DefaultMaxErrorCountKey,
			AppSettingsConstants.DefaultMaxErrorCountDefaultValue)]
		int IAppSettings.DefaultMaxErrorCount
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.DisableImageLocationValidationKey,
			AppSettingsConstants.DisableImageLocationValidationDefaultValue)]
		bool IAppSettings.DisableImageLocationValidation
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.DisableImageTypeValidationKey,
			AppSettingsConstants.DisableImageTypeValidationDefaultValue)]
		bool IAppSettings.DisableImageTypeValidation
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.DisableOutsideInFileIdentificationKey,
			AppSettingsConstants.DisableOutsideInFileIdentificationDefaultValue)]
		bool IAppSettings.DisableOutsideInFileIdentification
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.DisableTextFileEncodingCheckKey,
			AppSettingsConstants.DisableTextFileEncodingCheckDefaultValue)]
		bool IAppSettings.DisableTextFileEncodingCheck
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.DisableThrowOnIllegalCharactersKey,
			AppSettingsConstants.DisableThrowOnIllegalCharactersDefaultValue)]
		bool IAppSettings.DisableThrowOnIllegalCharacters
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.DynamicBatchResizingOnKey,
			AppSettingsConstants.DynamicBatchResizingOnDefaultValue)]
		bool IAppSettings.DynamicBatchResizingOn
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.EnableCaseSensitiveSearchOnImportKey,
			AppSettingsConstants.EnableCaseSensitiveSearchOnImportDefaultValue)]
		bool IAppSettings.EnableCaseSensitiveSearchOnImport
		{
			get;
			set;
		}

		/// <inheritdoc />
		/// <remarks>
		/// This is an unmapped setting and generally reserved for testing purposes.
		/// </remarks>
		[AppSetting]
		public bool EnforceMinRetryCount
		{
			get;
			set;
		}

		/// <inheritdoc />
		/// <remarks>
		/// This is an unmapped setting and generally reserved for testing purposes.
		/// </remarks>
		[AppSetting]
		public bool EnforceMinWaitTime
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionDataExchange,
			AppSettingsConstants.EnforceVersionCompatibilityCheckKey,
			AppSettingsConstants.EnforceVersionCompatibilityCheckDefaultValue)]
		bool IAppSettingsInternal.EnforceVersionCompatibilityCheck
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.ExportBatchSizeKey,
			AppSettingsConstants.ExportBatchSizeDefaultValue)]
		int IAppSettings.ExportBatchSize
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyUtility,
			AppSettingsConstants.ExportErrorNumberOfRetriesKey,
			AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue)]
		int IAppSettings.ExportErrorNumberOfRetries
		{
			get
			{
				if (this.EnforceMinRetryCount && this.exportErrorNumberOfRetries < 1)
				{
					this.exportErrorNumberOfRetries = AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue;
				}

				return this.exportErrorNumberOfRetries;
			}

			set => this.exportErrorNumberOfRetries = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyUtility,
			AppSettingsConstants.ExportErrorWaitTimeInSecondsKey,
			AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue)]
		int IAppSettings.ExportErrorWaitTimeInSeconds
		{
			get
			{
				if (this.EnforceMinWaitTime && this.exportErrorWaitTimeInSeconds < 1)
				{
					this.exportErrorWaitTimeInSeconds = AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue;
				}

				return this.exportErrorWaitTimeInSeconds;
			}

			set => this.exportErrorWaitTimeInSeconds = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.ExportThreadCountKey,
			AppSettingsConstants.ExportThreadCountDefaultValue)]
		int IAppSettings.ExportThreadCount
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionDataExchange,
			AppSettingsConstants.FileTypeIdentifyTimeoutSecondsKey,
			AppSettingsConstants.FileTypeIdentifyTimeoutSecondsDefaultValue)]
		int IAppSettings.FileTypeIdentifyTimeoutSeconds
		{
			get;
			set;
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Globalization",
			"CA1308:NormalizeStringsToUppercase",
			Justification = "This is required for backwards compatibility.")]
		[AppSetting]
		bool IAppSettings.ForceFolderPreview
		{
			get
			{
				string value = AppSettingsManager.GetRegistryKeyValue(AppSettingsConstants.ForceFolderPreviewRegistryKey);
				if (string.IsNullOrEmpty(value))
				{
					AppSettingsManager.SetRegistryKeyValue(
						AppSettingsConstants.ForceFolderPreviewRegistryKey,
						bool.TrueString.ToLowerInvariant());
					return AppSettingsConstants.ForceFolderPreviewDefaultValue;
				}

				return string.Compare(value, bool.TrueString, StringComparison.OrdinalIgnoreCase) == 0;
			}

			set =>
				AppSettingsManager.SetRegistryKeyValue(
					AppSettingsConstants.ForceFolderPreviewRegistryKey,
					value.ToString().ToLowerInvariant());
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.ForceParallelismInNewExportKey,
			AppSettingsConstants.ForceParallelismInNewExportDefaultValue)]
		bool IAppSettings.ForceParallelismInNewExport
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.ForceWebUploadKey,
			AppSettingsConstants.ForceWebUploadDefaultValue)]
		bool IAppSettings.ForceWebUpload
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.HttpTimeoutSecondsKey,
			AppSettingsConstants.HttpTimeoutSecondsDefaultValue)]
		int IAppSettings.HttpTimeoutSeconds
		{
			get
			{
				if (this.httpTimeoutSeconds < 1)
				{
					this.httpTimeoutSeconds = AppSettingsConstants.HttpTimeoutSecondsDefaultValue;
				}

				return this.httpTimeoutSeconds;
			}

			set => this.httpTimeoutSeconds = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.ImportBatchMaxVolumeKey,
			AppSettingsConstants.ImportBatchMaxVolumeDefaultValue)]
		int IAppSettings.ImportBatchMaxVolume
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.ImportBatchSizeKey,
			AppSettingsConstants.ImportBatchSizeDefaultValue)]
		int IAppSettings.ImportBatchSize
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyUtility,
			AppSettingsConstants.IoErrorNumberOfRetriesKey,
			AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue)]
		int IAppSettings.IoErrorNumberOfRetries
		{
			get
			{
				if (this.EnforceMinRetryCount && this.ioErrorNumberOfRetries < 1)
				{
					this.ioErrorNumberOfRetries = AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue;
				}

				return this.ioErrorNumberOfRetries;
			}

			set => this.ioErrorNumberOfRetries = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyUtility,
			AppSettingsConstants.IoErrorWaitTimeInSecondsKey,
			AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue)]
		int IAppSettings.IoErrorWaitTimeInSeconds
		{
			get
			{
				if (this.EnforceMinWaitTime && this.ioErrorWaitTimeInSeconds < 1)
				{
					this.ioErrorWaitTimeInSeconds = AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue;
				}

				return this.ioErrorWaitTimeInSeconds;
			}

			set => this.ioErrorWaitTimeInSeconds = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.JobCompleteBatchSizeKey,
			AppSettingsConstants.JobCompleteBatchSizeDefaultValue)]
		int IAppSettings.JobCompleteBatchSize
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.LoadImportedFullTextFromServerKey,
			AppSettingsConstants.LoadImportedFullTextFromServerDefaultValue)]
		bool IAppSettings.LoadImportedFullTextFromServer
		{
			get;
			set;
		}

		/// <inheritdoc />
		/// <remarks>
		/// This was technically supported in <see cref="AppSettingsConstants.SectionLegacyUtility"/> but never documented.
		/// </remarks>
		[AppSetting(
			AppSettingsConstants.SectionLegacyWindowsProcess,
			AppSettingsConstants.LogAllEventsKey,
			AppSettingsConstants.LogAllEventsDefaultValue)]
		bool IAppSettings.LogAllEvents
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.LogConfigXmlFileNameKey,
			AppSettingsConstants.LogConfigXmlFileNameDefaultValue)]
		string IAppSettings.LogConfigXmlFileName
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting]
		int IAppSettings.MaxFilesForTapiBridge
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.MaximumReloginTriesKey,
			AppSettingsConstants.MaximumReloginTriesDefaultValue)]
		int IAppSettings.MaxReloginTries
		{
			get
			{
				if (this.maximumReloginTries < 1)
				{
					this.maximumReloginTries = AppSettingsConstants.MaximumReloginTriesDefaultValue;
				}

				return this.maximumReloginTries;
			}

			set => this.maximumReloginTries = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyUtility,
			AppSettingsConstants.MaxNumberOfFileExportTasksKey,
			AppSettingsConstants.MaxNumberOfFileExportTasksDefaultValue)]
		int IAppSettings.MaxNumberOfFileExportTasks
		{
			get
			{
				if (this.maxNumberOfFileExportTasks < 1)
				{
					this.maxNumberOfFileExportTasks = AppSettingsConstants.MaxNumberOfFileExportTasksDefaultValue;
				}

				return this.maxNumberOfFileExportTasks;
			}

			set => this.maxNumberOfFileExportTasks = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.MinBatchSizeKey,
			AppSettingsConstants.MinBatchSizeDefaultValue)]
		int IAppSettings.MinBatchSize
		{
			get;
			set;
		}

		/// <inheritdoc />
		/// <remarks>
		/// This setting is intentionally omitted from the RDC app.config file and provided for potential backwards or forwards
		/// compatibility due to a recent redirection URL change by the platform team. See REL-294177 for more details.
		/// </remarks>
		[AppSetting(
			AppSettingsConstants.SectionDataExchange,
			AppSettingsConstants.OAuth2ImplicitCredentialRedirectUrlKey,
			AppSettingsConstants.OAuth2ImplicitCredentialRedirectUrlDefaultValue)]
		string IAppSettings.OAuth2ImplicitCredentialRedirectUrl
		{
			get;
			set;
		}

		/// <inheritdoc />
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA2227:CollectionPropertiesShouldBeReadOnly",
			Justification = "This is required for backwards compatibility.")]
		[AppSetting]
		IList<int> IAppSettings.ObjectFieldIdListContainsArtifactId
		{
			get
			{
				string value = AppSettingsManager.GetRegistryKeyValue(AppSettingsConstants.ObjectFieldIdListContainsArtifactIdRegistryKey);
				if (string.IsNullOrEmpty(value))
				{
					AppSettingsManager.SetRegistryKeyValue(
						AppSettingsConstants.ObjectFieldIdListContainsArtifactIdRegistryKey,
						string.Empty);
					return new List<int>();
				}

				return value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
			}

			set
			{
				if (value != null)
				{
					AppSettingsManager.SetRegistryKeyValue(
						AppSettingsConstants.ObjectFieldIdListContainsArtifactIdRegistryKey,
						string.Join(",", value.Select(x => x.ToString())));
				}
			}
		}

		/// <inheritdoc />
		[AppSetting]
		string IAppSettings.OpenIdConnectHomeRealmDiscoveryHint
		{
			get
			{
				string value = AppSettingsManager.GetRegistryKeyValue(AppSettingsConstants.OpenIdConnectHomeRealmDiscoveryHintKey);
				return value;
			}
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.PermissionErrorsRetryKey,
			AppSettingsConstants.PermissionErrorsRetryDefaultValue)]
		bool IAppSettings.PermissionErrorsRetry
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.PreviewThresholdKey,
			AppSettingsConstants.PreviewThresholdDefaultValue)]
		int IAppSettings.PreviewThreshold
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.ProcessFormRefreshRateKey,
			AppSettingsConstants.ProcessFormRefreshRateDefaultValue)]
		int IAppSettings.ProcessFormRefreshRate
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting]
		string IAppSettings.ProgrammaticWebApiServiceUrl
		{
			get => this.programmaticWebApiServiceUrl;
			set => this.programmaticWebApiServiceUrl = ValidateUriFormat(value);
		}

		/// <inheritdoc />
		[AppSetting]
		RetryOptions IAppSettings.RetryOptions
		{
			get
			{
				// Always use other settings to drive these enum switches.
				RetryOptions value = AppSettingsConstants.RetryOptionsDefaultValue;
				if (((IAppSettings)this).PermissionErrorsRetry)
				{
					value |= RetryOptions.Permissions;
				}
				else
				{
					value &= ~RetryOptions.Permissions;
				}

				return value;
			}
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.SuppressServerCertificateValidationKey,
			AppSettingsConstants.SuppressServerCertificateValidationDefaultValue)]
		bool IAppSettings.SuppressServerCertificateValidation
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiAsperaBcpRootFolderKey,
			AppSettingsConstants.TapiAsperaBcpRootFolderDefaultValue)]
		string IAppSettings.TapiAsperaBcpRootFolder
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiAsperaNativeDocRootLevelsKey,
			AppSettingsConstants.TapiAsperaNativeDocRootLevelsDefaultValue)]
		int IAppSettings.TapiAsperaNativeDocRootLevels
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiBadPathErrorsRetryKey,
			AppSettingsConstants.TapiBadPathErrorsRetryDefaultValue)]
		bool IAppSettings.TapiBadPathErrorsRetry
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting]
		int IAppSettings.TapiBridgeExportTransferWaitingTimeInSeconds
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionDataExchange,
			AppSettingsConstants.TapiFileNotFoundErrorsDisabledKey,
			AppSettingsConstants.TapiFileNotFoundErrorsDisabledDefaultValue)]
		bool IAppSettings.TapiFileNotFoundErrorsDisabled
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionDataExchange,
			AppSettingsConstants.TapiFileNotFoundErrorsRetryKey,
			AppSettingsConstants.TapiFileNotFoundErrorsRetryDefaultValue)]
		bool IAppSettings.TapiFileNotFoundErrorsRetry
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiForceAsperaClientKey,
			AppSettingsConstants.TapiForceAsperaClientDefaultValue)]
		bool IAppSettings.TapiForceAsperaClient
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiForceBcpHttpClientKey,
			AppSettingsConstants.TapiForceBcpHttpClientDefaultValue)]
		bool IAppSettings.TapiForceBcpHttpClient
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiForceClientCandidatesKey,
			AppSettingsConstants.TapiForceClientCandidatesDefaultValue)]
		string IAppSettings.TapiForceClientCandidates
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiForceFileShareClientKey,
			AppSettingsConstants.TapiForceFileShareClientDefaultValue)]
		bool IAppSettings.TapiForceFileShareClient
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiForceHttpClientKey,
			AppSettingsConstants.TapiForceHttpClientDefaultValue)]
		bool IAppSettings.TapiForceHttpClient
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiLargeFileProgressEnabledKey,
			AppSettingsConstants.TapiLargeFileProgressEnabledDefaultValue)]
		bool IAppSettings.TapiLargeFileProgressEnabled
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionDataExchange,
			AppSettingsConstants.TapiMaxInactivitySecondsKey,
			AppSettingsConstants.TapiMaxInactivitySecondsDefaultValue)]
		int IAppSettings.TapiMaxInactivitySeconds
		{
			get
			{
				if (this.tapiMaxInactivitySeconds < 1)
				{
					this.tapiMaxInactivitySeconds = AppSettingsConstants.TapiMaxInactivitySecondsDefaultValue;
				}

				return this.tapiMaxInactivitySeconds;
			}

			set => this.tapiMaxInactivitySeconds = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiMaxJobParallelismKey,
			AppSettingsConstants.TapiMaxJobParallelismDefaultValue)]
		int IAppSettings.TapiMaxJobParallelism
		{
			get
			{
				if (this.tapiMaxJobParallelism < 1)
				{
					this.tapiMaxJobParallelism = AppSettingsConstants.TapiMaxJobParallelismDefaultValue;
				}

				return this.tapiMaxJobParallelism;
			}

			set => this.tapiMaxJobParallelism = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiMinDataRateMbpsKey,
			AppSettingsConstants.TapiMinDataRateMbpsDefaultValue)]
		int IAppSettings.TapiMinDataRateMbps
		{
			get
			{
				if (this.tapiMinDataRateMbps < 1)
				{
					this.tapiMinDataRateMbps = AppSettingsConstants.TapiMinDataRateMbpsDefaultValue;
				}

				return this.tapiMinDataRateMbps;
			}

			set => this.tapiMinDataRateMbps = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiPreserveFileTimestampsKey,
			AppSettingsConstants.TapiPreserveFileTimestampsDefaultValue)]
		bool IAppSettings.TapiPreserveFileTimestamps
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiSubmitApmMetricsKey,
			AppSettingsConstants.TapiSubmitApmMetricsDefaultValue)]
		bool IAppSettings.TapiSubmitApmMetrics
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiTargetDataRateMbpsKey,
			AppSettingsConstants.TapiTargetDataRateMbpsDefaultValue)]
		int IAppSettings.TapiTargetDataRateMbps
		{
			get
			{
				if (this.tapiTargetDataRateMbps < 1)
				{
					this.tapiTargetDataRateMbps = AppSettingsConstants.TapiTargetDataRateMbpsDefaultValue;
				}

				return this.tapiTargetDataRateMbps;
			}

			set => this.tapiTargetDataRateMbps = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TapiTransferLogDirectoryKey,
			AppSettingsConstants.TapiTransferLogDirectoryDefaultValue)]
		string IAppSettings.TapiTransferLogDirectory
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.TempDirectoryKey,
			AppSettingsConstants.TempDirectoryDefaultValue)]
		string IAppSettings.TempDirectory
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.UseOldExportKey,
			AppSettingsConstants.UseOldExportDefaultValue)]
		bool IAppSettings.UseOldExport
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.UsePipeliningForNativeAndObjectImportsKey,
			AppSettingsConstants.UsePipeliningForNativeAndObjectImportsDefaultValue)]
		bool IAppSettings.UsePipeliningForNativeAndObjectImports
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacykCuraConfig,
			AppSettingsConstants.ValueRefreshThresholdKey,
			AppSettingsConstants.ValueRefreshThresholdDefaultValue)]
		int IAppSettings.ValueRefreshThreshold
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.WaitBeforeReconnectKey,
			AppSettingsConstants.WaitBeforeReconnectDefaultValue)]
		int IAppSettings.WaitBeforeReconnect
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.WebApiOperationTimeoutKey,
			AppSettingsConstants.WebApiOperationTimeoutDefaultValue)]
		int IAppSettings.WebApiOperationTimeout
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Web API service URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		/// <remarks>
		/// This property is used to store a configuration driven URL.
		/// </remarks>
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.WebApiServiceUrlKey,
			AppSettingsConstants.WebApiServiceUrlDefaultValue)]
		public string WebApiServiceMappedUrl
		{
			get => this.webApiServiceMappedUrl;
			set => this.webApiServiceMappedUrl = ValidateUriFormat(value);
		}

		/// <inheritdoc />
		[AppSetting]
		string IAppSettings.WebApiServiceUrl
		{
			get
			{
				// The Web API URL is driven in this order:
				// 1. The code assigns ProgrammaticWebApiServiceUrl.
				// 2. The user configures by application setting.
				// 3. The Registry.
				IAppSettings dotNetSettings = this;
				string returnValue = dotNetSettings.ProgrammaticWebApiServiceUrl;
				if (string.IsNullOrWhiteSpace(returnValue) && !string.IsNullOrWhiteSpace(this.WebApiServiceMappedUrl))
				{
					returnValue = this.WebApiServiceMappedUrl;
				}

				if (string.IsNullOrWhiteSpace(returnValue))
				{
					returnValue = AppSettingsManager.GetRegistryKeyValue(AppSettingsConstants.WebApiServiceUrlRegistryKey);
				}

				return ValidateUriFormat(returnValue);
			}
			set => AppSettingsManager.SetRegistryKeyValue(AppSettingsConstants.WebApiServiceUrlRegistryKey, ValidateUriFormat(value));
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			AppSettingsConstants.WebBasedFileDownloadChunkSizeKey,
			AppSettingsConstants.WebBasedFileDownloadChunkSizeDefaultValue)]
		int IAppSettings.WebBasedFileDownloadChunkSize
		{
			get => System.Math.Max(this.webBasedFileDownloadChunkSize, AppSettingsConstants.WebBasedFileDownloadChunkSizeMinValue);
			set => this.webBasedFileDownloadChunkSize = value;
		}

		/// <summary>
		/// Validates that the URI is valid and returns a properly formatted URI string.
		/// </summary>
		/// <param name="value">
		/// The URI value.
		/// </param>
		/// <returns>
		/// The properly formatted URI string.
		/// </returns>
		public static string ValidateUriFormat(string value)
		{
			if (!string.IsNullOrEmpty(value) && !value.Trim().EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				value = value.Trim() + "/";
			}

			// Replaced the original implementation that used a try/catch and full object construction with one that's more debugger friendly and isn't quite so ominous.
			Uri uri = null;
			return Uri.TryCreate(value, UriKind.Absolute, out uri) ? value : string.Empty;
		}

		/// <inheritdoc />
		IAppSettings IAppSettings.DeepCopy()
		{
			return new AppDotNetSettings(this);
		}

		/// <inheritdoc />
		string IAppSettings.ValidateUriFormat(string value)
		{
			return ValidateUriFormat(value);
		}
	}
}