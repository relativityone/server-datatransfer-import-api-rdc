
Imports Relativity.Services.ServiceProxy

Namespace Monitoring.Sinks

    Public Class MetricService
        Implements IMetricService

        Private ReadOnly _apmSink As MetricSinkApm

        Private _metricSinkConfig As IMetricSinkConfig
        
        ''' <summary>
        ''' Create sinks that does not require <see cref="IServiceFactory"/> and setup their configuration.
        ''' </summary>
        ''' <param name="metricSinkConfig">Sinks configuration.</param>
        Public Sub New(metricSinkConfig As IMetricSinkConfig)
            _metricSinkConfig = metricSinkConfig
            _apmSink = Nothing
        End Sub

        ''' <summary>
        ''' Create all sinks and setup their configuration
        ''' </summary>
        ''' <param name="metricSinkConfig">Sinks configuration.</param>
        ''' <param name="serviceFactory">Used to create proxies of Relativity Services.</param>
        Public Sub New(metricSinkConfig As IMetricSinkConfig, serviceFactory As IServiceFactory)
            _metricSinkConfig = metricSinkConfig
            _apmSink = New MetricSinkApm(serviceFactory, _metricSinkConfig.SendApmMetrics)
        End Sub

        ''' <inheritdoc/>
        Public Sub Log(metric As MetricBase) Implements IMetricService.Log
            If Not _apmSink Is Nothing AndAlso _apmSink.IsEnabled Then _apmSink.Log(metric)
        End Sub

        ''' <inheritdoc/>
        Public Property MetricSinkConfig As IMetricSinkConfig Implements IMetricService.MetricSinkConfig
            Get
                Return _metricSinkConfig
            End Get
            Set
                _metricSinkConfig = Value
                UpdateSinksConfiguration()
            End Set
        End Property

        ''' <summary>
        ''' Update <see cref="IMetricSink.IsEnabled"/> property of every sink
        ''' </summary>
        Private Sub UpdateSinksConfiguration()
            If Not _apmSink Is Nothing Then _apmSink.IsEnabled = _metricSinkConfig.SendApmMetrics
        End Sub
    End Class
End NameSpace