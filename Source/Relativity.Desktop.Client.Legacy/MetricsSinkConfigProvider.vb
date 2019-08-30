﻿Imports System.Threading
Imports System.Threading.Tasks
Imports kCura.WinEDDS.Monitoring

Namespace Relativity.Desktop.Client

	Public Class MetricsSinkConfigProvider
	    Implements IMetricsSinkConfig
        
        Public Sub New ()
            ThrottleTimeout = TimeSpan.FromSeconds(Relativity.Desktop.Client.Config.RdcMetricsThrottlingSeconds)
            SendLiveApmMetrics = Relativity.Desktop.Client.Config.SendLiveApmMetrics
            SendSumMetrics = Relativity.Desktop.Client.Config.SendSumMetrics
            SendSummaryApmMetrics = Relativity.Desktop.Client.Config.SendSummaryApmMetrics
        End Sub
        
        ''' <inheritdoc/>
        Public Property ThrottleTimeout As TimeSpan Implements IMetricsSinkConfig.ThrottleTimeout

        ''' <inheritdoc/>
        Public Property SendLiveApmMetrics As Boolean Implements IMetricsSinkConfig.SendLiveApmMetrics

        ''' <inheritdoc/>
        Public Property SendSumMetrics As Boolean Implements IMetricsSinkConfig.SendSumMetrics

        ''' <inheritdoc/>
        Public Property SendSummaryApmMetrics As Boolean Implements IMetricsSinkConfig.SendSummaryApmMetrics
	End Class
End Namespace