Imports kCura.WinEDDS.Monitoring

Public Class MetricSinkConfigIAPI
    Implements IMetricsSinkConfig

    Public ReadOnly Property ThrottleTimeout As TimeSpan Implements IMetricsSinkConfig.ThrottleTimeout
    Get
        Return TimeSpan.FromSeconds(30)
    End Get
    End Property
    Public ReadOnly Property SendLiveApmMetrics As Boolean Implements IMetricsSinkConfig.SendLiveApmMetrics
    Get
        Return True
    End Get
    End Property
    Public ReadOnly Property SendSumMetrics As Boolean Implements IMetricsSinkConfig.SendSumMetrics
    Get
        Return True
    End Get
    End Property
    Public ReadOnly Property SendSummaryApmMetrics As Boolean Implements IMetricsSinkConfig.SendSummaryApmMetrics
    Get
        Return True
    End Get
    End Property
End Class