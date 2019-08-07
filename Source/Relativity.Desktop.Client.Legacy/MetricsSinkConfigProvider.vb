Imports System.Threading
Imports System.Threading.Tasks
Imports kCura.WinEDDS.Monitoring

Namespace Relativity.Desktop.Client

	Public Class MetricsSinkConfigProvider
	    Implements IMetricsSinkConfig
		Private Shared ReadOnly _CONFIG_REFRESH_TIME As TimeSpan = TimeSpan.FromMinutes(1)
		Private _lastRefresh As DateTime = DateTime.MinValue
		Private _configRefreshTask As Task

        Private _throttleTimeout As TimeSpan = New TimeSpan()
        Private _sendLiveApmMetrics As Boolean = New Boolean()
        Private _sendSumMetrics As Boolean = new Boolean()
        Private _sendSummaryApmMetrics As Boolean = New Boolean()
        
        Public ReadOnly Property ThrottleTimeout As TimeSpan Implements IMetricsSinkConfig.ThrottleTimeout
            Get
                ToggleRefresh()
                Return _throttleTimeout
            End Get
        End Property

        Public ReadOnly Property SendLiveApmMetrics As Boolean Implements IMetricsSinkConfig.SendLiveApmMetrics
            Get
                ToggleRefresh()
                Return _sendLiveApmMetrics
            End Get
        End Property

        Public ReadOnly Property SendSumMetrics As Boolean Implements IMetricsSinkConfig.SendSumMetrics
            Get
                ToggleRefresh()
                Return _sendSumMetrics
            End Get
        End Property

        Public ReadOnly Property SendSummaryApmMetrics As Boolean Implements IMetricsSinkConfig.SendSummaryApmMetrics
            Get
                ToggleRefresh()
                Return _sendSummaryApmMetrics
            End Get
        End Property

		Private Sub RefreshConfiguration()
		    _throttleTimeout = TimeSpan.FromSeconds(Relativity.Desktop.Client.Config.RdcMetricsThrottlingSeconds)
		    _sendLiveApmMetrics = Relativity.Desktop.Client.Config.SendLiveApmMetrics
		    _sendSumMetrics = Relativity.Desktop.Client.Config.SendSumMetrics
		    _sendSummaryApmMetrics = Relativity.Desktop.Client.Config.SendSummaryApmMetrics
		End Sub

        Private Sub ToggleRefresh()
            If DateTime.Now - _lastRefresh > _CONFIG_REFRESH_TIME Then
                If _configRefreshTask Is Nothing OrElse _configRefreshTask.IsCanceled OrElse _configRefreshTask.IsCompleted OrElse _configRefreshTask.IsFaulted Then
                    _configRefreshTask = Task.Run(New Action(AddressOf RefreshConfiguration))
                End If
                _lastRefresh = DateTime.Now
            End If
        End Sub

		Public Sub Initialize()
			ToggleRefresh()
			_configRefreshTask.Wait()
		End Sub
	End Class
End Namespace