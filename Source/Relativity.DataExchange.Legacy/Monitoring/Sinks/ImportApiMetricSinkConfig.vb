Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataExchange

Namespace Monitoring.Sinks

    Public Class ImportApiMetricSinkConfig
        Implements IMetricsSinkConfig

        Private _throttleTimeout As Nullable(Of TimeSpan) = Nothing
        Private _sendLiveApmMetrics As Nullable(Of Boolean) = Nothing
        Private _sendSumMetrics As Nullable(Of Boolean) = Nothing
        Private _sendSummaryApmMetrics As Nullable(Of Boolean) = Nothing
'
        ''' <inheritdoc/>
        Public Property ThrottleTimeout As TimeSpan Implements IMetricsSinkConfig.ThrottleTimeout
            Get
                If _throttleTimeout.HasValue Then
                    Return _throttleTimeout.Value
                End If
                Return TimeSpan.FromSeconds(AppSettings.Instance.TelemetryMetricsThrottlingSeconds)
            End Get
            Set(value As TimeSpan)
                _throttleTimeout = value
            End Set
        End Property

        ''' <inheritdoc/>
        Public Property SendLiveApmMetrics As Boolean Implements IMetricsSinkConfig.SendLiveApmMetrics
            Get
                If _sendLiveApmMetrics.HasValue
                    Return _sendLiveApmMetrics.Value
                End If
                Return  AppSettings.Instance.TelemetrySubmitApmMetrics
            End Get
            Set(value As Boolean)
                _sendLiveApmMetrics = value
            End Set
        End Property

        ''' <inheritdoc/>
        Public Property SendSumMetrics As Boolean Implements IMetricsSinkConfig.SendSumMetrics
            Get
                If _sendSumMetrics.HasValue
                    Return _sendSumMetrics.Value
                End If
                Return AppSettings.Instance.TelemetrySubmitSumMetrics
            End Get
            Set(value As Boolean)
                _sendSumMetrics = value
            End Set
        End Property

        ''' <inheritdoc/>
        Public Property SendSummaryApmMetrics As Boolean Implements IMetricsSinkConfig.SendSummaryApmMetrics
            Get
                If _sendSummaryApmMetrics.HasValue
                    Return _sendSummaryApmMetrics.Value
                End If
                Return AppSettings.Instance.TelemetrySubmitApmMetrics
            End Get
            Set(value As Boolean)
                _sendSummaryApmMetrics = value
            End Set
        End Property
    End Class
End NameSpace