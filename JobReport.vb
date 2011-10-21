Imports System.Collections.Generic

Public Class JobReport


	Friend Sub New()
		_errorRows = New List(Of RowError)()
		_fieldMap = New List(Of FieldMapEntry)()

	End Sub

	Private _totalRows As Integer
	Private _fatalException As Exception
	Private _fieldMap As IList(Of FieldMapEntry)
	Private _errorRows As IList(Of RowError)
	Private _startTime As DateTime
	Private _endTime As DateTime

	Public Property StartTime As DateTime
		Get
			Return _startTime
		End Get
		Friend Set(value As DateTime)
			_startTime = value
		End Set
	End Property

	Public Property EndTime As DateTime
		Get
			Return _endTime
		End Get
		Friend Set(value As DateTime)
			_endTime = value
		End Set
	End Property

	Public ReadOnly Property FieldMap As IList(Of FieldMapEntry)
		Get
			Return _fieldMap
		End Get
	End Property


	Public Property TotalRows As Integer
		Get
			Return _totalRows
		End Get
		Friend Set(value As Integer)
			_totalRows = value
		End Set
	End Property

	Public ReadOnly Property ErrorRowCount As Integer
		Get
			Return _errorRows.Count
		End Get
	End Property

	Public Property FatalException As Exception
		Get
			Return _fatalException
		End Get
		Friend Set(value As Exception)
			_fatalException = value
		End Set
	End Property

	Public ReadOnly Property ErrorRows As IList(Of RowError)
		Get
			Return _errorRows
		End Get
	End Property

	Public Class RowError
		Public Sub New(ByVal rowNumber As Long, ByVal message As String, ByVal identifier As String)
			Me.RowNumber = rowNumber
			Me.Message = message
			Me.Identifier = identifier
		End Sub
		Public Property RowNumber As Long
		Public Property Message As String
		Public Property Identifier As String
	End Class

	Public Class FieldMapEntry
		Public Sub New(ByVal sourceField As String, workspaceField As String)
			Me.SourceField = sourceField
			Me.WorkspaceField = workspaceField
		End Sub
		Public Property SourceField As String
		Public Property WorkspaceField As String
	End Class
End Class
