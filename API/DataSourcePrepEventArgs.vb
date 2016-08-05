Namespace kCura.WinEDDS.Api
	Public Class DataSourcePrepEventArgs
		Public Enum EventType
			Open
			Close
			ReadEvent
		End Enum

		Private _type As EventType
		Private _bytesRead As Int64
		Private _totalBytes As Int64
		Private _stepSize As Int64
		Private _startTime As System.DateTime
		Private _endTime As System.DateTime

#Region "Accessors"

		Public ReadOnly Property Type() As EventType
			Get
				Return _type
			End Get
		End Property

		Public ReadOnly Property BytesRead() As Int64
			Get
				Return _bytesRead
			End Get
		End Property

		Public ReadOnly Property TotalBytes() As Int64
			Get
				Return _totalBytes
			End Get
		End Property

		Public ReadOnly Property StepSize() As Int64
			Get
				Return _stepSize
			End Get
		End Property

		Public ReadOnly Property StartTime() As System.DateTime
			Get
				Return _startTime
			End Get
		End Property

		Public ReadOnly Property EndTime() As System.DateTime
			Get
				Return _endTime
			End Get
		End Property

#End Region

		Public Sub New(ByVal eventType As EventType, ByVal bytes As Int64, ByVal total As Int64, ByVal [step] As Int64, ByVal start As System.DateTime, ByVal [end] As System.DateTime)
			_type = eventType
			_bytesRead = bytes
			_totalBytes = total
			_stepSize = [step]
			_startTime = start
			_endTime = [end]
		End Sub
	End Class

End Namespace