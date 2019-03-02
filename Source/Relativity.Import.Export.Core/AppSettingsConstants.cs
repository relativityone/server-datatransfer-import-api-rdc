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
		// All keys go here.
		public const string CreateErrorForInvalidDateKey = "CreateErrorForInvalidDate";
		public const string DisableThrowOnIllegalCharactersKey = "DisableNativeLocationValidation";
		public const string ExportErrorNumberOfRetriesKey = "ExportErrorNumberOfRetries";
		public const string ExportErrorWaitTimeInSecondsKey = "ExportErrorWaitTimeInSeconds";
		public const string ForceFolderPreviewKey = "ForceFolderPreview";
		public const string IoErrorNumberOfRetriesKey = "IOErrorNumberOfRetriesKey";
		public const string IoErrorWaitTimeInSecondsKey = "IOErrorWaitTimeInSeconds";
		public const string LogAllEventsKey = "LogAllEvents";
		public const string MaxNumberOfFileExportTasksKey = "MaxNumberOfFileExportTasks";
		public const string MaximumFilesForTapiBridgeKey = "MaximumFilesForTapiBridge";
		public const string ObjectFieldIdListContainsArtifactIdKey = "ObjectFieldIdListContainsArtifactId";
		public const string ProgrammaticWebApiServiceUrlKey = "ProgrammaticWebApiServiceUrl";
		public const string TapiBridgeExportTransferWaitingTimeInSecondsKey = "TapiBridgeExportTransferWaitingTimeInSeconds";
		public const string WebApiServiceUrl = "WebServiceURL";

		// All default values go here.
		public const bool CreateErrorForInvalidDateDefaultValue = true;
		public const bool DisableThrowOnIllegalCharactersDefaultValue = false;
		public const int ExportErrorNumberOfRetriesDefaultValue = 20;
		public const int ExportErrorNumberOfRetriesMinValue = 1;
		public const int ExportErrorWaitTimeInSecondsDefaultValue = 30;
		public const int ExportErrorWaitTimeInSecondsMinValue = 1;
		public const bool ForceFolderPreviewDefaultValue = true;
		public const int IoErrorNumberOfRetriesDefaultValue = 20;
		public const int IoErrorNumberOfRetriesMinValue = 1;
		public const int IoErrorWaitTimeInSecondsDefaultValue = 30;
		public const int IoErrorWaitTimeInSecondsMinValue = 1;
		public const bool LogAllEventsDefaultValue = false;
		public const int MaximumFilesForTapiBridgeDefaultValue = 10000;
		public const int MaximumFilesForTapiBridgeMinValue = 1;
		public const int MaxNumberOfFileExportTasksDefaultValue = 2;
		public const int MaxNumberOfFileExportTasksMinValue = 2;
		public const int TapiBridgeExportTransferWaitingTimeInSecondsDefaultValue = 600;
		public const int TapiBridgeExportTransferWaitingTimeInSecondsMinValue = 1;
	}
}