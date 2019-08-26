Imports Monitoring
Imports Relativity.DataTransfer.MessageService

Namespace kCura.WinEDDS.Monitoring
	Public Class TransferJobThroughputMessage
		Inherits TransferJobMessageBase

        Private Const RecordsPerSecondKeyName As String = "RecordsPerSecond"
        Private Const BytesPerSecondKeyName As String = "BytesPerSecond"

        Public Overrides ReadOnly Property BucketName As String = "RDC.Performance.Throughput"

		Public Property RecordsPerSecond As Double
            Get
                Return GetValueOrDefault (Of Double)(RecordsPerSecondKeyName)
            End Get
		    Set(value As Double)
                CustomData.Item(RecordsPerSecondKeyName) = value
		    End Set
		End Property

		Public Property BytesPerSecond As Double
            Get
                Return GetValueOrDefault (Of Double)(BytesPerSecondKeyName)
            End Get
		    Set(value As Double)
                CustomData.Item(BytesPerSecondKeyName) = value
		    End Set
		End Property

	End Class
End Namespace
