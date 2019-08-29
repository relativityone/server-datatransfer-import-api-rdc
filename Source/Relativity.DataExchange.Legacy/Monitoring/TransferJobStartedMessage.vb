Namespace Monitoring
	Public Class TransferJobStartedMessage
		Inherits TransferJobMessageBase
        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.JOB_STARTED_COUNT
	End Class
End Namespace