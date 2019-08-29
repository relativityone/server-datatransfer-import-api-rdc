Namespace Monitoring
	Public Class TransferJobCompletedMessage
		Inherits TransferJobMessageBase
        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.JOB_COMPLETED_COUNT
	End Class
End Namespace