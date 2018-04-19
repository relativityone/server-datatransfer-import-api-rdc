Imports System.Diagnostics.Eventing.Reader
Imports System.Threading.Tasks
Imports kCura.Windows.Process
Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService

Public MustInherit Class MonitoredProcessBase
	Inherits kCura.Windows.Process.ProcessBase

	Protected Property StartTime As System.DateTime
	Protected Property EndTime As System.DateTime
	Protected Property TotalRecords As Long
	Protected Property InitialTapiClientName As String
	Protected MustOverride ReadOnly Property JobType As String
	Protected ReadOnly Property MessageService As IMessageService
	Protected _hasFatalErrorOccured As Boolean

	Public Sub New(messageService As IMessageService)
		Me.MessageService = messageService
	End Sub

	Protected Overrides Sub Execute()
		Initialize()
		If Run() Then
			If HasErrors() Then
				OnHasErrors()
			Else
				OnSuccess()
			End If
		Else
			OnFatalError()
		End If

	End Sub

	Protected Overridable Sub OnFatalError()
		EndTime = System.DateTime.Now
		Me.ProcessObserver.RaiseStatusEvent("", $"{JobType} aborted")
	End Sub

	Protected Overridable Sub OnSuccess()
		EndTime = System.DateTime.Now
	End Sub

	Protected Overridable Sub OnHasErrors()
		EndTime = System.DateTime.Now
	End Sub

	Protected MustOverride Function HasErrors() As Boolean

	Protected MustOverride Sub Initialize()

	Protected MustOverride Function Run() As Boolean

	Protected Sub SendTransferJobStartedMessage(tapiClientName As String)

		If InitialTapiClientName Is Nothing Then
			MessageService.Send(New TransferJobStartedMessage With {.JobType = JobType, .TransferMode = tapiClientName})
			InitialTapiClientName = tapiClientName
		End If
	End Sub

	Protected Sub SendTransferJobFailedMessage(tapiClientName As String)
		MessageService.Send(New TransferJobFailedMessage With {.JobType = JobType, .TransferMode = tapiClientName})
	End Sub

	Protected Sub SendTransferJobCompletedMessage(tapiClientName As String)
		MessageService.Send(New TransferJobCompletedMessage With {.JobType = JobType, .TransferMode = tapiClientName})
	End Sub

	Protected Sub SendThroughputMessage(tapiClientName As String)
		Dim duration As System.TimeSpan = EndTime - StartTime
		Dim recordsPerSecond As Double = TotalRecords / duration.TotalSeconds
		MessageService.Send(New TransferJobThroughputMessage With {.JobType = JobType, .TransferMode = tapiClientName, .RecordsPerSecond = recordsPerSecond})
	End Sub

End Class
