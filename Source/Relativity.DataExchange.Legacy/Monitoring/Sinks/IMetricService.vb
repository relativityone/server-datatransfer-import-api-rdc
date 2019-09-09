Namespace Monitoring.Sinks
    Public Interface IMetricService
        
        ''' <summary>
        ''' Sends metric to all enabled sinks.
        ''' </summary>
        ''' <param name="metric">Metric to send.</param>
        Sub Log(metric As MetricBase)

        ''' <summary>
        ''' Gets sinks configuration
        ''' </summary>
        ''' <returns>Current sinks configuration</returns>
        Readonly Property MetricSinkConfig As IMetricSinkConfig
    End Interface
End NameSpace