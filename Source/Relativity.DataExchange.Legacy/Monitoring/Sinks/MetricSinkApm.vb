Imports Relativity.DataExchange.Service
Imports Relativity.Telemetry.Services.Interface
Imports Relativity.Telemetry.Services.Metrics

Namespace Monitoring.Sinks
    Public Class MetricSinkApm
        Implements IMetricSink

        Private ReadOnly _keplerProxy As IKeplerProxy

        Public Sub New (keplerProxy As IKeplerProxy, isEnabled As Boolean)
	        _keplerProxy = keplerProxy
            Me.IsEnabled = isEnabled
        End Sub

        ''' <inheritdoc/>
        Sub Log(metric As MetricBase) Implements IMetricSink.Log
	        _keplerProxy.ExecuteAsync(Async Function(serviceFactory As IServiceProxyFactory)
		        Using proxy As IAPMManager = serviceFactory.CreateProxyInstance(Of IAPMManager)()
			        Dim apmMetric As APMMetric = New APMMetric() With{.Name = metric.BucketName, .CustomData = metric.CustomData}
			        Await proxy.LogCountAsync(apmMetric, 0).ConfigureAwait(False)
		        End Using
	        End Function).GetAwaiter().GetResult()
        End Sub

        Public Property IsEnabled As Boolean Implements IMetricSink.IsEnabled
    End Class
End NameSpace