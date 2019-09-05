Imports System.Collections.Generic
Imports Relativity.Services.DataContracts.DTOs.MetricsCollection
Imports Relativity.Telemetry.DataContracts.Shared

Namespace Monitoring
	Public Class MetricJobStarted
		Inherits MetricBase

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.METRIC_JOB_STARTED

        ''' <inheritdoc/>
        Public Overrides Function GenerateSumMetrics() As List(Of MetricRef)
            Return New List(Of MetricRef) From {
                New MetricRef(FormatSumBucketName(TelemetryConstants.SumBucketPrefix.JOB_STARTED_COUNT), Guid.Empty, CorrelationID, MetricTypes.Counter, 1)
            }
        End Function
	End Class
End Namespace