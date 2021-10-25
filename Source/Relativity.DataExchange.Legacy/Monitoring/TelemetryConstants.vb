Namespace Monitoring
	Public Class TelemetryConstants

		''' <summary>
		''' Contain values for <see cref="MetricBase.BucketName"/>.
		''' </summary>
		Class BucketName
			Public Const METRIC_JOB_STARTED As String = "RDC.MetricJobStarted"
			Public Const METRIC_JOB_PROGRESS As String = "RDC.MetricJobProgress"
			Public Const METRIC_JOB_END_REPORT As String = "RDC.MetricJobEndReport"
			Public Const METRIC_JOB_BATCH As String = "RDC.MetricJobBatch"
			' This bucket name starts with "RDC" because that's registered prefix for our metrics. Authentication metrics are only send from ImportAPI class.
			Public Const METRIC_AUTHENTICATION_TYPE As String = "RDC.MetricImportAPIAuthenticationType"
		End Class

		class SumBucketPrefix
			Public Const JOB_STARTED_COUNT As String = "RDC.Performance.JobStartedCount"
			Public Const TOTAL_RECORDS As String = "RDC.Usage.TotalRecords"
			Public Const COMPLETED_RECORDS As String = "RDC.Usage.CompletedRecords"
			Public Const JOB_SIZE As String = "RDC.Usage.JobSize"
			Public Const JOB_COMPLETED_COUNT As String = "RDC.Performance.JobCompletedCount"
			Public Const JOB_FAILED_COUNT As String = "RDC.Performance.JobFailedCount"
			Public Const JOB_CANCELLED_COUNT As String = "RDC.Performance.JobCancelledCount"
			Public Const THROUGHPUT As String = "RDC.Performance.Throughput"
			Public Const THROUGHPUT_BYTES As String = "RDC.Performance.ThroughputBytes"
			' This bucket name starts with "RDC" because that's registered prefix for our metrics. Authentication metrics are only send from ImportAPI class.
			Public Const AUTHENTICATION As String = "RDC.IAPI.Authentication"
			Public Const SQL_THROUGHPUT As String = "RDC.Performance.SqlBulkImportThroughput"
			Public Const COMPLETED_FILES As String = "RDC.Usage.CompletedFiles"
		End Class

		''' <summary>
		''' Contain key names for <see cref="MetricBase.CustomData"/> dictionary pairs.
		''' </summary>
		Class KeyName
			Public Const TRANSFER_DIRECTION As String = "JobType"
			Public Const WORKSPACE_ID As String = "WorkspaceID"
			Public Const CORRELATION_ID As String = "CorrelationID"
			Public Const UNIT_OF_MEASURE As String = "UnitOfMeasure"
			Public Const TRANSFER_MODE As String = "TransferMode"
			Public Const INITIAL_TRANSFER_MODE As String = "InitialTransferMode"
			Public Const APPLICATION_NAME As String = "ApplicationName"
			Public Const USE_OLD_EXPORT As String = "UseOldExport"
			Public Const FILE_THROUGHPUT As String = "FileThroughputBytesPerSecond"
			Public Const METADATA_THROUGHPUT As String = "MetadataThroughputBytesPerSecond"
			Public Const SQL_THROUGHPUT As String = "SqlBulkLoadThroughputRecordsPerSecond"
			Public Const JOB_STATUS As String = "JobStatus"
			Public Const TOTAL_SIZE_BYTES As String = "TotalSizeBytes"
			Public Const FILE_SIZE_BYTES As String = "FileSizeBytes"
			Public Const METADATA_SIZE_BYTES As String = "MetadataSizeBytes"
			Public Const TOTAL_RECORDS As String = "TotalRecords"
			Public Const COMPLETED_RECORDS As String = "CompletedRecords"
			Public Const RECORDS_WITH_ERRORS As String = "RecordsWithErrors"
			Public Const THROUGHPUT_BYTES_PER_SECOND As String = "ThroughputBytesPerSecond"
			Public Const THROUGHPUT_RECORDS_PER_SECOND As String = "ThroughputRecordsPerSecond"
			Public Const AUTHENTICATION_METHOD As String = "AuthenticationMethod"
			Public Const SYSTEM_TYPE As String = "SystemType"
			Public Const SUB_SYSTEM_TYPE As String = "SubSystemType"
			Public Const IMPORT_OBJECT_TYPE As String = "ImportObjectType"
			Public Const JOB_DURATION As String = "JobDurationInSeconds"
			Public Const IMPORT_API_VERSION As String = "ImportApiVersion"
			Public Const RELATIVITY_VERSION As String = "RelativityVersion"
			Public Const BATCH_NUMBER As String = "BatchNumber"
			Public Const MASS_IMPORT_DURATION As String = "MassImportDurationMilliseconds"
			Public Const NUMBER_OF_RECORDS As String = "NumberOfRecords"
			Public Const NUMBER_OF_FILES As String = "NumberOfFiles"
			Public Const JOB_START_TIMESTAMP As String = "JobStartTimeStamp"
			Public Const JOB_END_TIMESTAMP As String = "JobEndTimeStamp"
			Public Const JOB_RUN_ID As String = "JobRunId"
			Public Const EXPORTED_NATIVE_COUNT As String = "ExportedNativeCount"
			Public Const EXPORTED_PDF_COUNT As String = "ExportedPdfCount"
			Public Const EXPORTED_IMAGE_COUNT As String = "ExportedImageCount"
			Public Const EXPORTED_LONG_TEXT_COUNT As String = "ExportedLongTextCount"
			Public Const TOTAL_PHYSICAL_MEMORY As String = "TotalPhysicalMemory"
			Public Const AVAILABLE_PHYSICAL_MEMORY As String = "AvailablePhysicalMemory"
			Public Const OPERATING_SYSTEM_NAME As String = "OperatingSystemName"
			Public Const OPERATING_SYSTEM_VERSION As String = "OperatingSystemVersion"
			Public Const IS_64_BIT_OS As String = "Is64BitOperatingSystem"
			Public Const IS_64_BIT_PROCESS As String = "Is64BitProcess"
			Public Const CPU_COUNT As String = "CpuCount"
			Public Const CALLING_ASSEMBLY As String = "CallingAssembly"
		End Class

		''' <summary>
		''' Contain values for <see cref="MetricBase.CustomData"/> dictionary pairs.
		''' </summary>
		Class Values
			Public Const NOT_APPLICABLE As String = "N/A"
		End Class

		''' <summary>
		''' Contain values for <see cref="MetricJobEndReport.JobStatus"/> property.
		''' </summary>
		Enum JobStatus
			Completed
			Failed
			Cancelled
		End Enum

		''' <summary>
		''' Contain values for <see cref="MetricAuthenticationType.AuthenticationMethod"/> property.
		''' </summary>
		Enum AuthenticationMethod
			UsernamePassword
			BearerToken
			Windows
		End Enum

		Enum ImportObjectType
			NotApplicable
			Image
			ProductionImage
			Native
			Objects
		End Enum

		Enum TransferDirection
			Import
			Export
		End Enum
	End Class
End Namespace