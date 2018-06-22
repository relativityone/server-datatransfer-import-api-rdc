Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.Tools
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS.Monitoring
	Public Class JobSumEndOfLifeSink
		Inherits MetricSinkBase
		Implements IMessageSink(Of TransferJobThroughputMessage)
		Implements IMessageSink(Of TransferJobTotalRecordsCountMessage)
		Implements IMessageSink(Of TransferJobCompletedRecordsCountMessage)

		Public Sub New(serviceFactory As IServiceFactory, metricsManagerFactory As IMetricsManagerFactory)
			MyBase.New(serviceFactory, metricsManagerFactory)
		End Sub

		Public Sub OnMessage(message As TransferJobThroughputMessage) Implements IMessageSink(Of TransferJobThroughputMessage).OnMessage
			LogDouble(FormatPerformanceBucketName("Throughput", message.JobType, message.TransferMode), message.RecordsPerSecond)
		End Sub

		Public Sub OnMessage(message As TransferJobTotalRecordsCountMessage) Implements IMessageSink(Of TransferJobTotalRecordsCountMessage).OnMessage
			LogDouble(FormatUsageBucketName("TotalRecords", message.JobType, message.TransferMode), message.TotalRecords)
		End Sub

		Public Sub OnMessage(message As TransferJobCompletedRecordsCountMessage) Implements IMessageSink(Of TransferJobCompletedRecordsCountMessage).OnMessage
			LogDouble(FormatUsageBucketName("CompletedRecords", message.JobType, message.TransferMode), message.CompletedRecords)
		End Sub
	End Class
End Namespace