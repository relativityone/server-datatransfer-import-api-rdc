Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS.Monitoring
	Public Class MessageServiceFactory

		Private Shared ReadOnly UsagePrefix As String = "RDC.Usage"
		Private Shared ReadOnly PerformancePrefix As String = "RDC.Performance"
		Private Shared ReadOnly MessageService As New MessageService()
		Private Shared ReadOnly MessageManagerFactory As New MetricsManagerFactory()

		Public Shared Function SetupMessageService(serviceFactory As IServiceFactory) As IMessageService

			MessageService.Subscribe(Of TransferJobStartedMessage)(
				Sub(message)
					LogCount(serviceFactory, FormatPerformanceBucketName("JobStartedCount", message.JobType, message.TransferMode), 1)
				End Sub)

			MessageService.Subscribe(Of TransferJobCompletedMessage)(
				Sub(message)
					LogCount(serviceFactory, FormatPerformanceBucketName("JobCompletedCount", message.JobType, message.TransferMode), 1)
				End Sub)

			MessageService.Subscribe(Of TransferJobFailedMessage)(
				Sub(message)
					LogCount(serviceFactory, FormatPerformanceBucketName("JobFailedCount", message.JobType, message.TransferMode), 1)
				End Sub)

			MessageService.Subscribe(Of TransferJobThroughputMessage)(
				Sub(message)
					LogDouble(serviceFactory, FormatPerformanceBucketName("Throughput", message.JobType, message.TransferMode), message.RecordsPerSecond)
				End Sub)

			MessageService.Subscribe(Of TransferJobTotalRecordsCountMessage)(
				Sub(message)
					LogDouble(serviceFactory, FormatUsageBucketName("TotalRecords", message.JobType, message.TransferMode), message.TotalRecords)
				End Sub)

			MessageService.Subscribe(Of TransferJobCompletedRecordsCountMessage)(
				Sub(message)
					LogDouble(serviceFactory, FormatUsageBucketName("CompletedRecords", message.JobType, message.TransferMode), message.CompletedRecords)
				End Sub)

			MessageService.Subscribe(Of TransferJobApmThroughputMessage)(
				Sub(message)
					LogApmDouble(serviceFactory, FormatPerformanceBucketName("ThroughputBytes", message.JobType, message.TransferMode), 1, message)
				End Sub)

			MessageService.Subscribe(Of TransferJobSizeMessage)(
				Sub(message)
					LogApmDouble(serviceFactory, FormatPerformanceBucketName("JobSize", message.JobType, message.TransferMode), message.JobSize, message)
				End Sub)

			Return MessageService
		End Function

		Private Shared Function FormatUsageBucketName(metricName As String, jobType As String, transferMode As String) As String
			Return $"{UsagePrefix}.{metricName}.{jobType}.{transferMode}"
		End Function

		Private Shared Function FormatPerformanceBucketName(metricName As String, jobType As String, transferMode As String) As String
			Return $"{PerformancePrefix}.{metricName}.{jobType}.{transferMode}"
		End Function

		Private Shared Sub LogCount(serviceFactory As IServiceFactory, bucketName As String, value As Long)
			Dim keplerManager As IMetricsManager = MessageManagerFactory.CreateSUMKeplerManager(serviceFactory)
			keplerManager.LogCount(bucketName, value)
		End Sub

		Private Shared Sub LogDouble(serviceFactory As IServiceFactory, bucketName As String, value As Double)
			Dim keplerManager As IMetricsManager = MessageManagerFactory.CreateSUMKeplerManager(serviceFactory)
			keplerManager.LogDouble(bucketName, value)
		End Sub

		Private Shared Sub LogApmDouble(serviceFactory As IServiceFactory, bucketName As String, value As Double, metadata As IMetricMetadata)
			Dim keplerManager As IMetricsManager = MessageManagerFactory.CreateAPMKeplerManager(serviceFactory)
			keplerManager.LogDouble(bucketName, value, metadata)
		End Sub

	End Class
End Namespace