Namespace Monitoring
    Public Class TelemetryConstants

        ''' <summary>
        ''' Contain values for <see cref="TransferJobMessageBase.BucketName"/>
        ''' </summary>
        Class BucketName
            Public Const JOB_STARTED_COUNT As String = "RDC.Performance.JobStartedCount"
            Public Const JOB_COMPLETED_COUNT As String = "RDC.Performance.JobCompletedCount"
            Public Const JOB_FAILED_COUNT As String = "RDC.Performance.JobFailedCount"
            Public Const JOB_STATISTICS As String = "RDC.Performance.JobStatistics"
            Public Const TOTAL_RECORDS As String = "RDC.Usage.TotalRecords"
            Public Const COMPLETED_RECORDS As String = "RDC.Usage.CompletedRecords"
            Public Const THROUGHPUT As String = "RDC.Performance.Throughput"
            Public Const PROGRESS As String = "RDC.Performance.Progress"
        End Class

        ''' <summary>
        ''' Contain values for <see cref="TransferJobMessageBase.CustomData"/> dictionary key names
        ''' </summary>
        Class KeyName
            Public Const JOB_TYPE As String = "JobType"
            Public Const TRANSFER_MODE As String = "TransferMode"
            Public Const APPLICATION_NAME As String = "ApplicationName"
            Public Const TOTAL_RECORDS As String = "TotalRecords"
            Public Const COMPLETED_RECORDS As String = "CompletedRecords"
            Public Const FILE_THROUGHPUT As String = "FileThroughput"
            Public Const METADATA_THROUGHPUT As String = "MetadataThroughput"
            Public Const BYTES_PER_SECOND As String = "BytesPerSecond"
            Public Const RECORDS_PER_SECOND As String = "RecordsPerSecond"
            Public Const JOB_SIZE_IN_BYTES As String = "JobSizeInBytes"
            Public Const METADATA_BYTES As String = "MetadataBytes"
            Public Const FILE_BYTES As String = "FileBytes"
            Public Const USE_OLD_EXPORT As String = "UseOldExport"
        End Class
    End Class
End Namespace