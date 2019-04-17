// ----------------------------------------------------------------------------
// <copyright file="AppSettingsConstants.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	/// <summary>
	/// Defines all application settings constant keys and default values.
	/// </summary>
	public static class AppSettingsConstants
	{
#pragma warning disable SA1600
		// All configuration sections go here.
		public const string SectionLegacykCuraConfig = "kCura.Config";
		public const string SectionLegacyWindowsProcess = "kCura.Windows.Process";
		public const string SectionLegacyUtility = "kCura.Utility";
		public const string SectionLegacyWinEdds = "kCura.WinEDDS";
		public const string SectionImportExport = "Relativity.Import.Export";

		// All Registry keys go here.
		public const string ForceFolderPreviewRegistryKey = "ForceFolderPreview";
		public const string ObjectFieldIdListContainsArtifactIdRegistryKey = "ObjectFieldIdListContainsArtifactId";
		public const string OpenIdConnectHomeRealmDiscoveryHintKey = "HRDHint";
		public const string WebApiServiceUrlRegistryKey = "WebServiceURL";

		// All default values go here.
		public const string ApplicationNameKey = "ApplicationName";
		public const string AuditLevelKey = "AuditLevel";
		public const string AuditLevelDefaultValue = "FullAudit";
		public const string CreateErrorForInvalidDateKey = "CreateErrorForInvalidDate";
		public const bool CreateErrorForInvalidDateDefaultValue = true;
		public const string CreateErrorForEmptyNativeFileKey = "CreateErrorForEmptyNativeFile";
		public const bool CreateErrorForEmptyNativeFileDefaultValue = false;
		public const string CreateFoldersInWebApiKey = "CreateFoldersInWebAPI";
		public const bool CreateFoldersInWebApiDefaultValue = true;
		public const string DefaultMaxErrorCountKey = "DefaultMaxErrorCount";
		public const int DefaultMaxErrorCountDefaultValue = 1000;
		public const string DisableImageLocationValidationKey = "DisableImageLocationValidation";
		public const bool DisableImageLocationValidationDefaultValue = false;
		public const string DisableImageTypeValidationKey = "DisableImageTypeValidation";
		public const bool DisableImageTypeValidationDefaultValue = false;
		public const string DisableOutsideInFileIdentificationKey = "DisableNativeValidation";
		public const bool DisableOutsideInFileIdentificationDefaultValue = false;
		public const string DisableTextFileEncodingCheckKey = "DisableTextFileEncodingCheck";
		public const bool DisableTextFileEncodingCheckDefaultValue = false;
		public const string DisableThrowOnIllegalCharactersKey = "DisableNativeLocationValidation";
		public const bool DisableThrowOnIllegalCharactersDefaultValue = false;
		public const string DynamicBatchResizingOnKey = "DynamicBatchResizingOn";
		public const bool DynamicBatchResizingOnDefaultValue = true;
		public const string EnableCaseSensitiveSearchOnImportKey = "EnableCaseSensitiveSearchOnImport";
		public const bool EnableCaseSensitiveSearchOnImportDefaultValue = true;
		public const string EnableSingleModeImportKey = "EnableSingleModeImport";
		public const bool EnableSingleModeImportDefaultValue = false;
		public const string EnforceVersionCompatibilityCheckKey = "EnforceVersionCompatibilityCheck";
		public const bool EnforceVersionCompatibilityCheckDefaultValue = false;
		public const string ExportBatchSizeKey = "ExportBatchSize";
		public const int ExportBatchSizeDefaultValue = 1000;
		public const string ExportErrorNumberOfRetriesKey = "ExportErrorNumberOfRetries";
		public const int ExportErrorNumberOfRetriesDefaultValue = 20;
		public const string ExportErrorWaitTimeInSecondsKey = "ExportErrorWaitTimeInSeconds";
		public const int ExportErrorWaitTimeInSecondsDefaultValue = 30;
		public const string ExportThreadCountKey = "ExportThreadCount";
		public const int ExportThreadCountDefaultValue = 2;
		public const bool ForceFolderPreviewDefaultValue = true;
		public const string ForceParallelismInNewExportKey = "ForceParallelismInNewExport";
		public const bool ForceParallelismInNewExportDefaultValue = false;
		public const string ForceWebUploadKey = "ForceWebUpload";
		public const bool ForceWebUploadDefaultValue = false;
		public const string HttpTimeoutSecondsKey = "HttpTimeoutSeconds";
		public const int HttpTimeoutSecondsDefaultValue = 300;
		public const string ImportBatchMaxVolumeKey = "ImportBatchMaxVolume";
		public const int ImportBatchMaxVolumeDefaultValue = 10485760;
		public const string ImportBatchSizeKey = "ImportBatchSize";
		public const int ImportBatchSizeDefaultValue = 1000;
		public const string IoErrorNumberOfRetriesKey = "IOErrorNumberOfRetries";
		public const int IoErrorNumberOfRetriesDefaultValue = 20;
		public const string IoErrorWaitTimeInSecondsKey = "IOErrorWaitTimeInSeconds";
		public const int IoErrorWaitTimeInSecondsDefaultValue = 30;
		public const string JobCompleteBatchSizeKey = "JobCompleteBatchSize";
		public const int JobCompleteBatchSizeDefaultValue = 50000;
		public const string LoadImportedFullTextFromServerKey = "LoadImportedFullTextFromServer";
		public const bool LoadImportedFullTextFromServerDefaultValue = false;
		public const string LogAllEventsKey = "LogAllEvents";
		public const bool LogAllEventsDefaultValue = false;
		public const string LogConfigXmlFileNameKey = "LogConfigFile";
		public const string LogConfigXmlFileNameDefaultValue = "LogConfig.xml";
		public const string MaxFilesForTapiBridgeKey = "MaximumFilesForTapiBridge";
		public const int MaxFilesForTapiBridgeDefaultValue = 10000;
		public const string MaxNumberOfFileExportTasksKey = "MaxNumberOfFileExportTasks";
		public const int MaxNumberOfFileExportTasksDefaultValue = 2;
		public const string MinBatchSizeKey = "MinimumBatchSize";
		public const int MinBatchSizeDefaultValue = 100;
		public const string MaximumReloginTriesKey = "MaximumReloginTries";
		public const int MaximumReloginTriesDefaultValue = 4;
		public const string PermissionErrorsRetryKey = "PermissionErrorsRetry";
		public const bool PermissionErrorsRetryDefaultValue = false;
		public const string PreviewThresholdKey = "PreviewThreshold";
		public const int PreviewThresholdDefaultValue = 1000;
		public const string ProcessFormRefreshRateKey = "ProcessFormRefreshRate";
		public const int ProcessFormRefreshRateDefaultValue = 0;
		public const string RestUrlKey = "RestUrl";
		public const string RestUrlDefaultValue = "/Relativity.REST/api";
		public const string ServicesUrlKey = "ServicesUrl";
		public const string ServicesUrlDefaultValue = "/Relativity.Services/";
		public const Relativity.Import.Export.Io.RetryOptions RetryOptionsDefaultValue = Relativity.Import.Export.Io.RetryOptions.Io;
		public const string SuppressServerCertificateValidationKey = "SuppressCertificateCheckOnClient";
		public const bool SuppressServerCertificateValidationDefaultValue = false;
		public const string TapiAsperaBcpRootFolderKey = "TapiAsperaBcpRootFolder";
		public const string TapiAsperaBcpRootFolderDefaultValue = "";
		public const string TapiAsperaNativeDocRootLevelsKey = "TapiAsperaNativeDocRootLevels";
		public const int TapiAsperaNativeDocRootLevelsDefaultValue = 1;
		public const string TapiBadPathErrorsRetryKey = "BadPathErrorsRetry";
		public const bool TapiBadPathErrorsRetryDefaultValue = false;
		public const string TapiBridgeExportTransferWaitingTimeInSecondsKey = "TapiBridgeExportTransferWaitingTimeInSeconds";
		public const int TapiBridgeExportTransferWaitingTimeInSecondsDefaultValue = 600;
		public const string TapiForceAsperaClientKey = "TapiForceAsperaClient";
		public const bool TapiForceAsperaClientDefaultValue = false;
		public const string TapiForceBcpHttpClientKey = "TapiForceBcpHttpClient";
		public const bool TapiForceBcpHttpClientDefaultValue = false;
		public const string TapiForceClientCandidatesKey = "TapiForceClientCandidates";
		public const string TapiForceClientCandidatesDefaultValue = "";
		public const string TapiForceFileShareClientKey = "TapiForceFileShareClient";
		public const bool TapiForceFileShareClientDefaultValue = false;
		public const string TapiForceHttpClientKey = "TapiForceHttpClient";
		public const bool TapiForceHttpClientDefaultValue = false;
		public const string TapiLargeFileProgressEnabledKey = "TapiLargeFileProgressEnabled";
		public const bool TapiLargeFileProgressEnabledDefaultValue = false;
		public const string TapiMaxJobParallelismKey = "TapiMaxJobParallelism";
		public const int TapiMaxJobParallelismDefaultValue = 10;
		public const string TapiMinDataRateMbpsKey = "TapiMinDataRateMbps";
		public const int TapiMinDataRateMbpsDefaultValue = 0;
		public const string TapiPreserveFileTimestampsKey = "TapiPreserveFileTimestamps";
		public const bool TapiPreserveFileTimestampsDefaultValue = false;
		public const string TapiSubmitApmMetricsKey = "TapiSubmitApmMetrics";
		public const bool TapiSubmitApmMetricsDefaultValue = true;
		public const string TapiTargetDataRateMbpsKey = "TapiTargetDataRateMbps";
		public const string TapiTransferLogDirectoryKey = "TapiTransferLogDirectory";
		public const string TapiTransferLogDirectoryDefaultValue = "";
		public const string TempDirectoryKey = "TempDirectory";
		public const string TempDirectoryDefaultValue = "";
		public const int TapiTargetDataRateMbpsDefaultValue = 100;
		public const string UseOldExportKey = "UseOldExport";
		public const bool UseOldExportDefaultValue = false;
		public const string UsePipeliningForNativeAndObjectImportsKey = "UsePipeliningForNativeAndObjectImports";
		public const bool UsePipeliningForNativeAndObjectImportsDefaultValue = true;
		public const string ValueRefreshThresholdKey = "ValueRefreshThreshold";
		public const int ValueRefreshThresholdDefaultValue = 10000;
		public const string WaitBeforeReconnectKey = "WaitBeforeReconnect";
		public const int WaitBeforeReconnectDefaultValue = 2000;
		public const string WebApiOperationTimeoutKey = "WebAPIOperationTimeout";
		public const int WebApiOperationTimeoutDefaultValue = 600000;
		public const string WebBasedFileDownloadChunkSizeKey = "WebBasedFileDownloadChunkSize";
		public const int WebBasedFileDownloadChunkSizeDefaultValue = 1048576;
		public const int WebBasedFileDownloadChunkSizeMinValue = 1024;
		public const string WebApiServiceUrlKey = "WebServiceURL";
		public const string WebApiServiceUrlDefaultValue = "";
#pragma warning restore SA1600
	}
}