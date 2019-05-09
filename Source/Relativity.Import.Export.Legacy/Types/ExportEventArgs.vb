Imports Relativity.Import.Export.Process

Namespace kCura.WinEDDS
	Public Class ExportEventArgs
		Protected _documentsExported As Int32
		Protected _totalDocuments As Int32
		Protected _eventType As EventType2
		Protected _message As String
		Protected _additionalInfo As Object
		Protected _statistics As Statistics

#Region "Public Properties"
		Public Property DocumentsExported() As Int32
			Get
				Return _documentsExported
			End Get
			Set(ByVal value As Int32)
				_documentsExported = value
			End Set
		End Property

		Public Property TotalDocuments() As Int32
			Get
				Return _totalDocuments
			End Get
			Set(ByVal value As Int32)
				_totalDocuments = value
			End Set
		End Property

		Public Property EventType() As EventType2
			Get
				Return _eventType
			End Get
			Set(ByVal value As EventType2)
				_eventType = value
			End Set
		End Property

		Public Property Message() As String
			Get
				Return _message
			End Get
			Set(ByVal value As String)
				_message = value
			End Set
		End Property

		Public ReadOnly Property AdditionalInfo() As Object
			Get
				Return _additionalInfo
			End Get
		End Property
		
		Public ReadOnly Property Statistics() As Statistics
			Get
				Return _statistics
			End Get
		End Property
#End Region

		Public Sub New(ByVal documentsExported As Int32, ByVal totalDocuments As Int32, ByVal additionalInfo As Object, ByVal statistics As Statistics)
			_documentsExported = documentsExported
			_totalDocuments = totalDocuments
			_additionalInfo = additionalInfo
			_statistics = statistics
		End Sub

		Public Sub New(ByVal documentsExported As Int32, ByVal totalDocuments As Int32, ByVal message As String, ByVal eventType As EventType2, ByVal additionalInfo As Object, ByVal statistics As Statistics)
			_documentsExported = documentsExported
			_totalDocuments = totalDocuments
			_message = message
			_eventType = eventType
			_additionalInfo = additionalInfo
			_statistics = statistics
		End Sub
	End Class
End Namespace