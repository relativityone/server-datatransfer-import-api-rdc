Imports Relativity.Services.ServiceProxy
Imports Relativity.Telemetry.Services.Interface
Imports Relativity.Telemetry.Services.Metrics

Namespace Monitoring.Sinks
    Public Class MetricSinkRelativityLogging
        Implements IMetricSink

        Public Sub New (isEnabled As Boolean)
            Me.IsEnabled = isEnabled
        End Sub

        ''' <inheritdoc/>
        Sub Log(metric As MetricBase) Implements IMetricSink.Log
            ' For now log as warning because information is not logged from RDC to Splunk
            ' For now only log MetricJobStarted
	        Dim metricJobStarted As MetricJobStarted = TryCast(metric, MetricJobStarted)
            If Not metricJobStarted Is Nothing Then
                Relativity.Logging.Log.Logger.LogWarning("Relativity.DataExchange metric. Bucket: {bucketName}, value: {@Metrics}", metricJobStarted.BucketName, metricJobStarted.CustomData)
            End If
        End Sub

        Public Property IsEnabled As Boolean Implements IMetricSink.IsEnabled
    End Class
End NameSpace