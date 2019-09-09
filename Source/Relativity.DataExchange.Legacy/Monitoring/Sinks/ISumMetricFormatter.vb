Imports System.Collections.Generic
Imports Relativity.Telemetry.DataContracts.Shared

Namespace Monitoring.Sinks
    Public interface ISumMetricFormatter

        ''' <summary>
        ''' Generates SUM metrics from <see cref="MetricBase"/>. This is because single <see cref="MetricBase"/> subclass may be split into multiple SUM metrics (SUM metrics can have only one property with the value).
        ''' </summary>
        ''' <param name="metric"></param>
        ''' <returns></returns>
        Function GenerateSumMetrics(metric As MetricBase) As List(Of MetricRef)
    end interface
End NameSpace