Namespace Monitoring
	Public Class TransferJobThroughputMessage
		Inherits TransferJobMessageBase

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.THROUGHPUT

        ''' <summary>
        ''' Gets or sets total transfer rate in records per second
        ''' </summary>
        ''' <returns>Transfer rate in records per second</returns>
		Public Property RecordsPerSecond As Double
            Get
                Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.RECORDS_PER_SECOND)
            End Get
		    Set
                CustomData.Item(TelemetryConstants.KeyName.RECORDS_PER_SECOND) = Value
		    End Set
		End Property

        ''' <summary>
        ''' Gets or sets total transfer rate in bytes per second
        ''' </summary>
        ''' <returns>Transfer rate in bytes per second</returns>
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
