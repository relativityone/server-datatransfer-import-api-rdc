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
				Case GetType(MetricAuthenticationType)
					Return GenerateSumMetricAuthenticationType(CType(metric, MetricAuthenticationType))
				Case Else
					Return New List(Of MetricRef)()
			End Select
		End Function

		Private Function GenerateSumMetricJobStarted(metric As MetricJobStarted) As List(Of MetricRef)
			Return New List(Of MetricRef) From {
				New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.JOB_STARTED_COUNT, metric.TransferDirection, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.Counter, 1)
				}
		End Function

		Private Function GenerateSumMetricJobEndReport(metric As MetricJobEndReport) As List(Of MetricRef)
			Dim metrics As List(Of MetricRef) = New List(Of MetricRef) From {
				New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.TOTAL_RECORDS, metric.TransferDirection, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeLong, metric.TotalRecords),
				New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.COMPLETED_RECORDS, metric.TransferDirection, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeLong, metric.CompletedRecords),
				New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.JOB_SIZE, metric.TransferDirection, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeLong, metric.TotalSizeBytes),
				New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.THROUGHPUT, metric.TransferDirection, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeDouble, metric.ThroughputRecordsPerSecond),
				New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.THROUGHPUT_BYTES, metric.TransferDirection, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeDouble, metric.ThroughputBytesPerSecond),
				New MetricRef(FormatSumBucketName(GetBucketPrefixFromJobStatus(metric.JobStatus), metric.TransferDirection, metric.TransferMode), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.Counter, 1)}
			If metric.TransferDirection = TelemetryConstants.TransferDirection.Import Then
				metrics.Add(New MetricRef($"{TelemetryConstants.SumBucketPrefix.SQL_THROUGHPUT}.{metric.ImportObjectType}", IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeDouble, metric.SqlBulkLoadThroughputRecordsPerSecond))
			End If
			If metric.TransferDirection = TelemetryConstants.TransferDirection.Export Then
				metrics.Add(New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.COMPLETED_FILES, metric.TransferMode, "Native"), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeLong, metric.ExportedNativeCount))
				metrics.Add(New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.COMPLETED_FILES, metric.TransferMode, "Pdf"), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeLong, metric.ExportedPdfCount))
				metrics.Add(New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.COMPLETED_FILES, metric.TransferMode, "Image"), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeLong, metric.ExportedImageCount))
				metrics.Add(New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.COMPLETED_FILES, metric.TransferMode, "LongText"), IntegerToGuid(metric.WorkspaceID), metric.CorrelationID, MetricTypes.PointInTimeLong, metric.ExportedLongTextCount))
			End If
			Return metrics
		End Function

		Private Function GenerateSumMetricAuthenticationType(metric As MetricAuthenticationType) As List(Of MetricRef)
			Return New List(Of MetricRef) From {
				New MetricRef($"{TelemetryConstants.SumBucketPrefix.AUTHENTICATION}.{metric.SystemType}.{metric.AuthenticationMethod}", Guid.Empty, metric.CorrelationID, MetricTypes.Counter, 1)
				}
		End Function

		Private Shared Function FormatSumBucketName(ByVal ParamArray parts() As Object) As String
			Return String.Join(".", parts)
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