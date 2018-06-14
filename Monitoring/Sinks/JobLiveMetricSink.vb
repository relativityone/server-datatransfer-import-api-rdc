Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS
	Public Class JobLiveMetricSink
		Inherits MetricSinkBase

		Public Sub New(serviceFactory As IServiceFactory)
			MyBase.New(serviceFactory)
		End Sub
		
		Public Overrides Sub Subscribe(messageService As IMessageService)
			messageService.Subscribe(Of TransferJobApmThroughputMessage)(
				Sub(message)
					LogApmDouble(FormatPerformanceBucketName("ThroughputBytes", message.JobType, message.TransferMode), 1, message)
				End Sub)
		End Sub

	End Class
End Namespace