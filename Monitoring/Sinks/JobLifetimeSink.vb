Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS
	Public Class JobLifetimeSink
		Inherits MetricSinkBase

		Public Sub New(serviceFactory As IServiceFactory, metricsManagerFactory As IMetricsManagerFactory)
			MyBase.New(serviceFactory, metricsManagerFactory)
		End Sub

		Public Overrides Sub Subscribe(messageService As IMessageService)
			messageService.Subscribe(Of TransferJobStartedMessage)(
				Sub(message)
					LogCount(FormatPerformanceBucketName("JobStartedCount", message.JobType, message.TransferMode), 1)
				End Sub)

			messageService.Subscribe(Of TransferJobCompletedMessage)(
				Sub(message)
					LogCount(FormatPerformanceBucketName("JobCompletedCount", message.JobType, message.TransferMode), 1)
				End Sub)

			messageService.Subscribe(Of TransferJobFailedMessage)(
				Sub(message)
					LogCount(FormatPerformanceBucketName("JobFailedCount", message.JobType, message.TransferMode), 1)
				End Sub)
		End Sub
	End Class
End Namespace