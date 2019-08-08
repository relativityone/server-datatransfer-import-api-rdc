Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService

Namespace Monitoring.Sinks
    Public Interface IMetricSinkManager
        Function SetupMessageService(metricsSinkConfig As IMetricsSinkConfig) As IMessageService
    End Interface
End NameSpace