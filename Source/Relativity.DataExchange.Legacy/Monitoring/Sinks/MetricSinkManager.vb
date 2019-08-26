Imports System.Net
Imports kCura.WinEDDS
Imports kCura.WinEDDS.Monitoring
Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.Tools
Imports Relativity.Services.ServiceProxy

Namespace Monitoring.Sinks

    Public Class MetricSinkManager
        Implements IMetricSinkManager
        Private _messageService As IMessageService = Nothing
        Private ReadOnly _metricsManagerFactory As IMetricsManagerFactory
        Private ReadOnly _serviceFactory As IServiceFactory

        Public Sub New(metricsManagerFactory As IMetricsManagerFactory, serviceFactory As IServiceFactory)
            _metricsManagerFactory = metricsManagerFactory
            _serviceFactory = serviceFactory
        End Sub

        ''' <summary>
        ''' lazy loaded message service
        ''' </summary>
        ''' <returns>
        ''' <see cref="IMessageService"/>
        ''' </returns>
        Public Function IMetricSinkManager_SetupMessageService(metricsSinkConfig As IMetricsSinkConfig) As IMessageService Implements IMetricSinkManager.SetupMessageService
            If _messageService Is Nothing Then
                _messageService = RegisterSinks(metricsSinkConfig)
            End If
            Return _messageService
        End Function

        ''' <summary>
        ''' Create new MessageService object and add sinks to it
        ''' </summary>
        Private Function RegisterSinks(metricsSinkConfig As IMetricsSinkConfig) As IMessageService
            Dim messageService As IMessageService = New MessageService()

            Dim metricSinkApm As MetricSinkApm = New MetricSinkApm(_serviceFactory, _metricsManagerFactory)
            messageService.AddSink(New ToggledMessageSink(Of TransferJobCompletedMessage)(metricSinkApm, Function() metricsSinkConfig.SendSummaryApmMetrics))
            messageService.AddSink(New ToggledMessageSink(Of TransferJobCompletedRecordsCountMessage)(metricSinkApm, Function() metricsSinkConfig.SendSummaryApmMetrics))
            messageService.AddSink(New ToggledMessageSink(Of TransferJobFailedMessage)(metricSinkApm, Function() metricsSinkConfig.SendSummaryApmMetrics))
            Dim metricSinkApmThrottled As ThrottledMessageSink(Of TransferJobProgressMessage) = New ThrottledMessageSink(Of TransferJobProgressMessage)(metricSinkApm, Function() metricsSinkConfig.ThrottleTimeout)
            messageService.AddSink(New ToggledMessageSink(Of TransferJobProgressMessage)(metricSinkApmThrottled, Function() metricsSinkConfig.SendLiveApmMetrics))
            messageService.AddSink(New ToggledMessageSink(Of TransferJobStartedMessage)(metricSinkApm, Function() metricsSinkConfig.SendSummaryApmMetrics))
            messageService.AddSink(New ToggledMessageSink(Of TransferJobStatisticsMessage)(metricSinkApm, Function() metricsSinkConfig.SendSummaryApmMetrics))
            messageService.AddSink(New ToggledMessageSink(Of TransferJobThroughputMessage)(metricSinkApm, Function() metricsSinkConfig.SendSummaryApmMetrics))
            messageService.AddSink(New ToggledMessageSink(Of TransferJobTotalRecordsCountMessage)(metricSinkApm, Function() metricsSinkConfig.SendSummaryApmMetrics))

            Return messageService
        End Function
    End Class
End NameSpace