Namespace Monitoring.Sinks
    Public Interface IMetricSink
        
        ''' <summary>
        ''' Send a given metric to the sink
        ''' </summary>
        ''' <param name="metric">Metric to send</param>
        Sub Log(metric As MetricBase)

        Property IsEnabled() As Boolean
    End Interface
End NameSpace