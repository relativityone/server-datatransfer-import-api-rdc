Imports System.Threading.Tasks
Imports kCura.WinEDDS.Monitoring

Namespace Relativity.Desktop.Client

	Public Class MetricsSinkConfigProvider
		Private Shared ReadOnly _CONFIG_REFRESH_TIME As TimeSpan = TimeSpan.FromMinutes(1)
		Private _lastRefresh As DateTime = DateTime.MinValue
		Private _currentConfig As IMetricsSinkConfig
		Private _configRefreshTask As Task

		Public ReadOnly Property CurrentConfig As IMetricsSinkConfig
			Get
				If DateTime.Now - _lastRefresh > _CONFIG_REFRESH_TIME Then
					ToggleRefresh()
					_lastRefresh = DateTime.Now
				End If
				Return _currentConfig
			End Get
		End Property

		Private Sub ToggleRefresh()
			If _configRefreshTask Is Nothing OrElse _configRefreshTask.IsCanceled OrElse _configRefreshTask.IsCompleted OrElse _configRefreshTask.IsFaulted Then
				_configRefreshTask = Task.Run(New Action(AddressOf RefreshConfiguration))
			End If
		End Sub

		Private Sub RefreshConfiguration()
			Dim config = New MetricsSinkConfig With {
				.ThrottleTimeout = TimeSpan.FromSeconds(Relativity.Desktop.Client.Config.RdcMetricsThrottlingSeconds),
				.SendLiveAPMMetrics = Relativity.Desktop.Client.Config.SendLiveApmMetrics,
				.SendSumMetrics = Relativity.Desktop.Client.Config.SendSumMetrics,
				.SendSummaryApmMetrics = Relativity.Desktop.Client.Config.SendSummaryApmMetrics
			}
			_currentConfig = config
		End Sub

		Public Sub Initialize()
			ToggleRefresh()
			_configRefreshTask.Wait()
		End Sub
	End Class
End Namespace