Imports System.Diagnostics.Eventing.Reader
Imports kCura.Windows.Process
Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService

Public MustInherit Class MonitoredProcessBase
	Inherits kCura.Windows.Process.ProcessBase

	Protected ReadOnly Property MessageService As IMessageService

	Public Sub New (messageService As IMessageService)
		Me.MessageService = messageService
	End Sub

	Protected Overrides Sub Execute()
		Initialize()
		If Run()
			If HasErrors()
				'_messageService.Send() -- failed job
				OnHasErrors()
			Else
				'_messageService.Send() -- success
				OnSuccess()
			End If
		Else
			OnFatalError()
		End If

	End Sub

	Protected Overridable Sub OnFatalError()
	End Sub

	Protected MustOverride Sub OnSuccess()

	Protected MustOverride Sub OnHasErrors()

	Protected MustOverride Function HasErrors() As Boolean

	Protected MustOverride Sub Initialize()

	Protected MustOverride Function Run() As Boolean
	
End Class
