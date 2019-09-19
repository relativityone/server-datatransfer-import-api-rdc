Imports Monitoring.Sinks

Namespace Relativity.Desktop.Client

	Public Class RdcMetricSinkConfig
	    Implements IMetricSinkConfig
        
        Public Sub New ()
            ThrottleTimeout = TimeSpan.FromSeconds(Config.RdcMetricsThrottlingSeconds)
            SendSumMetrics = Config.SendSumMetrics
            SendApmMetrics = Config.SendSummaryApmMetrics
        End Sub
        
        ''' <inheritdoc/>
        Public Property ThrottleTimeout As TimeSpan Implements IMetricSinkConfig.ThrottleTimeout

        ''' <inheritdoc/>
        Public Property SendSumMetrics As Boolean Implements IMetricSinkConfig.SendSumMetrics

        ''' <inheritdoc/>
        Public Property SendApmMetrics As Boolean Implements IMetricSinkConfig.SendApmMetrics
	End Class
End Namespace