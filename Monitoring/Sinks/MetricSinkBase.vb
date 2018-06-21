Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS
	Public interface IMetricSink
		Sub LogCount(bucketName As String, value As Long)
		Sub LogDouble(bucketName As String, value As Double)
		Sub LogApmDouble(bucketName As String, value As Double, metadata As IMetricMetadata)
		Sub Subscribe(messageService As IMessageService)
	end interface

	Public MustInherit Class MetricSinkBase
		Implements IMetricSink
		Private ReadOnly _serviceFactory As IServiceFactory

		Protected ReadOnly UsagePrefix As String = "RDC.Usage"
		Protected ReadOnly PerformancePrefix As String = "RDC.Performance"
		Private ReadOnly _metricsManagerFactory As IMetricsManagerFactory

		Protected Sub New (serviceFactory As IServiceFactory, metricsManagerFactory As IMetricsManagerFactory)
			_serviceFactory = serviceFactory
			_metricsManagerFactory = metricsManagerFactory
		End Sub

		Public MustOverride Sub Subscribe(messageService As IMessageService) Implements IMetricSink.Subscribe

		Protected Function FormatUsageBucketName(metricName As String, jobType As String, transferMode As String) As String
			Return $"{UsagePrefix}.{metricName}.{jobType}.{transferMode}"
		End Function

		Protected Function FormatPerformanceBucketName(metricName As String, jobType As String, transferMode As String) As String
			Return $"{PerformancePrefix}.{metricName}.{jobType}.{transferMode}"
		End Function

		Public Sub LogCount(bucketName As String, value As Long) Implements IMetricSink.LogCount
			Dim keplerManager As IMetricsManager = _metricsManagerFactory.CreateSUMKeplerManager(_serviceFactory)
			keplerManager.LogCount(bucketName, value)
		End Sub

		Public Sub LogDouble(bucketName As String, value As Double) Implements IMetricSink.LogDouble
			Dim keplerManager As IMetricsManager = _metricsManagerFactory.CreateSUMKeplerManager(_serviceFactory)
			keplerManager.LogDouble(bucketName, value)
		End Sub

		Public Sub LogApmDouble(bucketName As String, value As Double, metadata As IMetricMetadata) Implements IMetricSink.LogApmDouble
			Dim keplerManager As IMetricsManager = _metricsManagerFactory.CreateAPMKeplerManager(_serviceFactory)
			keplerManager.LogDouble(bucketName, value, metadata)
		End Sub

	End Class
End Namespace