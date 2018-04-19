
Imports Relativity.DataTransfer.MessageService
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS.Monitoring
	Public Class MessageServiceFactory

		Private Shared ReadOnly PerformancePrefix As String = "RDC.Performance"

		Public Shared Function SetupMessageService(serviceFactory As IServiceFactory) As IMessageService

			Dim messageService As New MessageService()
			Dim messageManagerFactory As New MetricsManagerFactory()
			
			messageService.Subscribe(Of TransferJobStartedMessage)(
				Sub(message)
					Dim keplerManager As IMetricsManager = messageManagerFactory.CreateSUMKeplerManager(serviceFactory)
					keplerManager.LogCount($"{PerformancePrefix}.JobStartedCount.{message.JobType}.{message.TransferMode}", 1)
				End Sub)

			messageService.Subscribe(Of TransferJobCompletedMessage)(
				Sub(message)
					Dim keplerManager As IMetricsManager = messageManagerFactory.CreateSUMKeplerManager(serviceFactory)
					keplerManager.LogCount($"{PerformancePrefix}.JobCompletedCount.{message.JobType}.{message.TransferMode}", 1)
				End Sub)

			messageService.Subscribe(Of TransferJobFailedMessage)(
				Sub(message)
					Dim keplerManager As IMetricsManager = messageManagerFactory.CreateSUMKeplerManager(serviceFactory)
					keplerManager.LogCount($"{PerformancePrefix}.JobFailedCount.{message.JobType}.{message.TransferMode}", 1)
				End Sub)
			Return messageService
		End Function
	End Class
End Namespace