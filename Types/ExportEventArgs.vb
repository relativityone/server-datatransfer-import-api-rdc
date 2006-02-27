Namespace kCura.WinEDDS
	Public Class ExportEventArgs
		Protected _documentsExported As Int32
		Protected _totalDocuments As Int32
    Protected _eventType As kCura.Windows.Process.EventType
		Protected _message As String

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

    Public Property EventType() As kCura.Windows.Process.EventType
      Get
        Return _eventType
      End Get
      Set(ByVal value As kCura.Windows.Process.EventType)
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
#End Region

		Public Sub New(ByVal documentsExported As Int32, ByVal totalDocuments As Int32)
			_documentsExported = documentsExported
			_totalDocuments = totalDocuments
		End Sub

    Public Sub New(ByVal documentsExported As Int32, ByVal totalDocuments As Int32, ByVal message As String, ByVal eventType As kCura.Windows.Process.EventType)
      _documentsExported = documentsExported
      _totalDocuments = totalDocuments
      _message = message
      _eventType = eventType
    End Sub
  End Class
End Namespace