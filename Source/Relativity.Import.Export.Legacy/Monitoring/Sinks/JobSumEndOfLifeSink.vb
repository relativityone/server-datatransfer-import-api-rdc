Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.Tools
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS.Monitoring
	Public Class JobSumEndOfLifeSink
		Inherits MetricSinkBase
		Implements IMessageSink(Of TransferJobThroughputMessage)
		Implements IMessageSink(Of TransferJobTotalRecordsCountMessage)
		Implements IMessageSink(Of TransferJobCompletedRecordsCountMessage)
		Implements IMessageSink(Of TransferJobStatisticsMessage)

		Public Sub New(serviceFactory As IServiceFactory, metricsManagerFactory As IMetricsManagerFactory)
			MyBase.New(serviceFactory, metricsManagerFactory)
		End Sub

		Public Sub OnMessage(message As TransferJobThroughputMessage) Implements IMessageSink(Of TransferJobThroughputMessage).OnMessage
			LogDouble(FormatPerformanceBucketName("Throughput", message.JobType, message.TransferMode), message.RecordsPerSecond, message)
			LogDouble(FormatPerformanceBucketName("ThroughputBytes", message.JobType, message.TransferMode), message.BytesPerSecond, message)
		End Sub

		Public Sub OnMessage(message As TransferJobTotalRecordsCountMessage) Implements IMessageSink(Of TransferJobTotalRecordsCountMessage).OnMessage
			LogDouble(FormatUsageBucketName("TotalRecords", message.JobType, message.TransferMode), message.TotalRecords, message)
		End Sub

		Public Sub OnMessage(message As TransferJobCompletedRecordsCountMessage) Implements IMessageSink(Of TransferJobCompletedRecordsCountMessage).OnMessage
			LogDouble(FormatUsageBucketName("CompletedRecords", message.JobType, message.TransferMode), message.CompletedRecords, message)
		End Sub

		Public Sub OnMessage(message As TransferJobStatisticsMessage) Implements IMessageSink(Of TransferJobStatisticsMessage).OnMessage
			LogDouble(FormatPerformanceBucketName("JobSize", message.JobType, message.TransferMode), message.JobSizeInBytes, message)
		End Sub
	End Class
End Namespace