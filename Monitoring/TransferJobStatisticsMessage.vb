Imports Monitoring

Namespace kCura.WinEDDS.Monitoring
	Public Class TransferJobStatisticsMessage
		Inherits TransferJobMessageBase

		Private Const JobSizeInBytesKeyName As String = "JobSizeInBytes"
		Private Const MetadataBytesKeyName As String = "MetadataBytes"
		Private Const FileBytesKeyName As String = "FileBytes"

		Public Property JobSizeInBytes As Double
			Get
				Return GetValueOrDefault (Of Double)(JobSizeInBytesKeyName)
			End Get
			Set
				CustomData.Item(JobSizeInBytesKeyName) = Value
			End Set
		End Property

		Public Property MetadataBytes As Long
			Get
				Return GetValueOrDefault (Of Long)(MetadataBytesKeyName)
			End Get
			Set
				CustomData.Item(MetadataBytesKeyName) = Value
			End Set
		End Property

		Public Property FileBytes As Long
			Get
				Return GetValueOrDefault (Of Long)(FileBytesKeyName)
			End Get
			Set
				CustomData.Item(FileBytesKeyName) = Value
			End Set
		End Property
	End Class
End Namespace