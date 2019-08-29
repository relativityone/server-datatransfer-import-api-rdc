Namespace Monitoring
	Public Class TransferJobFailedMessage
		Inherits TransferJobMessageBase
        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.JOB_FAILED_COUNT
	End Class
End Namespace
