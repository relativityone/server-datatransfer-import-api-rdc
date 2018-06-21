Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS
	Public Class JobSumEndOfLifeSink
		Inherits MetricSinkBase
		Implements IMetricSink(Of TransferJobThroughputMessage)
		Implements IMetricSink(Of TransferJobTotalRecordsCountMessage)
		Implements IMetricSink(Of TransferJobCompletedRecordsCountMessage)

		Public Sub New(serviceFactory As IServiceFactory, metricsManagerFactory As IMetricsManagerFactory)
			MyBase.New(serviceFactory, metricsManagerFactory)
		End Sub

		Public Sub OnMessage(message As TransferJobThroughputMessage) Implements IMetricSink(Of TransferJobThroughputMessage).OnMessage
			LogDouble(FormatPerformanceBucketName("Throughput", message.JobType, message.TransferMode), message.RecordsPerSecond)
		End Sub

		Public Sub OnMessage(message As TransferJobTotalRecordsCountMessage) Implements IMetricSink(Of TransferJobTotalRecordsCountMessage).OnMessage
			LogDouble(FormatUsageBucketName("TotalRecords", message.JobType, message.TransferMode), message.TotalRecords)
		End Sub

		Public Sub OnMessage(message As TransferJobCompletedRecordsCountMessage) Implements IMetricSink(Of TransferJobCompletedRecordsCountMessage).OnMessage
			LogDouble(FormatUsageBucketName("CompletedRecords", message.JobType, message.TransferMode), message.CompletedRecords)
		End Sub
	End Class
End Namespace