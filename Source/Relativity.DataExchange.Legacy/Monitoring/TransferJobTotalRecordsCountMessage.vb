Namespace Monitoring
	Public Class TransferJobTotalRecordsCountMessage
		Inherits TransferJobMessageBase

        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.TOTAL_RECORDS

		Public Property TotalRecords As Long
            Get
                Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.TOTAL_RECORDS)
            End Get
		    Set
                CustomData.Item(TelemetryConstants.KeyName.TOTAL_RECORDS) = Value
		    End Set
		End Property

	End Class
End Namespace