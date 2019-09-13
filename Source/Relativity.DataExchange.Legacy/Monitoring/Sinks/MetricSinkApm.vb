Imports Relativity.Services.ServiceProxy
Imports Relativity.Telemetry.Services.Interface
Imports Relativity.Telemetry.Services.Metrics

Namespace Monitoring.Sinks
    Public Class MetricSinkApm
        Implements IMetricSink

        Private ReadOnly _serviceFactory As IServiceFactory

        Public Sub New (serviceFactory As IServiceFactory, isEnabled As Boolean)
            _serviceFactory = serviceFactory
            Me.IsEnabled = isEnabled
        End Sub

        ''' <inheritdoc/>
        Sub Log(metric As MetricBase) Implements IMetricSink.Log
            Using proxy As IAPMManager = _serviceFactory.CreateProxy(Of IAPMManager)()
                Dim apmMetric As APMMetric = New APMMetric() With{.Name = metric.BucketName, .CustomData = metric.CustomData}
                proxy.LogCountAsync(apmMetric, 0).ConfigureAwait(False).GetAwaiter().GetResult()
            End Using
        End Sub

        Public Property IsEnabled As Boolean Implements IMetricSink.IsEnabled
    End Class
End NameSpace