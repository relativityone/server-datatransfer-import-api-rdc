Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.Tools
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS.Monitoring
	Public Class JobLifetimeSink
		Inherits MetricSinkBase
		Implements IMessageSink(Of TransferJobStartedMessage)
		Implements IMessageSink(Of TransferJobCompletedMessage)
		Implements IMessageSink(Of TransferJobFailedMessage)

		Public Sub New(serviceFactory As IServiceFactory, metricsManagerFactory As IMetricsManagerFactory)
			MyBase.New(serviceFactory, metricsManagerFactory)
		End Sub

		Public Sub OnMessage(message As TransferJobStartedMessage) Implements IMessageSink(Of TransferJobStartedMessage).OnMessage
			LogCount(FormatPerformanceBucketName("JobStartedCount", message.JobType, message.TransferMode), 1)
		End Sub

		Public Sub OnMessage(message As TransferJobCompletedMessage) Implements IMessageSink(Of TransferJobCompletedMessage).OnMessage
			LogCount(FormatPerformanceBucketName("JobCompletedCount", message.JobType, message.TransferMode), 1)
		End Sub

		Public Sub OnMessage(message As TransferJobFailedMessage) Implements IMessageSink(Of TransferJobFailedMessage).OnMessage
			LogCount(FormatPerformanceBucketName("JobFailedCount", message.JobType, message.TransferMode), 1)
		End Sub
	End Class
End Namespace