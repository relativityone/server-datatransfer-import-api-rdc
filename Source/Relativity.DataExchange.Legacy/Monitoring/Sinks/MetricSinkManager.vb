Imports System.Net
Imports kCura.WinEDDS
Imports kCura.WinEDDS.Monitoring
Imports Monitoring.Sinks
Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.Tools
Imports Relativity.Services.ServiceProxy

Public Class MetricSinkManager
    Implements IMetricSinkManager
    Private _messageService As IMessageService = Nothing
    Private ReadOnly _metricsSinkConfig As IMetricsSinkConfig
    Private ReadOnly _metricsManagerFactory As IMetricsManagerFactory
    Private ReadOnly _serviceFactory As IServiceFactory

    Public Sub New(metricsSinkConfig As IMetricsSinkConfig, metricsManagerFactory As IMetricsManagerFactory, serviceFactory As IServiceFactory)
        _metricsSinkConfig = metricsSinkConfig
        _metricsManagerFactory = metricsManagerFactory
        _serviceFactory = serviceFactory
    End Sub

    ''' <summary>
    ''' lazy loaded message service
    ''' </summary>
    ''' <returns>
    ''' <see cref="IMessageService"/>
    ''' </returns>
    Public Function IMetricSinkManager_SetupMessageService() As IMessageService Implements IMetricSinkManager.SetupMessageService
        If _messageService Is Nothing Then
            _messageService = RegisterSinks()
        End If
        Return _messageService
    End Function

    ''' <summary>
    ''' Create new MessageService object and add sinks to it
    ''' </summary>
    Private Function RegisterSinks() As IMessageService
        Dim messageService As IMessageService = New MessageService()

        Dim jobLiveSink As JobLiveMetricSink = New JobLiveMetricSink(_serviceFactory, _metricsManagerFactory)
        Dim jobLifetimeSink As JobLifetimeSink = New JobLifetimeSink(_serviceFactory, _metricsManagerFactory)
        Dim jobSumEolSink As JobSumEndOfLifeSink = New JobSumEndOfLifeSink(_serviceFactory, _metricsManagerFactory)
        Dim jobApmEolSink As JobApmEndOfLifeSink = New JobApmEndOfLifeSink(_serviceFactory, _metricsManagerFactory)

        Dim jobLiveThrottledSink As ThrottledMessageSink(Of TransferJobProgressMessage) = New ThrottledMessageSink(Of TransferJobProgressMessage)(jobLiveSink, Function() _metricsSinkConfig.ThrottleTimeout)
				

        messageService.AddSink(New ToggledMessageSink(Of TransferJobStartedMessage)(jobLifetimeSink, Function() _metricsSinkConfig.SendSumMetrics))
        messageService.AddSink(New ToggledMessageSink(Of TransferJobCompletedMessage)(jobLifetimeSink, Function() _metricsSinkConfig.SendSumMetrics))
        messageService.AddSink(New ToggledMessageSink(Of TransferJobFailedMessage)(jobLifetimeSink, Function() _metricsSinkConfig.SendSumMetrics))

        messageService.AddSink(New ToggledMessageSink(Of TransferJobThroughputMessage)(jobSumEolSink, Function() _metricsSinkConfig.SendSumMetrics))
        messageService.AddSink(New ToggledMessageSink(Of TransferJobTotalRecordsCountMessage)(jobSumEolSink, Function() _metricsSinkConfig.SendSumMetrics))
        messageService.AddSink(New ToggledMessageSink(Of TransferJobCompletedRecordsCountMessage)(jobSumEolSink, Function() _metricsSinkConfig.SendSumMetrics))
        messageService.AddSink(New ToggledMessageSink(Of TransferJobStatisticsMessage)(jobSumEolSink, Function() _metricsSinkConfig.SendSumMetrics))

        messageService.AddSink(New ToggledMessageSink(Of TransferJobProgressMessage)(jobLiveThrottledSink, Function() _metricsSinkConfig.SendLiveApmMetrics))

        messageService.AddSink(New ToggledMessageSink(Of TransferJobStatisticsMessage)(jobApmEolSink, Function() _metricsSinkConfig.SendSummaryApmMetrics))

        Return messageService
    End Function
End Class