Namespace Monitoring
	Public Class TransferJobCompletedMessage
		Inherits TransferJobMessageBase

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.JOB_COMPLETED_COUNT
	End Class
End Namespace