Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS
	Public Class JobLifetimeSink
		Inherits MetricSinkBase
		Implements IMetricSink(Of TransferJobStartedMessage)
		Implements IMetricSink(Of TransferJobCompletedMessage)
		Implements IMetricSink(Of TransferJobFailedMessage)

		Public Sub New(serviceFactory As IServiceFactory, metricsManagerFactory As IMetricsManagerFactory)
			MyBase.New(serviceFactory, metricsManagerFactory)
		End Sub

		Public Sub OnMessage(message As TransferJobStartedMessage) Implements IMetricSink(Of TransferJobStartedMessage).OnMessage
			LogCount(FormatPerformanceBucketName("JobStartedCount", message.JobType, message.TransferMode), 1)
		End Sub

		Public Sub OnMessage(message As TransferJobCompletedMessage) Implements IMetricSink(Of TransferJobCompletedMessage).OnMessage
			LogCount(FormatPerformanceBucketName("JobCompletedCount", message.JobType, message.TransferMode), 1)
		End Sub

		Public Sub OnMessage(message As TransferJobFailedMessage) Implements IMetricSink(Of TransferJobFailedMessage).OnMessage
			LogCount(FormatPerformanceBucketName("JobFailedCount", message.JobType, message.TransferMode), 1)
		End Sub
	End Class
End Namespace