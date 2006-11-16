Namespace kCura.Windows.Process
	Public Class ProcessProgressEvent

#Region "Members"
		Private _startTime As DateTime
		Private _endTime As DateTime
		Private _totalRecords As Int64
		Private _totalRecordsProcessed As Int64
		Private _totalRecordsProcessedWithWarnings As Int64
		Private _totalRecordsProcessedWithErrors As Int64
		Private _totalRecordsDisplay As String
		Private _totalRecordsProcessedDisplay As String
#End Region

#Region "Accessors"
		Public Property StartTime() As DateTime
			Get
				Return _startTime
			End Get
			Set(ByVal value As DateTime)
				_startTime = value
			End Set
		End Property

		Public Property EndTime() As DateTime
			Get
				Return _endTime
			End Get
			Set(ByVal value As DateTime)
				_endTime = value
			End Set
		End Property

		Public Property TotalRecords() As Int64
			Get
				Return _totalRecords
			End Get
			Set(ByVal value As Int64)
				_totalRecords = value
			End Set
		End Property

		Public Property TotalRecordsProcessed() As Int64
			Get
				Return _totalRecordsProcessed
			End Get
			Set(ByVal value As Int64)
				_totalRecordsProcessed = value
			End Set
		End Property

		Public Property TotalRecordsProcessedWithWarnings() As Int64
			Get
				Return _totalRecordsProcessedWithWarnings
			End Get
			Set(ByVal value As Int64)
				_totalRecordsProcessedWithWarnings = value
			End Set
		End Property

		Public Property TotalRecordsProcessedWithErrors() As Int64
			Get
				Return _totalRecordsProcessedWithErrors
			End Get
			Set(ByVal value As Int64)
				_totalRecordsProcessedWithErrors = value
			End Set
		End Property

		Public Property TotalRecordsDisplay() As String
			Get
				Return _totalRecordsDisplay
			End Get
			Set(ByVal value As String)
				_totalRecordsDisplay = value
			End Set
		End Property

		Public Property TotalRecordsProcessedDisplay() As String
			Get
				Return _totalRecordsProcessedDisplay
			End Get
			Set(ByVal value As String)
				_totalRecordsProcessedDisplay = value
			End Set
		End Property

#End Region

		Public Sub New(ByVal totRecs As Int64, ByVal totRecsProc As Int64, ByVal totRecsProcWarn As Int64, ByVal totRecsProcErr As Int64, ByVal sTime As DateTime, ByVal eTime As DateTime, ByVal totRecsDisplay As String, ByVal totRecsProcDisplay As String)
			Me.TotalRecords = totRecs
			Me.TotalRecordsProcessed = totRecsProc
			If Not totRecsDisplay Is Nothing Then
				Me.TotalRecordsDisplay = totRecsDisplay
			Else
				Me.TotalRecordsDisplay = totRecs.ToString
			End If
			If Not totRecsProcDisplay Is Nothing Then
				Me.TotalRecordsProcessedDisplay = totRecsProcDisplay
			Else
				Me.TotalRecordsProcessedDisplay = totRecsProc.ToString
			End If
			Me.TotalRecordsProcessedWithWarnings = totRecsProcWarn
			Me.TotalRecordsProcessedWithErrors = totRecsProcErr
			Me.StartTime = sTime
			Me.EndTime = eTime
		End Sub

	End Class
End Namespace