Imports System.Collections.Generic
Imports Relativity.DataExchange.Logger
Imports Relativity.DataExchange.Service
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
		''' <param name="keplerProxy">Kepler proxy.</param>
		Public Sub New(metricSinkConfig As IMetricSinkConfig, keplerProxy As IKeplerProxy)
			Me.MetricSinkConfig = metricSinkConfig
			_sinks = New List(Of IMetricSink) From {
				New MetricSinkApm(keplerProxy, Me.MetricSinkConfig.SendApmMetrics),
				New MetricSinkSum(keplerProxy, New SumMetricFormatter, Me.MetricSinkConfig.SendSumMetrics),
				New MetricSinkRelativityLogging(RelativityLogger.Instance.IsEnabled)}
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
		Public ReadOnly Property MetricSinkConfig As IMetricSinkConfig Implements IMetricService.MetricSinkConfig
	End Class
End Namespace