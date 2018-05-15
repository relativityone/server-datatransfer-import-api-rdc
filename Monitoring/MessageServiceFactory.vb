
Imports Relativity.DataTransfer.MessageService
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS.Monitoring
	Public Class MessageServiceFactory

		Private Shared ReadOnly UsagePrefix As String = "RDC.Usage"
		Private Shared ReadOnly MessageService As New MessageService()
		Private Shared ReadOnly MessageManagerFactory As New MetricsManagerFactory()

		Public Shared Function SetupMessageService(serviceFactory As IServiceFactory) As IMessageService

			MessageService.Subscribe(Of TransferJobStartedMessage)(
				Sub(message)
					LogCount(serviceFactory, FormatBucketName("JobStartedCount", message.JobType, message.TransferMode), 1)
				End Sub)

			MessageService.Subscribe(Of TransferJobCompletedMessage)(
				Sub(message)
					LogCount(serviceFactory, FormatBucketName("JobCompletedCount", message.JobType, message.TransferMode), 1)
				End Sub)

			MessageService.Subscribe(Of TransferJobFailedMessage)(
				Sub(message)
					LogCount(serviceFactory, FormatBucketName("JobFailedCount", message.JobType, message.TransferMode), 1)
				End Sub)

			MessageService.Subscribe(Of TransferJobThroughputMessage)(
				Sub(message)
					LogDouble(serviceFactory, FormatBucketName("Throughput", message.JobType, message.TransferMode), message.RecordsPerSecond)
				End Sub)

			MessageService.Subscribe(Of TransferJobTotalRecordsCountMessage)(
				Sub(message)
					LogDouble(serviceFactory, FormatBucketName("TotalRecordsCount", message.JobType, message.TransferMode), message.TotalRecords)
				End Sub)

			MessageService.Subscribe(Of TransferJobCompletedRecordsCountMessage)(
				Sub(message)
					LogDouble(serviceFactory, FormatBucketName("CompletedRecordsCount", message.JobType, message.TransferMode), message.CompletedRecords)
				End Sub)

			Return messageService
		End Function

		Private Shared Function FormatBucketName(metricName As String, jobType As String, transferMode As String) As String
			Return $"{UsagePrefix}.{metricName}.{jobType}.{transferMode}"
		End Function

		Private Shared Sub LogCount(serviceFactory As IServiceFactory, bucketName As String, value As Long)
			Dim keplerManager As IMetricsManager = MessageManagerFactory.CreateSUMKeplerManager(serviceFactory)
			keplerManager.LogCount(bucketName, value)
		End Sub

		Private Shared Sub LogDouble(serviceFactory As IServiceFactory, bucketName As String, value As Double)
			Dim keplerManager As IMetricsManager = MessageManagerFactory.CreateSUMKeplerManager(serviceFactory)
			keplerManager.LogDouble(bucketName, value)
		End Sub

	End Class
End Namespace