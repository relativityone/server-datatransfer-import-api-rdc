Namespace kCura.WinEDDS.Monitoring
	Public Class TransferJobCompletedMessage
		Inherits TransferJobMessageBase
        Public Overrides ReadOnly Property BucketName As String = "RDC.Performance.JobCompletedCount"
	End Class
End Namespace