Imports System.Collections.Generic
Imports Relativity.Services.DataContracts.DTOs.MetricsCollection
Imports Relativity.Telemetry.DataContracts.Shared

Namespace Monitoring.Sinks
    Public Class SumMetricFormatter
        Implements ISumMetricFormatter

        ''' <inheritdoc/>
        Public Function GenerateSumMetrics(metric As MetricBase) As List(Of MetricRef) Implements ISumMetricFormatter.GenerateSumMetrics
            Select Case metric.GetType()
                Case GetType(MetricJobStarted)
                    Return GenerateSumMetricJobStarted(CType(metric, MetricJobStarted))
                Case GetType(MetricJobEndReport)
                    Return GenerateSumMetricJobEndReport(CType(metric, MetricJobEndReport))
                Case Else
                    Return New List(Of MetricRef)()
            End Select
        End Function

        Private Function GenerateSumMetricJobStarted(metric As MetricJobStarted) As List(Of MetricRef)
            Return New List(Of MetricRef) From {
                New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.JOB_STARTED_COUNT, metric.JobType, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.Counter, 1)
                }
        End Function

        Private Function GenerateSumMetricJobEndReport(metric As MetricJobEndReport) As List(Of MetricRef)
            Return New List(Of MetricRef) From {
                New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.TOTAL_RECORDS, metric.JobType, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeLong, metric.TotalRecords),
                New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.COMPLETED_RECORDS, metric.JobType, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeLong, metric.CompletedRecords),
                New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.JOB_SIZE, metric.JobType, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeLong, metric.TotalSizeBytes),
                New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.THROUGHPUT, metric.JobType, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeDouble, metric.ThroughputRecordsPerSecond),
                New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.THROUGHPUT_BYTES, metric.JobType, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeDouble, metric.ThroughputBytesPerSecond),
                New MetricRef(FormatSumBucketName(GetBucketPrefixFromJobStatus(metric.JobStatus), metric.JobType, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.Counter, 1)
                }
        End Function

        ''' <summary>
        ''' Formatting SUM bucket name. SUM metrics does not allow to add custom properties so we need to pass <see cref="MetricBase.JobType"/> and <see cref="MetricBase.TransferMode"/> in bucket name.
        ''' </summary>
        ''' <param name="prefix">Bucket name prefix. This values are stored in <see cref="TelemetryConstants.SumBucketPrefix"/>.</param>
        ''' <param name="jobType">Job type - Import or Export</param>
        ''' <param name="transferMode">Transfer mode - Direct, Aspera or Web</param>
        ''' <returns></returns>
        Private Function FormatSumBucketName(prefix As String, jobType As String, transferMode As String) As String
            Return $"{prefix}.{jobType}.{transferMode}"
        End Function

        Private Function IntegerToGuid(value As Integer) As Guid
            Return New Guid(CUInt(value).ToString().PadRight(32, "f"c).Substring(0, 32))
        End Function

		Private Function GetBucketPrefixFromJobStatus(jobStatus As TelemetryConstants.JobStatus) As String
			Select jobStatus
			    Case TelemetryConstants.JobStatus.Completed
					Return TelemetryConstants.SumBucketPrefix.JOB_COMPLETED_COUNT
				Case TelemetryConstants.JobStatus.Failed
					Return TelemetryConstants.SumBucketPrefix.JOB_FAILED_COUNT
				Case TelemetryConstants.JobStatus.Cancelled
					Return TelemetryConstants.SumBucketPrefix.JOB_CANCELLED_COUNT
				Case Else
					Return jobStatus.ToString()
			End Select
		End Function
    End Class
End NameSpace