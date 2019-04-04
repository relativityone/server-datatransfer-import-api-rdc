Namespace kCura.WinEDDS.NUnit.Monitoring.Sinks
	Public Class MetricsConfig
		Implements IMetricsConfig

		Private ReadOnly _lockObj As New Object
		Private Shared ReadOnly _CONFIG_REFRESH_TIME As TimeSpan = TimeSpan.FromMinutes(1)
		Private _lastRefresh As DateTime = DateTime.MinValue
		Private _throttleTimeout As TimeSpan
		Private _sendLiveApmMetrics As Boolean
		Private _sendSumMetrics As Boolean
		Private _sendSummaryApmMetrics As Boolean

		Private Sub RefreshIfNeeded()
			If DateTime.Now - _lastRefresh > _CONFIG_REFRESH_TIME Then
				SyncLock _lockObj
					If DateTime.Now - _lastRefresh > _CONFIG_REFRESH_TIME Then
						_lastRefresh = DateTime.Now
						_throttleTimeout = TimeSpan.FromSeconds(EDDS.WinForm.Config.RdcMetricsThrottlingSeconds)
						_sendLiveApmMetrics = WinEDDS.Config.SendLiveApmMetrics
						_sendSumMetrics = WinEDDS.Config.SendSumMetrics
						_sendSummaryApmMetrics = WinEDDS.Config.SendSummaryApmMetrics
					End If
				End SyncLock
			End If
		End Sub

		Public ReadOnly Property ThrottleTimeout As TimeSpan Implements IMetricsConfig.ThrottleTimeout
			Get
				RefreshIfNeeded()
				Return _throttleTimeout
			End Get
		End Property

		Public ReadOnly Property SendLiveAPMMetrics As Boolean Implements IMetricsSinkConfig.SendLiveAPMMetrics
			Get
				RefreshIfNeeded()
				Return _sendLiveApmMetrics
			End Get
		End Property
		Public ReadOnly Property SendSumMetrics As Boolean Implements IMetricsSinkConfig.SendSumMetrics
			Get
				RefreshIfNeeded()
				Return _sendSumMetrics
			End Get
		End Property
		Public ReadOnly Property SendSummaryApmMetrics As Boolean Implements IMetricsSinkConfig.SendSummaryApmMetrics
			Get
				RefreshIfNeeded()
				Return _sendSummaryApmMetrics
			End Get
		End Property
	End Class
End Namespace