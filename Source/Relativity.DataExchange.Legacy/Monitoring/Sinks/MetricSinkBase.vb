Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM
Imports Relativity.Services.ServiceProxy

Namespace kCura.WinEDDS.Monitoring
	Public MustInherit Class MetricSinkBase
		Private ReadOnly _serviceFactory As IServiceFactory

		Protected ReadOnly UsagePrefix As String = "RDC.Usage"
		Protected ReadOnly PerformancePrefix As String = "RDC.Performance"
		Private ReadOnly _metricsManagerFactory As IMetricsManagerFactory

		Protected Sub New(serviceFactory As IServiceFactory, metricsManagerFactory As IMetricsManagerFactory)
			_serviceFactory = serviceFactory
			_metricsManagerFactory = metricsManagerFactory
		End Sub

        Private Function FormatApplicationName(applicationName As String) As String
            Return applicationName.Replace(" ", "")
        End Function

		Protected Function FormatUsageBucketName(metricName As String, jobType As String, transferMode As String, applicationName As String) As String
			Return $"{UsagePrefix}.{metricName}.{jobType}.{transferMode}.{FormatApplicationName(applicationName)}"
		End Function

		Protected Function FormatPerformanceBucketName(metricName As String, jobType As String, transferMode As String, applicationName As String) As String
			Return $"{PerformancePrefix}.{metricName}.{jobType}.{transferMode}.{FormatApplicationName(applicationName)}"
		End Function

		Public Sub LogCount(bucketName As String, value As Long, metadata As IMetricMetadata)
			Dim keplerManager As IMetricsManager = _metricsManagerFactory.CreateSUMKeplerManager(_serviceFactory)
			keplerManager.LogCount(bucketName, value, metadata)
		End Sub

		Public Sub LogDouble(bucketName As String, value As Double, metadata As IMetricMetadata)
			Dim keplerManager As IMetricsManager = _metricsManagerFactory.CreateSUMKeplerManager(_serviceFactory)
			keplerManager.LogDouble(bucketName, value, metadata)
		End Sub

		Public Sub LogApmDouble(bucketName As String, value As Double, metadata As IMetricMetadata)
			Dim keplerManager As IMetricsManager = _metricsManagerFactory.CreateAPMKeplerManager(_serviceFactory)
			keplerManager.LogDouble(bucketName, value, metadata)
		End Sub
	End Class
End Namespace