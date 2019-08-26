Imports Monitoring
Imports Relativity.DataTransfer.MessageService

Namespace kCura.WinEDDS.Monitoring
	Public Class TransferJobTotalRecordsCountMessage
		Inherits TransferJobMessageBase

        Private Const TotalRecordsKeyName As String = "TotalRecords"

        Public Overrides ReadOnly Property BucketName As String = "RDC.Usage.TotalRecords"

		Public Property TotalRecords As Long
            Get
                Return GetValueOrDefault (Of Long)(TotalRecordsKeyName)
            End Get
		    Set(value As Long)
                CustomData.Item(TotalRecordsKeyName) = value
		    End Set
		End Property

	End Class
End Namespace