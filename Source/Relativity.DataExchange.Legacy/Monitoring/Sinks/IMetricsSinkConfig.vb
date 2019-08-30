Namespace kCura.WinEDDS.Monitoring
	Public interface IMetricsSinkConfig

        ''' <summary>
        ''' Gets or sets a value indicating how often to submit APM metrics during job execution.
        ''' </summary>
        ''' <returns>Period in which we're sending APM metrics.</returns>
		Property ThrottleTimeout() As TimeSpan

        ''' <summary>
        ''' Gets or sets a value indicating whether to submit APM metrics periodically when executing a job.
        ''' </summary>
        ''' <returns>True to submit APM metrics periodically, otherwise False.</returns>
		Property SendLiveApmMetrics As Boolean

        ''' <summary>
        ''' Gets or sets a value indicating whether to submit SUM metrics on job start and on job completion.
        ''' </summary>
        ''' <returns>True to submit SUM metrics, otherwise False.</returns>
		Property SendSumMetrics() As Boolean

        ''' <summary>
        ''' Gets or sets a value indicating whether to submit APM metrics on job completion.
        ''' </summary>
        ''' <returns>True to submit APM metrics, otherwise False.</returns>
		Property SendSummaryApmMetrics() As Boolean
	end Interface
End NameSpace