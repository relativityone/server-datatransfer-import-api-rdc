Namespace Monitoring.Sinks
    Public Class MetricSinkRelativityLogging
        Implements IMetricSink

        Public Sub New(isEnabled As Boolean)
            Me.IsEnabled = isEnabled
        End Sub

        ''' <inheritdoc/>
        Sub Log(metric As MetricBase) Implements IMetricSink.Log
            ' For now log as warning because information is not logged from RDC to Splunk
            ' For now only log MetricJobStarted and MetricJobEndReport
            If TypeOf metric Is MetricJobStarted OrElse TypeOf metric Is MetricJobEndReport Then
                Relativity.Logging.Log.Logger.LogWarning("Relativity.DataExchange metric. Bucket: {bucketName}, value: {@Metrics}", metric.BucketName, metric.CustomData)
            End If
        End Sub

        Public Property IsEnabled As Boolean Implements IMetricSink.IsEnabled
    End Class
End Namespace