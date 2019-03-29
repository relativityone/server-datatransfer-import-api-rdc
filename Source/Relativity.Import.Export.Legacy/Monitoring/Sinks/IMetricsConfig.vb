

Namespace kCura.WinEDDS.Monitoring
	Public interface IMetricsSinkConfig
		ReadOnly Property ThrottleTimeout() As TimeSpan
		ReadOnly Property SendLiveApmMetrics As Boolean
		ReadOnly Property SendSumMetrics() As Boolean
		ReadOnly Property SendSummaryApmMetrics() As Boolean
	end Interface
End NameSpace