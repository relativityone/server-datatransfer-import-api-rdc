﻿Imports System.Collections.Generic
Imports Relativity.Services.DataContracts.DTOs.MetricsCollection
Imports Relativity.Telemetry.DataContracts.Shared

Namespace Monitoring
    Public Class MetricJobEndReport
        Inherits MetricBase

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.METRIC_JOB_END_REPORT

        ''' <inheritdoc/>
        Public Overrides Function GenerateSumMetrics() As List(Of MetricRef)
            Return New List(Of MetricRef) From {
                New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.TOTAL_RECORDS), Guid.Empty, CorrelationID, MetricTypes.Counter, TotalRecords),
                New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.COMPLETED_RECORDS), Guid.Empty, CorrelationID, MetricTypes.Counter, CompletedRecords),
                New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.JOB_SIZE), Guid.Empty, CorrelationID, MetricTypes.Counter, TotalSizeBytes),
                New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.THROUGHPUT), Guid.Empty, CorrelationID, MetricTypes.PointInTimeDouble, ThroughputRecordsPerSecond),
                New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.THROUGHPUT_BYTES), Guid.Empty, CorrelationID, MetricTypes.PointInTimeDouble, ThroughputBytesPerSecond),
                New MetricRef(FormatSumBucketName(CStr(IIf(JobStatus = TelemetryConstants.JobStatus.COMPLETED, TelemetryConstants.SumBucketPrefix.JOB_COMPLETED_COUNT, TelemetryConstants.SumBucketPrefix.JOB_FAILED_COUNT))), Guid.Empty, CorrelationID, MetricTypes.Counter, 1)
                }
        End Function

        ''' <summary>
        ''' Gets or sets job status - <see cref="TelemetryConstants.JobStatus"/>
        ''' </summary>
        ''' <returns>Job status</returns>
        Public Property JobStatus() As String
            Get
                Return GetValueOrDefault (Of String)(TelemetryConstants.KeyName.JOB_STATUS)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.JOB_STATUS) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets total job size in bytes. This value is equal to sum of <see cref="MetadataSizeBytes"/> and <see cref="FileSizeBytes"/>.
        ''' </summary>
        ''' <returns>Total job size in bytes</returns>
        Public Property TotalSizeBytes() As Long
            Get
                Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.TOTAL_SIZE_BYTES)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.TOTAL_SIZE_BYTES) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets total size of all files in bytes.
        ''' </summary>
        ''' <returns>Total size of all files in bytes</returns>
        Public Property FileSizeBytes() As Long
            Get
                Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.FILE_SIZE_BYTES)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.FILE_SIZE_BYTES) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets metadata size in bytes.
        ''' </summary>
        ''' <returns>Metadata size in bytes</returns>
        Public Property MetadataSizeBytes As Long
            Get
                Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.METADATA_SIZE_BYTES)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.METADATA_SIZE_BYTES) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets total number of records to process in a job
        ''' </summary>
        ''' <returns>Total number of records</returns>
        Public Property TotalRecords As Long
            Get
                Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.TOTAL_RECORDS)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.TOTAL_RECORDS) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the number of successfully processed records
        ''' </summary>
        ''' <returns>Number of completed records</returns>
        Public Property CompletedRecords As Long
            Get
                Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.COMPLETED_RECORDS)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.COMPLETED_RECORDS) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets total transfer rate in bytes per second
        ''' </summary>
        ''' <returns>Transfer rate in bytes per second</returns>
        Public Property ThroughputBytesPerSecond As Double
            Get
                Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.THROUGHPUT_BYTES_PER_SECOND)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.THROUGHPUT_BYTES_PER_SECOND) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets total transfer rate in records per second
        ''' </summary>
        ''' <returns>Transfer rate in records per second</returns>
        Public Property ThroughputRecordsPerSecond As Double
            Get
                Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.THROUGHPUT_RECORDS_PER_SECOND)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.THROUGHPUT_RECORDS_PER_SECOND) = Value
            End Set
        End Property
    End Class
End NameSpace