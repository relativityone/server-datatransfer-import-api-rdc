Imports Monitoring
Imports Relativity.DataTransfer.MessageService

Namespace kCura.WinEDDS.Monitoring
	Public Class TransferJobCompletedRecordsCountMessage
		Inherits TransferJobMessageBase

        Private Const CompletedRecordsKeyName As String = "CompletedRecords"

        Public Overrides ReadOnly Property BucketName As String = "RDC.Usage.CompletedRecords"

		Public Property CompletedRecords As Long
            Get
                Return GetValueOrDefault (Of Long)(CompletedRecordsKeyName)
            End Get
		    Set(value As Long)
                CustomData.Item(CompletedRecordsKeyName) = value
		    End Set
		End Property
	End Class
End Namespace