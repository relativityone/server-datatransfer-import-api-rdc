Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService

Namespace Monitoring.Sinks
    Public Interface IMetricSinkManager

        ''' <summary>
        ''' Create message service and register metric sinks to support telemetry
        ''' </summary>
        ''' <param name="metricsSinkConfig">Configuration for metric sinks</param>
        ''' <returns>
        ''' <see cref="IMessageService"/>
        ''' </returns>
        Function SetupMessageService(metricsSinkConfig As IMetricsSinkConfig) As IMessageService
    End Interface
End NameSpace