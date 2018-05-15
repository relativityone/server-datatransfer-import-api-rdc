
Imports Relativity.DataTransfer.MessageService
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS.Monitoring
	Public Class MessageServiceFactory

		Private Shared ReadOnly UsagePrefix As String = "RDC.Usage"

		Public Shared Function SetupMessageService(serviceFactory As IServiceFactory) As IMessageService

			Dim messageService As New MessageService()
			Dim messageManagerFactory As New MetricsManagerFactory()

			messageService.Subscribe(Of TransferJobStartedMessage)(
				Sub(message)
					Dim keplerManager As IMetricsManager = messageManagerFactory.CreateSUMKeplerManager(serviceFactory)
					keplerManager.LogCount($"{UsagePrefix}.JobStartedCount.{message.JobType}.{message.TransferMode}", 1)
				End Sub)

			messageService.Subscribe(Of TransferJobCompletedMessage)(
				Sub(message)
					Dim keplerManager As IMetricsManager = messageManagerFactory.CreateSUMKeplerManager(serviceFactory)
					keplerManager.LogCount($"{UsagePrefix}.JobCompletedCount.{message.JobType}.{message.TransferMode}", 1)
				End Sub)

			messageService.Subscribe(Of TransferJobFailedMessage)(
				Sub(message)
					Dim keplerManager As IMetricsManager = messageManagerFactory.CreateSUMKeplerManager(serviceFactory)
					keplerManager.LogCount($"{UsagePrefix}.JobFailedCount.{message.JobType}.{message.TransferMode}", 1)
				End Sub)

			messageService.Subscribe(Of TransferJobThroughputMessage)(
				Sub(message)
					Dim keplerManager As IMetricsManager = messageManagerFactory.CreateSUMKeplerManager(serviceFactory)
					keplerManager.LogDouble($"{UsagePrefix}.Throughput.{message.JobType}.{message.TransferMode}", message.RecordsPerSecond)
				End Sub)

			messageService.Subscribe(Of TransferJobTotalRecordsCountMessage)(
				Sub(message)
					Dim keplerManager As IMetricsManager = messageManagerFactory.CreateSUMKeplerManager(serviceFactory)
					keplerManager.LogDouble($"{UsagePrefix}.TotalRecordsCount.{message.JobType}.{message.TransferMode}", message.TotalRecords)
				End Sub)

			messageService.Subscribe(Of TransferJobCompletedRecordsCountMessage)(
				Sub(message)
					Dim keplerManager As IMetricsManager = messageManagerFactory.CreateSUMKeplerManager(serviceFactory)
					keplerManager.LogDouble($"{UsagePrefix}.CompletedRecordsCount.{message.JobType}.{message.TransferMode}", message.CompletedRecords)
				End Sub)

			Return messageService
		End Function
	End Class
End Namespace