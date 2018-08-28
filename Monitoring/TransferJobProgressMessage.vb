Imports Monitoring

Namespace kCura.WinEDDS.Monitoring
	Public Class TransferJobProgressMessage
		Inherits TransferJobMessageBase

		Private Const FileThroughputKeyName As String = "FileThroughput"
		Private Const MetadataThroughputKeyName As String = "MetadataThroughput"

		Public Property FileThroughput As Double
			Get
				Return GetValueOrDefault (Of Double)(FileThroughputKeyName)
			End Get
			Set
				CustomData.Item(FileThroughputKeyName) = Value
			End Set
		End Property

		Public Property MetadataThroughput As Double
			Get
				Return GetValueOrDefault (Of Double)(MetadataThroughputKeyName)
			End Get
			Set
				CustomData.Item(MetadataThroughputKeyName) = Value
			End Set
		End Property
	End Class
End Namespace