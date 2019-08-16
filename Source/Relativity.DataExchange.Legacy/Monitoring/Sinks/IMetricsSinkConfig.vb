

Namespace kCura.WinEDDS.Monitoring
	Public interface IMetricsSinkConfig
		Property ThrottleTimeout() As TimeSpan
		Property SendLiveApmMetrics As Boolean
		Property SendSumMetrics() As Boolean
		Property SendSummaryApmMetrics() As Boolean
	end Interface
End NameSpace