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

        Protected Overrides Sub Log(message As TransferJobMessageBase)
            Dim keplerManager As IMetricsManager = _metricsManagerFactory.CreateAPMKeplerManager(_serviceFactory)
            keplerManager.LogDouble(message.BucketName, 0, message)
        End Sub
    End Class
End NameSpace