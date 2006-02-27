Namespace kCura.Windows.Process
  Public Class StatusEventArgs
    Private _currentRecordIndex As Int32
    Private _totalRecords As Int32
    Private _message As String
    Private _eventType As EventType

    Public ReadOnly Property CurrentRecordIndex() As Int32
      Get
        Return _currentRecordIndex
      End Get
    End Property

    Public ReadOnly Property TotalRecords() As Int32
      Get
        Return _totalRecords
      End Get
    End Property

    Public ReadOnly Property Message() As String
      Get
        Return _message
      End Get
    End Property

    Public ReadOnly Property EventType() As EventType
      Get
        Return _eventType
      End Get
    End Property

    Public Sub New(ByVal et As EventType, ByVal recordNumber As Int32, ByVal totalRecords As Int32, ByVal message As String)
      _eventType = et
      _currentRecordIndex = recordNumber
      _totalRecords = totalRecords
      _message = message
    End Sub
  End Class
End Namespace
