Namespace kCura.WinEDDS.Monitoring
	Public Class TransferJobFailedMessage
		Inherits TransferJobMessageBase
        Public Overrides ReadOnly Property BucketName As String = "RDC.Performance.JobFailedCount"
	End Class
End Namespace
