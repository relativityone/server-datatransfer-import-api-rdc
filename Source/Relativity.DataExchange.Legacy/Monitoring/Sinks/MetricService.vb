Imports System.Collections.Generic
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Logger
Imports Relativity.Services.ServiceProxy

Namespace Monitoring.Sinks

	Public Class MetricService
		Implements IMetricService

		Private ReadOnly _sinks As List(Of IMetricSink)

		''' <summary>
		''' Create sinks that does not require <see cref="IServiceFactory"/> and setup their configuration.
		''' </summary>
		''' <param name="metricSinkConfig">Sinks configuration.</param>
		Public Sub New(metricSinkConfig As IMetricSinkConfig)
			Me.MetricSinkConfig = metricSinkConfig
			_sinks = New List(Of IMetricSink)()
		End Sub

		''' <summary>
		''' Create all sinks and setup their configuration
		''' </summary>
		''' <param name="metricSinkConfig">Sinks configuration.</param>
		''' <param name="serviceFactory">Used to create proxies of Relativity Services.</param>
		Public Sub New(metricSinkConfig As IMetricSinkConfig, serviceFactory As IServiceFactory)
			Me.MetricSinkConfig = metricSinkConfig
			_sinks = New List(Of IMetricSink) From {New MetricSinkApm(serviceFactory, Me.MetricSinkConfig.SendApmMetrics), New MetricSinkSum(serviceFactory, New SumMetricFormatter, Me.MetricSinkConfig.SendSumMetrics)}
		End Sub

		''' <inheritdoc/>
		Public Sub Log(metric As MetricBase) Implements IMetricService.Log
			For Each sink As IMetricSink In _sinks
				If sink.IsEnabled Then
					Try
						sink.Log(metric)
					Catch ex As Exception
						RelativityLogger.Instance.LogWarning(ex, "Failed to submit {metricType}", metric.GetType())
					End Try
				End If
			Next
		End Sub

		''' <inheritdoc/>
		Public Readonly Property MetricSinkConfig As IMetricSinkConfig Implements IMetricService.MetricSinkConfig
	End Class
End NameSpace