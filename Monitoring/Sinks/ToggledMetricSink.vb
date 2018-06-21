Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM

Namespace kCura.WinEDDS.Monitoring
	Public Class ToggledMetricSink(Of T As {Class, IMessage})
		Implements IMetricSink(Of T)

		Private ReadOnly _baseSink As IMetricSink(Of T)
		Private ReadOnly _toggleProvider As Func(Of Boolean)

		Public Sub New(baseSink As IMetricSink(Of T), toggleProvider As Func(Of Boolean))
			_baseSink = baseSink
			_toggleProvider = toggleProvider
		End Sub

		Public Sub OnMessage(message As T) Implements IMetricSink(Of T).OnMessage
			If (_toggleProvider()) Then _baseSink.OnMessage(message)
		End Sub
	End Class
End Namespace