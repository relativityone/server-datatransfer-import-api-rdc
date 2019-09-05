
Imports Relativity.Services.ServiceProxy

Namespace Monitoring.Sinks

    Public Class MetricService
        Implements IMetricService

        Private ReadOnly _apmSink As MetricSinkApm
        Private ReadOnly _sumSink As MetricSinkSum

        Private _metricSinkConfig As IMetricSinkConfig
        
        ''' <summary>
        ''' Create sinks that does not require <see cref="IServiceFactory"/> and setup their configuration.
        ''' </summary>
        ''' <param name="metricSinkConfig">Sinks configuration.</param>
        Public Sub New(metricSinkConfig As IMetricSinkConfig)
            _metricSinkConfig = metricSinkConfig
            _apmSink = Nothing
            _sumSink = Nothing
        End Sub

        ''' <summary>
        ''' Create all sinks and setup their configuration
        ''' </summary>
        ''' <param name="metricSinkConfig">Sinks configuration.</param>
        ''' <param name="serviceFactory">Used to create proxies of Relativity Services.</param>
        Public Sub New(metricSinkConfig As IMetricSinkConfig, serviceFactory As IServiceFactory)
            _metricSinkConfig = metricSinkConfig
            _apmSink = New MetricSinkApm(serviceFactory, _metricSinkConfig.SendApmMetrics)
            _sumSink = New MetricSinkSum(serviceFactory, _metricSinkConfig.SendSumMetrics)
        End Sub

        ''' <inheritdoc/>
        Public Sub Log(metric As MetricBase) Implements IMetricService.Log
            If Not _apmSink Is Nothing AndAlso _apmSink.IsEnabled Then _apmSink.Log(metric)
            If Not _sumSink Is Nothing AndAlso _sumSink.IsEnabled Then _sumSink.Log(metric)
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
            If Not _sumSink Is Nothing Then _sumSink.IsEnabled = _metricSinkConfig.SendSumMetrics
        End Sub
    End Class
End NameSpace