Namespace Monitoring
	Public Class MetricJobStarted
		Inherits MetricBase

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.METRIC_JOB_STARTED
	End Class
End Namespace