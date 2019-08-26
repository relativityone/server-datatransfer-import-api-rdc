Namespace kCura.WinEDDS.Monitoring
	Public Class TransferJobStartedMessage
		Inherits TransferJobMessageBase
        Public Overrides ReadOnly Property BucketName As String = "RDC.Performance.JobStartedCount"
	End Class

End Namespace