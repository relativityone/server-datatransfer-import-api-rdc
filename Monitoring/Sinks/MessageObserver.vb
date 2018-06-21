Imports kCura.WinEDDS
Imports Relativity.DataTransfer.MessageService

Namespace kCura.WinEDDS.Monitoring
	Public Class MessageObserver
		Private ReadOnly _messageService As IMessageService

		Public Sub New(messageService As IMessageService)
			_messageService = messageService
		End Sub

		Public Sub Add(Of T As {Class, IMessage})(sink As IMetricSink(Of T))
			_messageService.Subscribe(Of T)(Sub(message)
				sink.OnMessage(message)
			End Sub)
		End Sub
	End Class
End NameSpace