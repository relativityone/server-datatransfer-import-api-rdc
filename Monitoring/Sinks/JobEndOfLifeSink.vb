Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS
	Public Class JobEndOfLifeSink
		Inherits MetricSinkBase

		Public Sub New(serviceFactory As IServiceFactory)
			MyBase.New(serviceFactory)
		End Sub

		Public Overrides Sub Subscribe(messageService As IMessageService)
			messageService.Subscribe(Of TransferJobThroughputMessage)(
				Sub(message)
					LogDouble(FormatPerformanceBucketName("Throughput", message.JobType, message.TransferMode), message.RecordsPerSecond)
				End Sub)

			messageService.Subscribe(Of TransferJobTotalRecordsCountMessage)(
				Sub(message)
					LogDouble(FormatUsageBucketName("TotalRecords", message.JobType, message.TransferMode), message.TotalRecords)
				End Sub)

			messageService.Subscribe(Of TransferJobCompletedRecordsCountMessage)(
				Sub(message)
					LogDouble(FormatUsageBucketName("CompletedRecords", message.JobType, message.TransferMode), message.CompletedRecords)
				End Sub)

			messageService.Subscribe(Of TransferJobSizeMessage)(
				Sub(message)
					LogApmDouble(FormatPerformanceBucketName("JobSize", message.JobType, message.TransferMode), message.JobSize, message)
				End Sub)

		End Sub
	End Class
End Namespace