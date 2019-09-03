Imports kCura.WinEDDS.Monitoring

Namespace Monitoring.Sinks
    Public Interface IMetricService
        
        ''' <summary>
        ''' Sends metric to all enabled sinks.
        ''' </summary>
        ''' <param name="metric">Metric to send.</param>
        Sub Log(metric As MetricBase)

        ''' <summary>
        ''' Gets or sets sinks configuration
        ''' </summary>
        ''' <returns>Current sinks configuration</returns>
        Property MetricSinkConfig As IMetricsSinkConfig
    End Interface
End NameSpace