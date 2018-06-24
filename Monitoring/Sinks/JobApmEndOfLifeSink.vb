Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.Tools
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS.Monitoring
	Public Class JobApmEndOfLifeSink
		Inherits MetricSinkBase
		Implements IMessageSink(Of TransferJobStatisticsMessage)

		Public Sub New(serviceFactory As IServiceFactory, metricsManagerFactory As IMetricsManagerFactory)
			MyBase.New(serviceFactory, metricsManagerFactory)
		End Sub

		Public Sub OnMessage(message As TransferJobStatisticsMessage) Implements IMessageSink(Of TransferJobStatisticsMessage).OnMessage
			LogApmDouble($"{PerformancePrefix}.JobStatistics", message.JobSizeInBytes, message)
		End Sub
	End Class
End Namespace