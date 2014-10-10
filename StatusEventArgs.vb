Namespace kCura.Windows.Process
	Public Class StatusEventArgs
		Private _currentRecordIndex As Int64
		Private _totalRecords As Int64
		Private _message As String
		Private _eventType As EventType
		Private _countsTowardsTotal As Boolean
		Private _additionalInfo As Object

		Public ReadOnly Property CurrentRecordIndex() As Int64
			Get
				Return _currentRecordIndex
			End Get
		End Property

		Public ReadOnly Property TotalRecords() As Int64
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

		Public ReadOnly Property CountsTowardsTotal() As Boolean
			Get
				Return _countsTowardsTotal
			End Get
		End Property

		Public ReadOnly Property AdditionalInfo() As Object
			Get
				Return _additionalInfo
			End Get
		End Property

		Public Sub New(ByVal et As EventType, ByVal recordNumber As Int64, ByVal totalRecords As Int64, ByVal message As String, ByVal countsTowardsTotal As Boolean, ByVal additionalInfo As Object)
			_eventType = et
			_currentRecordIndex = recordNumber
			_totalRecords = totalRecords
			_message = message
			_countsTowardsTotal = countsTowardsTotal
			_additionalInfo = additionalInfo
		End Sub

		Public Sub New(ByVal et As EventType, ByVal recordNumber As Int64, ByVal totalRecords As Int64, ByVal message As String, ByVal additionalInfo As Object)
			Me.New(et, recordNumber, totalRecords, message, True, additionalInfo)
		End Sub
	End Class
End Namespace
