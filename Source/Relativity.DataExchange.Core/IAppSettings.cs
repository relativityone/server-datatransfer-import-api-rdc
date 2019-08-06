// ----------------------------------------------------------------------------
// <copyright file="IAppSettings.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Collections.Generic;

	using Relativity.DataExchange.Io;

	/// <summary>
	/// Represents an abstract object that provides thread-safe general import/export application settings.
	/// </summary>
	/// <remarks>
	/// Consider exposing this object to Import API.
	/// </remarks>
	public interface IAppSettings
	{
		/// <summary>
		/// Gets or sets the name of the application. This value is encoded within logs and potential transfer monitors.
		/// </summary>
		/// <value>
		/// The application name.
		/// </value>
		string ApplicationName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the audit level applied to import jobs.
		/// Valid values include: <c>FullAudit</c>, <c>NoSnapshot</c>, and <c>NoAudit</c>.
		/// </summary>
		/// <value>
		/// The audit level.
		/// </value>
		string AuditLevel
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to create an error when importing a zero byte file. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to create an error; otherwise, <see langword="false" />.
		/// </value>
		bool CreateErrorForEmptyNativeFile
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to create an error when importing fields with invalid dates. This is <see langword="true" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to create an error; otherwise, <see langword="false" />.
		/// </value>
		bool CreateErrorForInvalidDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to create folders using a WebAPI web service or a legacy client-side API. This is <see langword="true" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to create folders using a WebAPI web service; otherwise, <see langword="false" /> uses a legacy client-side API.
		/// </value>
		bool CreateFoldersInWebApi
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the default maximum error count.
		/// </summary>
		/// <value>
		/// The error count.
		/// </value>
		int DefaultMaxErrorCount
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to disable image location validation. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to disable image location validation; otherwise, <see langword="false" />.
		/// </value>
		bool DisableImageLocationValidation
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to disable image type validation. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to disable image type validation; otherwise, <see langword="false" />.
		/// </value>
		bool DisableImageTypeValidation
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to disable file identification using Outside In technology. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to disable disable file identification; otherwise, <see langword="false" />.
		/// </value>
		bool DisableOutsideInFileIdentification
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to disable the text file encoding check. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to disable the text file encoding check; otherwise, <see langword="false" />.
		/// </value>
		bool DisableTextFileEncodingCheck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to disable throwing exceptions when illegal characters are found within a path. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to disable throwing an exception; otherwise, <see langword="false" />.
		/// </value>
		bool DisableThrowOnIllegalCharacters
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to automatically decrease the import batch size during the import when an error occurs. This is <see langword="true" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to automatically decrease the import batch; otherwise, <see langword="false" />.
		/// </value>
		bool DynamicBatchResizingOn
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use case-sensitive file matching during imports. This is <see langword="true" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to use case-sensitive file matching; otherwise, <see langword="false" />.
		/// </value>
		bool EnableCaseSensitiveSearchOnImport
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enforce minimum retry counts. This is <see langword="true" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to enforce minimum retry counts; otherwise, <see langword="false" />.
		/// </value>
		bool EnforceMinRetryCount
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enforce minimum wait times. This is <see langword="true" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to enforce minimum wait times; otherwise, <see langword="false" />.
		/// </value>
		bool EnforceMinWaitTime
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the max number of records to export per batch.
		/// </summary>
		/// <value>
		/// The total number of records.
		/// </value>
		int ExportBatchSize
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number of retry attempts for export related fault tolerant methods.
		/// </summary>
		/// <value>
		/// The total number of retries.
		/// </value>
		int ExportErrorNumberOfRetries
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number of seconds to wait between retry attempts for export related fault tolerant methods.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		int ExportErrorWaitTimeInSeconds
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number of threads to use during export jobs.
		/// </summary>
		/// <value>
		/// The total number of threads.
		/// </value>
		int ExportThreadCount
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the maximum number of seconds to identify a file type before reaching the timeout.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		int FileTypeIdentifyTimeoutSeconds
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to force a folder preview. This is <see langword="true" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to force a folder preview; otherwise, <see langword="false" />.
		/// </value>
		bool ForceFolderPreview
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use parallelism for production exports that use the new implementation. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to use parallelism; otherwise, <see langword="false" />.
		/// </value>
		bool ForceParallelismInNewExport
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to force web-mode. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to force web-mode; otherwise, <see langword="false" />.
		/// </value>
		bool ForceWebUpload
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the HTTP timeout in seconds.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		int HttpTimeoutSeconds
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the maximum number of metadata bytes for a single single batch.
		/// </summary>
		/// <value>
		/// The total number of bytes.
		/// </value>
		int ImportBatchMaxVolume
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the max number of records to import per batch.
		/// </summary>
		/// <value>
		/// The total number of records.
		/// </value>
		int ImportBatchSize
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number of retry attempts for I/O related fault tolerant methods.
		/// </summary>
		/// <value>
		/// The total number of retries.
		/// </value>
		int IoErrorNumberOfRetries
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number of seconds to wait between retry attempts for I/O related fault tolerant methods.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		int IoErrorWaitTimeInSeconds
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the total of bytes for a single batch.
		/// </summary>
		/// <value>
		/// The batch size.
		/// </value>
		int JobCompleteBatchSize
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to load full-text data during the import. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to load full-text data; otherwise, <see langword="false" />.
		/// </value>
		bool LoadImportedFullTextFromServer
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to log all the I/O events. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to log all the I/O events; otherwise, <see langword="false" />.
		/// </value>
		bool LogAllEvents
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the Relativity Logging Xml configuration file.
		/// </summary>
		/// <value>
		/// The file name.
		/// </value>
		string LogConfigXmlFileName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the maximum number of files for each Transfer API bridge instance.
		/// </summary>
		/// <value>
		/// The maximum number of files.
		/// </value>
		int MaxFilesForTapiBridge
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the maximum number of file export tasks.
		/// </summary>
		/// <value>
		/// The maximum number of tasks.
		/// </value>
		int MaxNumberOfFileExportTasks
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the maximum number of WebAPI login attempts.
		/// </summary>
		/// <value>
		/// The maximum number of login attempts.
		/// </value>
		int MaxReloginTries
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the minimum number of records for a single batch.
		/// </summary>
		/// <value>
		/// The batch size.
		/// </value>
		int MinBatchSize
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the OAuth2 implicit credential redirect URL. This is only used for interactive processes like the RDC.
		/// </summary>
		/// <value>
		/// The redirect URL.
		/// </value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1056:UriPropertiesShouldNotBeStrings",
			Justification = "This is OK based on usage.")]
		string OAuth2ImplicitCredentialRedirectUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the list of artifacts to use for object field mapping.
		/// </summary>
		/// <value>
		/// The list of artifacts.
		/// </value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA2227:CollectionPropertiesShouldBeReadOnly",
			Justification = "This is required for backwards compatibility.")]
		IList<int> ObjectFieldIdListContainsArtifactId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the Open ID Connect HRD or home realm discovery login hint.
		/// </summary>
		/// <value>
		/// The login hint.
		/// </value>
		string OpenIdConnectHomeRealmDiscoveryHint
		{
			get;
		}

		/// <summary>
		/// Gets or sets a value indicating whether permission specific errors are retried. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to retry permissions specific errors; otherwise, <see langword="false" />.
		/// </value>
		bool PermissionErrorsRetry
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the preview threshold.
		/// </summary>
		/// <value>
		/// The threshold value.
		/// </value>
		int PreviewThreshold
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the process form refresh-rate. This is a Relativity Desktop Client specific setting.
		/// </summary>
		/// <value>
		/// The refresh rate.
		/// </value>
		int ProcessFormRefreshRate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the programmatic Relativity Web API service URL.
		/// </summary>
		/// <value>
		/// The <see cref="Uri"/> instance.
		/// </value>
		string ProgrammaticWebApiServiceUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the retry options used by all retry policy blocks. This value is read-only because the value is driven by a combination of other setting values such as <see cref="PermissionErrorsRetry"/>.
		/// </summary>
		/// <value>
		/// The <see cref="RetryOptions"/> value.
		/// </value>
		RetryOptions RetryOptions
		{
			get;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to suppress server certificate validation errors. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to enforce server certificate validation errors; otherwise, <see langword="false" />.
		/// </value>
		bool SuppressServerCertificateValidation
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the root folder associated with the BCP share.
		/// </summary>
		/// <value>
		/// The folder name.
		/// </value>
		string TapiAsperaBcpRootFolder
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number of levels that differ between the configured document root and the native file share.
		/// </summary>
		/// <value>
		/// The total number of levels.
		/// </value>
		int TapiAsperaNativeDocRootLevels
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether Transfer API retries files that fail due to invalid paths. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to retry invalid path specific errors; otherwise, <see langword="false" />.
		/// </value>
		bool TapiBadPathErrorsRetry
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the time, in seconds, that a Transfer API bridge waits before releasing the wait handle.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		int TapiBridgeExportTransferWaitingTimeInSeconds
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether Transfer API should disable treating missing files as errors. This is <see langword="false" /> by default and always <see langword="true" /> for exports.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to disable treating missing files as errors; otherwise, <see langword="false" />.
		/// </value>
		bool TapiFileNotFoundErrorsDisabled
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether Transfer API should retry missing files. This is <see langword="true" /> by default and always <see langword="false" /> for exports.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to retry missing files; otherwise, <see langword="false" />.
		/// </value>
		bool TapiFileNotFoundErrorsRetry
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to force using the Aspera transfer client. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to force the client; otherwise, <see langword="false" />.
		/// </value>
		bool TapiForceAsperaClient
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to force using the HTTP transfer client client for all load file transfers. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to force the client; otherwise, <see langword="false" />.
		/// </value>
		bool TapiForceBcpHttpClient
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a semi-colon delimited list of transfer client candidates.
		/// </summary>
		/// <value>
		/// The list of client candidates.
		/// </value>
		string TapiForceClientCandidates
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to force the file share transfer client. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to force the client; otherwise, <see langword="false" />.
		/// </value>
		bool TapiForceFileShareClient
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to force using the HTTP transfer client. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to force the client; otherwise, <see langword="false" />.
		/// </value>
		bool TapiForceHttpClient
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to raise progress events for large files. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to raise progress events; otherwise, <see langword="false" />.
		/// </value>
		bool TapiLargeFileProgressEnabled
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Transfer API maximum degree of parallelism for a single transfer job.
		/// </summary>
		/// <value>
		/// The degree of parallelism.
		/// </value>
		int TapiMaxJobParallelism
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Transfer API minimum data rate in megabits per second units.
		/// </summary>
		/// <value>
		/// The data rate.
		/// </value>
		int TapiMinDataRateMbps
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to preserve import and export file timestamps. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to preserve file timestamps; otherwise, <see langword="false" />.
		/// </value>
		bool TapiPreserveFileTimestamps
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether Transfer API should submit APM metrics when each transfer job completes. This is <see langword="true" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to submit APM metrics; otherwise, <see langword="false" />.
		/// </value>
		bool TapiSubmitApmMetrics
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether Import API should submit APM metrics periodically when executing job and on job completion. This is <see langword="true" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to submit APM metrics; otherwise, <see langword="false" />.
		/// </value>
		bool IApiSubmitApmMetrics
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether Import API should submit SUM metrics on job start and on job completion. This is <see langword="true" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to submit SUM metrics; otherwise, <see langword="false" />.
		/// </value>
		bool IApiSubmitSumMetrics
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating how often Import API should submit APM metrics during job execution.
		/// </summary>
		/// <value>
		/// Period in seconds in which we're sending metrics.
		/// </value>
		int IApiMetricsThrottlingSeconds
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Transfer API target data rate in megabits per second units.
		/// </summary>
		/// <value>
		/// The data rate.
		/// </value>
		int TapiTargetDataRateMbps
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the directory where all Transfer API transfer logs are written.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		string TapiTransferLogDirectory
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the directory used for all temp storage.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		string TempDirectory
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use the old export production implementation. This is <see langword="false" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to use the old export production implementation; otherwise, <see langword="false" /> to use the new export production implementation.
		/// </value>
		bool UseOldExport
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to execute native and object import tasks in parallel. This is <see langword="true" /> by default.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to execute tasks in parallel; otherwise, <see langword="false" />.
		/// </value>
		bool UsePipeliningForNativeAndObjectImports
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the the refresh frequency, in milliseconds, to be used for updating configuration settings.
		/// </summary>
		/// <value>
		/// The total number of milliseconds.
		/// </value>
		int ValueRefreshThreshold
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the time, in milliseconds, to wait before reconnecting.
		/// </summary>
		/// <value>
		/// The total number of milliseconds.
		/// </value>
		int WaitBeforeReconnect
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the timeout, in seconds, before a WebAPI service call throws a timeout exception.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		int WebApiOperationTimeout
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Relativity Web API service URL. This will always return <see cref="ProgrammaticWebApiServiceUrl"/> and then this value. If none are defined, a final check is made with the Windows Registry to determine if it has been set of the RDC.
		/// </summary>
		/// <value>
		/// The URL string.
		/// </value>
		string WebApiServiceUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the number of bytes used when downloading chunks using web mode.
		/// </summary>
		/// <value>
		/// The total number of bytes.
		/// </value>
		int WebBasedFileDownloadChunkSize
		{
			get;
			set;
		}

		/// <summary>
		/// Performs a deep copy of this instance.
		/// </summary>
		/// <returns>
		/// The <see cref="IAppSettings"/> instance.
		/// </returns>
		IAppSettings DeepCopy();

		/// <summary>
		/// Validates that the URI is valid and returns a properly formatted URI string.
		/// </summary>
		/// <param name="value">
		/// The URI value.
		/// </param>
		/// <returns>
		/// The properly formatted URI string.
		/// </returns>
		string ValidateUriFormat(string value);
	}
}