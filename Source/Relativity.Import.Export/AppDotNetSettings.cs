// ----------------------------------------------------------------------------
// <copyright file="AppDotNetSettings.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// Represents a class object that provide a thread-safe copy of all .NET application settings.
	/// </summary>
	/// <remarks>
	/// Intentionally using string literals for all key names to ensure all published settings
	/// remain consistent but allow property names to be refactored with zero impact.
	/// </remarks>
	[Serializable]
	internal sealed class AppDotNetSettings : IAppSettings
	{
		// All backing fields go here.
		private int exportErrorWaitTimeInSeconds;
		private int exportErrorNumberOfRetries;
		private int ioErrorNumberOfRetries;
		private int ioErrorWaitTimeInSeconds;
		private int httpTimeoutSeconds;
		private int maximumFilesForTapiBridge;
		private int maximumReloginTries;
		private int maxNumberOfFileExportTasks;
		private int tapiBridgeExportTransferWaitingTimeInSeconds;
		private int tapiMinDataRateMbps;
		private int tapiMaxJobParallelism;
		private int tapiTargetDataRateMbps;
		private int webBasedFileDownloadChunkSize;
		private string webApiServiceUrl;

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

			if (settings.ProgrammaticWebApiServiceUrl != null)
			{
				thisSettings.ProgrammaticWebApiServiceUrl = settings.ProgrammaticWebApiServiceUrl;
			}

			if (settings.WebApiServiceUrl != null)
			{
				this.webApiServiceUrl = settings.WebApiServiceUrl.ToString();
			}

			this.EnforceMinRetryCount = settings.EnforceMinRetryCount;
			this.EnforceMinWaitTime = settings.EnforceMinWaitTime;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"ApplicationName",
			"")]
		string IAppSettings.ApplicationName
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"AuditLevel",
			AppSettingsConstants.AuditLevelDefaultValue)]
		string IAppSettings.AuditLevel
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyUtility,
			"CreateErrorForInvalidDate",
			AppSettingsConstants.CreateErrorForEmptyNativeFileDefaultValue)]
		bool IAppSettings.CreateErrorForEmptyNativeFile
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"CreateErrorForInvalidDate",
			AppSettingsConstants.CreateErrorForInvalidDateDefaultValue)]
		bool IAppSettings.CreateErrorForInvalidDate
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"CreateFoldersInWebAPI",
			AppSettingsConstants.CreateFoldersInWebApiDefaultValue)]
		bool IAppSettings.CreateFoldersInWebApi
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"DefaultMaxErrorCount",
			AppSettingsConstants.DefaultMaxErrorCountDefaultValue)]
		int IAppSettings.DefaultMaxErrorCount
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"DisableImageLocationValidation",
			AppSettingsConstants.DisableImageLocationValidationDefaultValue)]
		bool IAppSettings.DisableImageLocationValidation
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"DisableImageTypeValidation",
			AppSettingsConstants.DisableImageTypeValidationDefaultValue)]
		bool IAppSettings.DisableImageTypeValidation
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"DisableNativeValidation",
			AppSettingsConstants.DisableOutsideInFileIdentificationDefaultValue)]
		bool IAppSettings.DisableOutsideInFileIdentification
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"DisableTextFileEncodingCheck",
			AppSettingsConstants.DisableTextFileEncodingCheckDefaultValue)]
		bool IAppSettings.DisableTextFileEncodingCheck
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"DisableNativeLocationValidation",
			AppSettingsConstants.DisableThrowOnIllegalCharactersDefaultValue)]
		bool IAppSettings.DisableThrowOnIllegalCharacters
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"DynamicBatchResizingOn",
			AppSettingsConstants.DynamicBatchResizingOnDefaultValue)]
		bool IAppSettings.DynamicBatchResizingOn
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"EnableCaseSensitiveSearchOnImport",
			AppSettingsConstants.EnableCaseSensitiveSearchOnImportDefaultValue)]
		bool IAppSettings.EnableCaseSensitiveSearchOnImport
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"EnableSingleModeImport",
			AppSettingsConstants.EnableSingleModeImportDefaultValue)]
		bool IAppSettings.EnableSingleModeImport
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting]
		public bool EnforceMinRetryCount
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting]
		public bool EnforceMinWaitTime
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"ExportBatchSize",
			AppSettingsConstants.ExportBatchSizeDefaultValue)]
		int IAppSettings.ExportBatchSize
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyUtility,
			"ExportErrorNumberOfRetries",
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
			"ExportErrorWaitTimeInSeconds",
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
			"ExportThreadCount",
			AppSettingsConstants.ExportThreadCountDefaultValue)]
		int IAppSettings.ExportThreadCount
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
				string value = AppSettingsReader.GetRegistryKeyValue(AppSettingsConstants.ForceFolderPreviewRegistryKey);
				if (string.IsNullOrEmpty(value))
				{
					AppSettingsReader.SetRegistryKeyValue(
						AppSettingsConstants.ForceFolderPreviewRegistryKey,
						bool.TrueString.ToLowerInvariant());
					return AppSettingsConstants.ForceFolderPreviewDefaultValue;
				}

				return string.Compare(value, bool.TrueString, StringComparison.OrdinalIgnoreCase) == 0;
			}

			set =>
				AppSettingsReader.SetRegistryKeyValue(
					AppSettingsConstants.ForceFolderPreviewRegistryKey,
					value.ToString().ToLowerInvariant());
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"ForceParallelismInNewExport",
			AppSettingsConstants.ForceParallelismInNewExportDefaultValue)]
		bool IAppSettings.ForceParallelismInNewExport
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"ForceWebUpload",
			AppSettingsConstants.ForceWebUploadDefaultValue)]
		bool IAppSettings.ForceWebUpload
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting]
		string IAppSettings.OpenIdConnectHomeRealmDiscoveryHint
		{
			get
			{
				string value = AppSettingsReader.GetRegistryKeyValue(AppSettingsConstants.OpenIdConnectHomeRealmDiscoveryHintKey);
				return value;
			}
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"HttpTimeoutSeconds",
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
			"ImportBatchMaxVolume",
			AppSettingsConstants.ImportBatchMaxVolumeDefaultValue)]
		int IAppSettings.ImportBatchMaxVolume
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"ImportBatchSize",
			AppSettingsConstants.ImportBatchSizeDefaultValue)]
		int IAppSettings.ImportBatchSize
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyUtility,
			"IOErrorNumberOfRetries",
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
			"IOErrorWaitTimeInSeconds",
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
			"JobCompleteBatchSize",
			AppSettingsConstants.JobCompleteBatchSizeDefaultValue)]
		int IAppSettings.JobCompleteBatchSize
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"LoadImportedFullTextFromServer",
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
			"LogAllEvents",
			AppSettingsConstants.LogAllEventsDefaultValue)]
		bool IAppSettings.LogAllEvents
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"LogConfigFile",
			AppSettingsConstants.LogConfigXmlFileNameDefaultValue)]
		string IAppSettings.LogConfigXmlFileName
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyUtility,
			"MaximumFilesForTapiBridge",
			AppSettingsConstants.MaxFilesForTapiBridgeDefaultValue)]
		int IAppSettings.MaxFilesForTapiBridge
		{
			get
			{
				if (this.maximumFilesForTapiBridge < 1)
				{
					this.maximumFilesForTapiBridge = AppSettingsConstants.MaxFilesForTapiBridgeDefaultValue;
				}

				return this.maximumFilesForTapiBridge;
			}

			set => this.maximumFilesForTapiBridge = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"MaximumReloginTries",
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
			"MaxNumberOfFileExportTasks",
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
			"MinimumBatchSize",
			AppSettingsConstants.MinBatchSizeDefaultValue)]
		int IAppSettings.MinBatchSize
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
				string value = AppSettingsReader.GetRegistryKeyValue(AppSettingsConstants.ObjectFieldIdListContainsArtifactIdRegistryKey);
				if (string.IsNullOrEmpty(value))
				{
					AppSettingsReader.SetRegistryKeyValue(
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
					AppSettingsReader.SetRegistryKeyValue(
						AppSettingsConstants.ObjectFieldIdListContainsArtifactIdRegistryKey,
						string.Join(",", value.Select(x => x.ToString())));
				}
			}
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"PermissionErrorsRetry",
			AppSettingsConstants.PermissionErrorsRetryDefaultValue)]
		bool IAppSettings.PermissionErrorsRetry
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"PreviewThreshold",
			AppSettingsConstants.PreviewThresholdDefaultValue)]
		int IAppSettings.PreviewThreshold
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"ProcessFormRefreshRate",
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
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"RestUrl",
			AppSettingsConstants.RestUrlDefaultValue)]
		string IAppSettings.RestUrl
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting]
		Relativity.Import.Export.Io.RetryOptions IAppSettings.RetryOptions
		{
			get
			{
				// Always use other settings to drive these enum switches.
				Relativity.Import.Export.Io.RetryOptions value = AppSettingsConstants.RetryOptionsDefaultValue;
				if (((IAppSettings)this).PermissionErrorsRetry)
				{
					value |= Relativity.Import.Export.Io.RetryOptions.Permissions;
				}
				else
				{
					value &= ~Relativity.Import.Export.Io.RetryOptions.Permissions;
				}

				return value;
			}
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"ServicesUrl",
			AppSettingsConstants.ServicesUrlDefaultValue)]
		string IAppSettings.ServicesUrl
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"SuppressCertificateCheckOnClient",
			AppSettingsConstants.SuppressServerCertificateValidationDefaultValue)]
		bool IAppSettings.SuppressServerCertificateValidation
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"TapiAsperaBcpRootFolder")]
		string IAppSettings.TapiAsperaBcpRootFolder
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"TapiAsperaNativeDocRootLevels",
			AppSettingsConstants.TapiAsperaNativeDocRootLevelsDefaultValue)]
		int IAppSettings.TapiAsperaNativeDocRootLevels
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"BadPathErrorsRetry",
			AppSettingsConstants.TapiBadPathErrorsRetryDefaultValue)]
		bool IAppSettings.TapiBadPathErrorsRetry
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyUtility,
			"TapiBridgeExportTransferWaitingTimeInSeconds",
			AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsDefaultValue)]
		int IAppSettings.TapiBridgeExportTransferWaitingTimeInSeconds
		{
			get
			{
				if (this.tapiBridgeExportTransferWaitingTimeInSeconds < 1)
				{
					this.tapiBridgeExportTransferWaitingTimeInSeconds = AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsDefaultValue;
				}

				return this.tapiBridgeExportTransferWaitingTimeInSeconds;
			}

			set => this.tapiBridgeExportTransferWaitingTimeInSeconds = value;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"TapiForceAsperaClient",
			AppSettingsConstants.TapiForceAsperaClientDefaultValue)]
		bool IAppSettings.TapiForceAsperaClient
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"TapiForceBcpHttpClient",
			AppSettingsConstants.TapiForceBcpHttpClientDefaultValue)]
		bool IAppSettings.TapiForceBcpHttpClient
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"TapiForceClientCandidates")]
		string IAppSettings.TapiForceClientCandidates
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"TapiForceFileShareClient",
			AppSettingsConstants.TapiForceFileShareClientDefaultValue)]
		bool IAppSettings.TapiForceFileShareClient
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"TapiForceHttpClient",
			AppSettingsConstants.TapiForceHttpClientDefaultValue)]
		bool IAppSettings.TapiForceHttpClient
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"TapiLargeFileProgressEnabled",
			AppSettingsConstants.TapiLargeFileProgressEnabledDefaultValue)]
		bool IAppSettings.TapiLargeFileProgressEnabled
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"TapiMaxJobParallelism",
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
			"TapiMinDataRateMbps",
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
			"TapiPreserveFileTimestamps",
			AppSettingsConstants.TapiPreserveFileTimestampsDefaultValue)]
		bool IAppSettings.TapiPreserveFileTimestamps
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"TapiSubmitApmMetrics",
			AppSettingsConstants.TapiSubmitApmMetricsDefaultValue)]
		bool IAppSettings.TapiSubmitApmMetrics
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"TapiTargetDataRateMbps",
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
			"TapiTransferLogDirectory",
			"")]
		string IAppSettings.TapiTransferLogDirectory
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"TempDirectory",
			"")]
		string IAppSettings.TempDirectory
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"UseOldExport",
			AppSettingsConstants.UseOldExportDefaultValue)]
		bool IAppSettings.UseOldExport
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"UsePipeliningForNativeAndObjectImports",
			AppSettingsConstants.UsePipeliningForNativeAndObjectImportsDefaultValue)]
		bool IAppSettings.UsePipeliningForNativeAndObjectImports
		{
			get;
			set;
		}

		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"WaitBeforeReconnect",
			AppSettingsConstants.WaitBeforeReconnectDefaultValue)]
		int IAppSettings.WaitBeforeReconnect
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"WebAPIOperationTimeout",
			AppSettingsConstants.WebApiOperationTimeoutDefaultValue)]
		int IAppSettings.WebApiOperationTimeout
		{
			get;
			set;
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"WebServiceURL")]
		string IAppSettings.WebApiServiceUrl
		{
			get
			{
				IAppSettings dotNetSettings = this;
				string returnValue = null;
				if (!string.IsNullOrWhiteSpace(dotNetSettings.ProgrammaticWebApiServiceUrl))
				{
					returnValue = dotNetSettings.ProgrammaticWebApiServiceUrl;
				}
				else if (!string.IsNullOrWhiteSpace(this.webApiServiceUrl))
				{
					returnValue = this.webApiServiceUrl;
				}

				if (string.IsNullOrWhiteSpace(returnValue))
				{
					returnValue = AppSettingsReader.GetRegistryKeyValue(AppSettingsConstants.WebApiServiceUrlRegistryKey);
				}

				return dotNetSettings.ValidateUriFormat(returnValue);
			}

			set
			{
				IAppSettings dotNetSettings = this;
				string validatedUri = dotNetSettings.ValidateUriFormat(value);
				AppSettingsReader.SetRegistryKeyValue(AppSettingsConstants.WebApiServiceUrlRegistryKey, validatedUri);
			}
		}

		/// <inheritdoc />
		[AppSetting(
			AppSettingsConstants.SectionLegacyWinEdds,
			"WebBasedFileDownloadChunkSize",
			AppSettingsConstants.WebBasedFileDownloadChunkSizeDefaultValue)]
		int IAppSettings.WebBasedFileDownloadChunkSize
		{
			get => System.Math.Max(this.webBasedFileDownloadChunkSize, AppSettingsConstants.WebBasedFileDownloadChunkSizeMinValue);
			set => this.webBasedFileDownloadChunkSize = value;
		}

		/// <inheritdoc />
		IAppSettings IAppSettings.DeepCopy()
		{
			return new AppDotNetSettings(this);
		}

		/// <summary>
		/// Refreshes the web API service URL.
		/// </summary>
		/// <param name="value">
		/// The value.
		/// </param>
		public void RefreshWebApiServiceUrl(string value)
		{
			// This method avoids setting WebApiServiceUrl during a refresh.
			this.webApiServiceUrl = value;
		}

		/// <inheritdoc />
		string IAppSettings.ValidateUriFormat(string value)
		{
			if (!string.IsNullOrEmpty(value) && !value.Trim().EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				value = value.Trim() + "/";
			}

			try
			{
				// NOTE: This is here for validation; an improper URI will cause this to throw an
				// exception. We set it then to 'Nothing' to avoid a warning-turned-error about
				// having an unused variable. -Phil S. 12/05/2011
				// fixed 1/24/2012 - slm - return an empty string if invalid uri format.  this will cause the
				// rdc to pop up its dialog prompting the user to enter a valid address
				Uri uriObj = new Uri(value);
				uriObj = null;
				return value;
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}