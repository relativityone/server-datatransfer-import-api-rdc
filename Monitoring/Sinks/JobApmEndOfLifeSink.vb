Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS
	Public Class JobApmEndOfLifeSink
		Inherits MetricSinkBase
		Implements IMetricSink(Of TransferJobSizeMessage)

		Public Sub New(serviceFactory As IServiceFactory, metricsManagerFactory As IMetricsManagerFactory)
			MyBase.New(serviceFactory, metricsManagerFactory)
		End Sub

		Public Sub OnMessage(message As TransferJobSizeMessage) Implements IMetricSink(Of TransferJobSizeMessage).OnMessage
			LogApmDouble(FormatPerformanceBucketName("JobSize", message.JobType, message.TransferMode), message.JobSize, message)
		End Sub
	End Class
End Namespace