Namespace Monitoring
	Public Class TransferJobThroughputMessage
		Inherits TransferJobMessageBase

        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.THROUGHPUT

		Public Property RecordsPerSecond As Double
            Get
                Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.RECORDS_PER_SECOND)
            End Get
		    Set
                CustomData.Item(TelemetryConstants.KeyName.RECORDS_PER_SECOND) = Value
		    End Set
		End Property

		Public Property BytesPerSecond As Double
            Get
                Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.BYTES_PER_SECOND)
            End Get
		    Set(value As Double)
                CustomData.Item(TelemetryConstants.KeyName.BYTES_PER_SECOND) = Value
		    End Set
		End Property

	End Class
End Namespace
