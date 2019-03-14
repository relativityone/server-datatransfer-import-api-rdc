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
		// All configuration sections go here.
		public const string SectionLegacyWindowsProcess = "kCura.Windows.Process";
		public const string SectionLegacyUtility = "kCura.Utility";
		public const string SectionLegacyWinEdds = "kCura.WinEDDS";
		public const string Section = "Relativity.Import.Export";

		// All Registry keys go here.
		public const string ForceFolderPreviewRegistryKey = "ForceFolderPreview";
		public const string ObjectFieldIdListContainsArtifactIdRegistryKey = "ObjectFieldIdListContainsArtifactId";
		public const string WebApiServiceUrlRegistryKey = "WebServiceURL";

		// All default values go here.
		public const string AuditLevelDefaultValue = "FullAudit";
		public const bool CreateErrorForEmptyNativeFileDefaultValue = false;
		public const bool CreateErrorForInvalidDateDefaultValue = true;
		public const bool CreateFoldersInWebApiDefaultValue = true;
		public const bool DisableImageLocationValidationDefaultValue = false;
		public const bool DisableImageTypeValidationDefaultValue = false;
		public const bool DisableThrowOnIllegalCharactersDefaultValue = false;
		public const bool DynamicBatchResizingOnDefaultValue = true;
		public const bool EnableCaseSensitiveSearchOnImportDefaultValue = true;
		public const int ExportBatchSizeDefaultValue = 1000;
		public const int ExportErrorNumberOfRetriesDefaultValue = 20;
		public const int ExportErrorWaitTimeInSecondsDefaultValue = 30;
		public const int ExportThreadCountDefaultValue = 2;
		public const bool ForceFolderPreviewDefaultValue = true;
		public const bool ForceParallelismInNewExportDefaultValue = false;
		public const bool ForceWebUploadDefaultValue = false;
		public const int HttpTimeoutSecondsDefaultValue = 300;
		public const int ImportBatchMaxVolumeDefaultValue = 10485760;
		public const int ImportBatchSizeDefaultValue = 1000;
		public const int IoErrorNumberOfRetriesDefaultValue = 20;
		public const int IoErrorWaitTimeInSecondsDefaultValue = 30;
		public const int JobCompleteBatchSizeDefaultValue = 50000;
		public const bool LoadImportedFullTextFromServerDefaultValue = false;
		public const bool LogAllEventsDefaultValue = false;
		public const string LogConfigXmlFileNameDefaultValue = "LogConfig.xml";
		public const int MaxFilesForTapiBridgeDefaultValue = 10000;
		public const int MaxNumberOfFileExportTasksDefaultValue = 2;
		public const int MinBatchSizeDefaultValue = 100;
		public const int MaximumReloginTriesDefaultValue = 4;
		public const bool PermissionErrorsRetryDefaultValue = false;
		public const int ProcessFormRefreshRateDefaultValue = 0;
		public const string RestUrlDefaultValue = "/Relativity.REST/api";
		public const string ServicesUrlDefaultValue = "/Relativity.Services/";
		public const Relativity.Import.Export.Io.RetryOptions RetryOptionsDefaultValue = Relativity.Import.Export.Io.RetryOptions.Io;
		public const bool SuppressServerCertificateValidationDefaultValue = false;
		public const int TapiAsperaNativeDocRootLevelsDefaultValue = 1;
		public const bool TapiBadPathErrorsRetryDefaultValue = false;
		public const int TapiBridgeExportTransferWaitingTimeInSecondsDefaultValue = 600;
		public const bool TapiForceAsperaClientDefaultValue = false;
		public const bool TapiForceBcpHttpClientDefaultValue = false;
		public const bool TapiForceFileShareClientDefaultValue = false;
		public const bool TapiForceHttpClientDefaultValue = false;
		public const bool TapiLargeFileProgressEnabledDefaultValue = false;
		public const int TapiMaxJobParallelismDefaultValue = 10;
		public const int TapiMinDataRateMbpsDefaultValue = 0;
		public const bool TapiPreserveFileTimestampsDefaultValue = false;
		public const bool TapiSubmitApmMetricsDefaultValue = true;
		public const int TapiTargetDataRateMbpsDefaultValue = 100;
		public const bool UseOldExportDefaultValue = false;
		public const bool UsePipeliningForNativeAndObjectImportsDefaultValue = true;
		public const int WebApiOperationTimeoutDefaultValue = 600000;
	}
}