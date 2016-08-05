Imports System.Collections.Generic

''' <summary>
''' Contains summary information about the outcome of an import job, including any errors that may have occurred.
''' </summary>
''' <remarks>
''' When an import completes successfully, it indicates that all rows were provessed, but not that all rows were imported successfully.  This class provides details about any errors which may have occurred.
''' </remarks>
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

	''' <summary>
	''' Import start time.;
	''' </summary>
	Public Property StartTime As DateTime
		Get
			Return _startTime
		End Get
		Friend Set(value As DateTime)
			_startTime = value
		End Set
	End Property

	''' <summary>
	''' Import finish time.
	''' </summary>
	Public Property EndTime As DateTime
		Get
			Return _endTime
		End Get
		Friend Set(value As DateTime)
			_endTime = value
		End Set
	End Property

	''' <summary>
	''' Collection of field map entries, indicating the mapping of source fields to destination fields in the workspace.
	''' </summary>
	Public ReadOnly Property FieldMap As IList(Of FieldMapEntry)
		Get
			Return _fieldMap
		End Get
	End Property

	''' <summary>
	''' The total number of process rows.
	''' </summary>
	''' <remarks>
	''' This value indicates the number of processed rows, not the number of successful rows.
	''' </remarks>
	Public Property TotalRows As Integer
		Get
			Return _totalRows
		End Get
		Friend Set(value As Integer)
			_totalRows = value
		End Set
	End Property

	''' <summary>
	''' The total number of non-fatal row-level errors that occurred.
	''' </summary>
	Public ReadOnly Property ErrorRowCount As Integer
		Get
			Return _errorRows.Count
		End Get
	End Property

	''' <summary>
	''' The exception which resulted in the OnFatalException event.
	''' </summary>
	Public Property FatalException As Exception
		Get
			Return _fatalException
		End Get
		Friend Set(value As Exception)
			_fatalException = value
		End Set
	End Property

	''' <summary>
	''' The collection of non-fatal row-level errors that occurred.
	''' </summary>
	Public ReadOnly Property ErrorRows As IList(Of RowError)
		Get
			Return _errorRows
		End Get
	End Property

	''' <summary>
	''' Provides information about an error in a specific row.
	''' </summary>
	Public Class RowError
		Public Sub New(ByVal rowNumber As Long, ByVal message As String, ByVal identifier As String)
			Me.RowNumber = rowNumber
			Me.Message = message
			Me.Identifier = identifier
		End Sub

		''' <summary>
		''' The number of the row containing the error.
		''' </summary>		
		''' <remarks>
		''' Row numbering begins at one.
		''' </remarks>
		Public Property RowNumber As Long

		''' <summary>
		''' The error message.
		''' </summary>
		Public Property Message As String

		''' <summary>
		''' The value of the identifier field in the row.
		''' </summary>
		Public Property Identifier As String
	End Class

	''' <summary>
	''' Provides information about the mapping between a source field and the destination field in a workspace.
	''' </summary>
	Public Class FieldMapEntry
		Public Sub New(ByVal sourceField As String, workspaceField As String)
			Me.SourceField = sourceField
			Me.WorkspaceField = workspaceField
		End Sub

		''' <summary>
		''' The name of the source field.
		''' </summary>
		Public Property SourceField As String

		''' <summary>
		''' The name of the destination field in the workspace.
		''' </summary>
		Public Property WorkspaceField As String
	End Class
End Class
