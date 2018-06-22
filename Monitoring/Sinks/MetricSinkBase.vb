Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS
	Public MustInherit Class MetricSinkBase
		Private ReadOnly _serviceFactory As IServiceFactory

		Private ReadOnly _usagePrefix As String = "RDC.Usage"
		Protected ReadOnly PerformancePrefix As String = "RDC.Performance"
		Private ReadOnly _messageManagerFactory As New MetricsManagerFactory()

		Protected Sub New (serviceFactory As IServiceFactory)
			_serviceFactory = serviceFactory
		End Sub

		Public MustOverride Sub Subscribe(messageService As IMessageService)

		Protected Function FormatUsageBucketName(metricName As String, jobType As String, transferMode As String) As String
			Return $"{_usagePrefix}.{metricName}.{jobType}.{transferMode}"
		End Function

		Protected Function FormatPerformanceBucketName(metricName As String, jobType As String, transferMode As String) As String
			Return $"{PerformancePrefix}.{metricName}.{jobType}.{transferMode}"
		End Function

		Protected Sub LogCount(bucketName As String, value As Long)
			Dim keplerManager As IMetricsManager = _messageManagerFactory.CreateSUMKeplerManager(_serviceFactory)
			keplerManager.LogCount(bucketName, value)
		End Sub

		Protected Sub LogDouble(bucketName As String, value As Double)
			Dim keplerManager As IMetricsManager = _messageManagerFactory.CreateSUMKeplerManager(_serviceFactory)
			keplerManager.LogDouble(bucketName, value)
		End Sub

		Protected Sub LogApmDouble(bucketName As String, value As Double, metadata As IMetricMetadata)
			Dim keplerManager As IMetricsManager = _messageManagerFactory.CreateAPMKeplerManager(_serviceFactory)
			keplerManager.LogDouble(bucketName, value, metadata)
		End Sub

	End Class
End Namespace