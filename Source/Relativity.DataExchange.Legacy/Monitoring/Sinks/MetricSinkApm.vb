Imports Relativity.DataTransfer.MessageService
Imports Relativity.Services.ServiceProxy

Namespace Monitoring.Sinks
    Public Class MetricSinkApm
        Inherits MetricSinkBase

        Private ReadOnly _serviceFactory As IServiceFactory
        Private ReadOnly _metricsManagerFactory As IMetricsManagerFactory

        Public Sub New (serviceFactory As IServiceFactory, metricsManagerFactory As IMetricsManagerFactory)
            _serviceFactory = serviceFactory
            _metricsManagerFactory = metricsManagerFactory
        End Sub

        ''' <inheritdoc/>
        Protected Overrides Sub Log(metric As MetricBase)
            Dim keplerManager As IMetricsManager = _metricsManagerFactory.CreateAPMKeplerManager(_serviceFactory)
            keplerManager.LogDouble(metric.BucketName, 0, metric)
        End Sub
    End Class
End NameSpace