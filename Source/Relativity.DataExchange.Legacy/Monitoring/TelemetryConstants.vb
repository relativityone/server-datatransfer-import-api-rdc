Namespace Monitoring
    Public Class TelemetryConstants

        ''' <summary>
        ''' Contain values for <see cref="MetricBase.BucketName"/>.
        ''' </summary>
        Class BucketName
            Public Const METRIC_JOB_STARTED As String = "RDC.MetricJobStarted"
            Public Const METRIC_JOB_PROGRESS As String = "RDC.MetricJobProgress"
            Public Const METRIC_JOB_END_REPORT As String = "RDC.MetricJobEndReport"
        End Class

        class SumBucketPrefix
            Public Const JOB_STARTED_COUNT As String = "RDC.Performance.JobStartedCount"
            Public Const TOTAL_RECORDS As String = "RDC.Usage.TotalRecords"
            Public Const COMPLETED_RECORDS As String = "RDC.Usage.CompletedRecords"
            Public Const JOB_SIZE As String = "RDC.Usage.JobSize"
            Public Const JOB_COMPLETED_COUNT As String = "RDC.Performance.JobCompletedCount"
            Public Const JOB_FAILED_COUNT As String = "RDC.Performance.JobFailedCount"
            Public Const THROUGHPUT As String = "RDC.Performance.Throughput"
            Public Const THROUGHPUT_BYTES As String = "RDC.Performance.ThroughputBytes"
        End Class

        ''' <summary>
        ''' Contain key names for <see cref="MetricBase.CustomData"/> dictionary pairs.
        ''' </summary>
        Class KeyName
            Public Const JOB_TYPE As String = "JobType"
            Public Const WORKSPACE_ID As String = "WorkspaceID"
            Public Const CORRELATION_ID As String = "CorrelationID"
            Public Const UNIT_OF_MEASURE As String = "UnitOfMeasure"
            Public Const TRANSFER_MODE As String = "TransferMode"
            Public Const APPLICATION_NAME As String = "ApplicationName"
            Public Const USE_OLD_EXPORT As String = "UseOldExport"
            Public Const FILE_THROUGHPUT As String = "FileThroughputBytesPerSecond"
            Public Const METADATA_THROUGHPUT As String = "MetadataThroughput"
            Public Const SQL_THROUGHPUT As String = "SqlBulkLoadThroughput"
            Public Const JOB_STATUS As String = "JobStatus"
            Public Const TOTAL_SIZE_BYTES As String = "TotalSizeBytes"
            Public Const FILE_SIZE_BYTES As String = "FileSizeBytes"
            Public Const METADATA_SIZE_BYTES As String = "MetadataSizeBytes"
            Public Const TOTAL_RECORDS As String = "TotalRecords"
            Public Const COMPLETED_RECORDS As String = "CompletedRecords"
            Public Const THROUGHPUT_BYTES_PER_SECOND As String = "ThroughputBytesPerSecond"
            Public Const THROUGHPUT_RECORDS_PER_SECOND As String = "ThroughputRecordsPerSecond"
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
        Class JobStatus
            Public Const COMPLETED As String = "Completed"
            Public Const FAILED As String = "Failed"
        End Class
    End Class
End Namespace