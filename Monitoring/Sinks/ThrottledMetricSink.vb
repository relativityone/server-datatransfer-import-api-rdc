Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM

Namespace kCura.WinEDDS
	Public Class ThrottledMetricSink
		Implements IMetricSink

		Private ReadOnly _jobLiveMetricSink As MetricSinkBase
		Private ReadOnly _func As Func(Of TimeSpan)

		Public Sub New(jobLiveMetricSink As MetricSinkBase, func As Func(Of TimeSpan))
			_jobLiveMetricSink = jobLiveMetricSink
			_func = func
		End Sub

		Public Sub LogCount(bucketName As String, value As Long) Implements IMetricSink.LogCount
			_jobLiveMetricSink.LogCount(bucketName, value)
		End Sub

		Public Sub LogDouble(bucketName As String, value As Double) Implements IMetricSink.LogDouble
			_jobLiveMetricSink.LogDouble(bucketName, value)
		End Sub

		Public Sub LogApmDouble(bucketName As String, value As Double, metadata As IMetricMetadata) Implements IMetricSink.LogApmDouble
			_jobLiveMetricSink.LogApmDouble(bucketName, value, metadata)
		End Sub

		Public Sub Subscribe(messageService As IMessageService) Implements IMetricSink.Subscribe
			_jobLiveMetricSink.Subscribe(messageService)
		End Sub
	End Class
End NameSpace