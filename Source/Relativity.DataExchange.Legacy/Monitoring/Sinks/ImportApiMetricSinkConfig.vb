
Imports Relativity.DataExchange

Namespace Monitoring.Sinks
    Public Class ImportApiMetricSinkConfig
        Implements IMetricSinkConfig

        Private _throttleTimeout As TimeSpan? = Nothing
        Private _sendSumMetrics As Boolean? = Nothing
        Private _sendApmMetrics As Boolean? = Nothing

        ''' <inheritdoc/>
        Public Property ThrottleTimeout As TimeSpan Implements IMetricSinkConfig.ThrottleTimeout
            Get
                If _throttleTimeout.HasValue Then
                    Return _throttleTimeout.Value
                End If
                Return TimeSpan.FromSeconds(AppSettings.Instance.TelemetryMetricsThrottlingSeconds)
            End Get
            Set
                _throttleTimeout = Value
            End Set
        End Property

        ''' <inheritdoc/>
        Public Property SendSumMetrics As Boolean Implements IMetricSinkConfig.SendSumMetrics
            Get
                If _sendSumMetrics.HasValue
                    Return _sendSumMetrics.Value
                End If
                Return AppSettings.Instance.TelemetrySubmitSumMetrics
            End Get
            Set
                _sendSumMetrics = Value
            End Set
        End Property

        ''' <inheritdoc/>
        Public Property SendApmMetrics As Boolean Implements IMetricSinkConfig.SendApmMetrics
            Get
                If _sendApmMetrics.HasValue
                    Return _sendApmMetrics.Value
                End If
                Return AppSettings.Instance.TelemetrySubmitApmMetrics
            End Get
            Set
                _sendApmMetrics = Value
            End Set
        End Property
    End Class
End NameSpace