Imports System.Collections.Generic
Imports Relativity.DataExchange.Service
Imports Relativity.Telemetry.DataContracts.Shared
Imports Relativity.Telemetry.Services.Metrics

Namespace Monitoring.Sinks
    Public Class MetricSinkSum
        Implements IMetricSink

	    Private ReadOnly _keplerProxy As IKeplerProxy
        Private ReadOnly _sumMetricFormatter As ISumMetricFormatter

        Public Sub New (keplerProxy As IKeplerProxy, sumMetricFormatter As ISumMetricFormatter, isEnabled As Boolean)
	        _keplerProxy = keplerProxy
            _sumMetricFormatter = sumMetricFormatter
            Me.IsEnabled = isEnabled
        End Sub

        ''' <inheritdoc/>
        Public Sub Log(metric As MetricBase) Implements IMetricSink.Log
            Dim metrics As List(Of MetricRef) = _sumMetricFormatter.GenerateSumMetrics(metric)
            If metrics Is Nothing OrElse metrics.Count = 0 Then Return
	        _keplerProxy.ExecuteAsync(Async Function(serviceFactory As IServiceProxyFactory)
		        Using proxy As IMetricsManager = serviceFactory.CreateProxyInstance(Of IMetricsManager)()
			        Await proxy.LogMetricsAsync(metrics).ConfigureAwait(False)
		        End Using
	        End Function).GetAwaiter().GetResult()
        End Sub

        ''' <inheritdoc/>
        Public Property IsEnabled As Boolean Implements IMetricSink.IsEnabled
    End Class
End NameSpace