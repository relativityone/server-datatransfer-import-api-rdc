Imports System.Collections.Generic
Imports Relativity.Services.ServiceProxy
Imports Relativity.Telemetry.DataContracts.Shared
Imports Relativity.Telemetry.Services.Metrics

Namespace Monitoring.Sinks
    Public Class MetricSinkSum
        Implements IMetricSink

        Private ReadOnly _serviceFactory As IServiceFactory

        Public Sub New (serviceFactory As IServiceFactory, isEnabled As Boolean)
            _serviceFactory = serviceFactory
            Me.IsEnabled = isEnabled
        End Sub

        ''' <inheritdoc/>
        Public Sub Log(metric As MetricBase) Implements IMetricSink.Log
            Dim metrics As List(Of MetricRef) = metric.GenerateSumMetrics()
            If metrics Is Nothing OrElse metrics.Count = 0 Then Return
            Using proxy As IMetricsManager = _serviceFactory.CreateProxy(Of IMetricsManager)()
                proxy.LogMetricsAsync(metrics).ConfigureAwait(False).GetAwaiter().GetResult()
            End Using
        End Sub

        ''' <inheritdoc/>
        Public Property IsEnabled As Boolean Implements IMetricSink.IsEnabled
    End Class
End NameSpace