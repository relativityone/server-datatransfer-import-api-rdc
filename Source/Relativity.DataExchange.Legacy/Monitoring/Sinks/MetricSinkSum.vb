Imports System.Collections.Generic
Imports Relativity.Services.ServiceProxy
Imports Relativity.Telemetry.DataContracts.Shared
Imports Relativity.Telemetry.Services.Metrics

Namespace Monitoring.Sinks
    Public Class MetricSinkSum
        Implements IMetricSink

        Private ReadOnly _serviceFactory As IServiceFactory
        Private ReadOnly _sumMetricFormatter As ISumMetricFormatter

        Public Sub New (serviceFactory As IServiceFactory, sumMetricFormatter As ISumMetricFormatter, isEnabled As Boolean)
            _serviceFactory = serviceFactory
            _sumMetricFormatter = sumMetricFormatter
            Me.IsEnabled = isEnabled
        End Sub

        ''' <inheritdoc/>
        Public Sub Log(metric As MetricBase) Implements IMetricSink.Log
            Dim metrics As List(Of MetricRef) = _sumMetricFormatter.GenerateSumMetrics(metric)
            If metrics Is Nothing OrElse metrics.Count = 0 Then Return
            Using proxy As IMetricsManager = _serviceFactory.CreateProxy(Of IMetricsManager)()
                proxy.LogMetricsAsync(metrics).ConfigureAwait(False).GetAwaiter().GetResult()
            End Using
        End Sub

        ''' <inheritdoc/>
        Public Property IsEnabled As Boolean Implements IMetricSink.IsEnabled
    End Class
End NameSpace