Namespace kCura.WinEDDS.NUnit.Monitoring.Sinks
	Public Class MetricsSinkConfig
		Implements IMetricsSinkConfig

		Private Shared _CONFIG_REFRESH_TIME As TimeSpan = TimeSpan.FromMinutes(1)
		Private _lastRefresh As DateTime = DateTime.MinValue
		Private _throttleTimeout As TimeSpan

		Private Sub RefreshIfNeeded()
			If DateTime.Now - _lastRefresh > _CONFIG_REFRESH_TIME Then
				_lastRefresh = DateTime.Now
				_throttleTimeout = TimeSpan.FromSeconds(EDDS.WinForm.Config.RdcMetricsThrottlingSeconds)
			End If
		End Sub

		Public ReadOnly Property ThrottleTimeout As TimeSpan Implements IMetricsSinkConfig.ThrottleTimeout
			Get
				RefreshIfNeeded()
				Return _throttleTimeout
			End Get
		End Property
	End Class
End Namespace