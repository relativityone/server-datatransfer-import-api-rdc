Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.Tools
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS.Monitoring
	Public Class JobLiveMetricSink
		Inherits MetricSinkBase
		Implements IMessageSink(Of TransferJobApmThroughputMessage)

		Public Sub New(serviceFactory As IServiceFactory, metricsManagerFactory As IMetricsManagerFactory)
			MyBase.New(serviceFactory, metricsManagerFactory)
		End Sub

		Public Sub OnMessage(message As TransferJobApmThroughputMessage) Implements IMessageSink(Of TransferJobApmThroughputMessage).OnMessage
			LogApmDouble($"{PerformancePrefix}.Progress", 1, message)
		End Sub
	End Class
End Namespace