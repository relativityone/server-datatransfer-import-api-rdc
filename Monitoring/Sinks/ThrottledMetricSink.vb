Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM

Namespace kCura.WinEDDS.Monitoring
	Public Class ThrottledMetricSink(Of T As {Class, IMessage})
		Implements IMetricSink(Of T)

		Private ReadOnly _baseSink As IMetricSink(Of T)
		Private ReadOnly _throttleTimeoutProvider As Func(Of TimeSpan)
		Private _lastSendTime As DateTime = DateTime.MinValue

		Public Sub New(sink As IMetricSink(Of T), throttleTimeoutProvider As Func(Of TimeSpan))
			_baseSink = sink
			_throttleTimeoutProvider = throttleTimeoutProvider
		End Sub

		Public Sub OnMessage(message As T) Implements IMetricSink(Of T).OnMessage
			Dim currentTime As DateTime = DateTime.Now
			If currentTime - _lastSendTime >= _throttleTimeoutProvider() Then
				_baseSink.OnMessage(message)
				_lastSendTime = currentTime
			End If
		End Sub
	End Class
End Namespace