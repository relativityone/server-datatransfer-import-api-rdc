

Namespace kCura.WinEDDS
	Public interface IMetricsSinkConfig
		ReadOnly Property ThrottleTimeout() As TimeSpan
		ReadOnly Property SendLiveAPMMetrics As Boolean
		ReadOnly Property SendSumMetrics() As Boolean
		ReadOnly Property SendSummaryApmMetrics() As Boolean
	end Interface
End NameSpace