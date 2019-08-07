Imports System.Diagnostics
Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataExchange

Public Class IApiMetricSinkConfig
    Implements IMetricsSinkConfig

    ''' <inheritdoc/>
    Public ReadOnly Property ThrottleTimeout As TimeSpan Implements IMetricsSinkConfig.ThrottleTimeout
        Get
            Return TimeSpan.FromSeconds(AppSettings.Instance.IapiMetricsThrottlingSeconds)
        End Get
    End Property

    ''' <inheritdoc/>
    Public ReadOnly Property SendLiveApmMetrics As Boolean Implements IMetricsSinkConfig.SendLiveApmMetrics
        Get
            Return  AppSettings.Instance.IapiSubmitApmMetrics
        End Get
    End Property

    ''' <inheritdoc/>
    Public ReadOnly Property SendSumMetrics As Boolean Implements IMetricsSinkConfig.SendSumMetrics
        Get
            Return AppSettings.Instance.IapiSubmitSumMetrics
        End Get
    End Property

    ''' <inheritdoc/>
    Public ReadOnly Property SendSummaryApmMetrics As Boolean Implements IMetricsSinkConfig.SendSummaryApmMetrics
        Get
            Return AppSettings.Instance.IapiSubmitApmMetrics
        End Get
    End Property
End Class