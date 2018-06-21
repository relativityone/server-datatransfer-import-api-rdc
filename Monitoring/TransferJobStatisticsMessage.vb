Namespace kCura.WinEDDS.Monitoring
	Public Class TransferJobStatisticsMessage
		Inherits TransferJobMessageBase

		Private Const JobSizeInBytesKeyName As String = "JobSizeInBytes"
		Private Const MetadataBytesKeyName As String = "MetadataBytes"
		Private Const FileBytesKeyName As String = "FileBytes"

		Public Property JobSizeInBytes As Double
			Get
				If CustomData.ContainsKey(JobSizeInBytesKeyName) Then
					Return CType(CustomData.Item(JobSizeInBytesKeyName), Double)
				Else
					Return 0
				End If
			End Get
			Set
				CustomData.Item(JobSizeInBytesKeyName) = Value
			End Set
		End Property

		Public Property MetadataBytes As Long
			Get
				If CustomData.ContainsKey(MetadataBytesKeyName) Then
					Return CType(CustomData.Item(MetadataBytesKeyName), Long)
				Else
					Return 0
				End If
			End Get
			Set
				CustomData.Item(MetadataBytesKeyName) = Value
			End Set
		End Property

		Public Property FileBytes As Long
			Get
				If CustomData.ContainsKey(FileBytesKeyName) Then
					Return CType(CustomData.Item(FileBytesKeyName), Long)
				Else
					Return 0
				End If
			End Get
			Set
				CustomData.Item(FileBytesKeyName) = Value
			End Set
		End Property
	End Class
End Namespace