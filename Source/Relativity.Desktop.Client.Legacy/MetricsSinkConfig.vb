Imports kCura.WinEDDS.Monitoring

Namespace kCura.EDDS.WinForm
	Public Class MetricsSinkConfig
		Implements IMetricsSinkConfig
		Public Property ThrottleTimeout As TimeSpan Implements IMetricsSinkConfig.ThrottleTimeout
		Public Property SendLiveAPMMetrics As Boolean Implements IMetricsSinkConfig.SendLiveApmMetrics
		Public Property SendSumMetrics As Boolean Implements IMetricsSinkConfig.SendSumMetrics
		Public Property SendSummaryApmMetrics As Boolean Implements IMetricsSinkConfig.SendSummaryApmMetrics
	End Class
End NameSpace