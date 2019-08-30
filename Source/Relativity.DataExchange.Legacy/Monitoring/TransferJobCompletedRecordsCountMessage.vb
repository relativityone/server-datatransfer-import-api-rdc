Namespace Monitoring
	Public Class TransferJobCompletedRecordsCountMessage
		Inherits TransferJobMessageBase

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.COMPLETED_RECORDS

        ''' <summary>
        ''' Gets or sets number of successfully processed records
        ''' </summary>
        ''' <returns>Number of completed records</returns>
		Public Property CompletedRecords As Long
            Get
                Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.COMPLETED_RECORDS)
            End Get
		    Set
                CustomData.Item(TelemetryConstants.KeyName.COMPLETED_RECORDS) = Value
		    End Set
		End Property
	End Class
End Namespace