Namespace Monitoring.Sinks
	Public interface IMetricSinkConfig

        ''' <summary>
        ''' Gets or sets a value indicating how often to submit APM metrics during job execution.
        ''' </summary>
        ''' <returns>Period in which we're sending APM metrics.</returns>
		Property ThrottleTimeout() As TimeSpan

        ''' <summary>
        ''' Gets or sets a value indicating whether to submit SUM metrics.
        ''' </summary>
        ''' <returns>True to submit SUM metrics, otherwise False.</returns>
		Property SendSumMetrics() As Boolean

        ''' <summary>
        ''' Gets or sets a value indicating whether to submit APM metrics.
        ''' </summary>
        ''' <returns>True to submit APM metrics, otherwise False.</returns>
		Property SendApmMetrics() As Boolean
	end Interface
End NameSpace