Imports Relativity.DataTransfer.MessageService

Namespace kCura.WinEDDS.Monitoring
	Public Class TransferJobApmThroughputMessage
		Inherits TransferJobMessageBase

		Private Const FileThroughputKeyName As String = "FileThroughput"
		Private Const MetadataThroughputKeyName As String = "MetadataThroughput"

		Public Property FileThroughput As Double
			Get
				If CustomData.ContainsKey(FileThroughputKeyName) Then
					Return CType(CustomData.Item(FileThroughputKeyName), Double)
				Else
					Return 0
				End If
			End Get
			Set
				CustomData.Item(FileThroughputKeyName) = Value
			End Set
		End Property

		Public Property MetadataThroughput As Double
			Get
				If CustomData.ContainsKey(MetadataThroughputKeyName) Then
					Return CType(CustomData.Item(MetadataThroughputKeyName), Double)
				Else
					Return 0
				End If
			End Get
			Set
				CustomData.Item(MetadataThroughputKeyName) = Value
			End Set
		End Property
	End Class
End Namespace