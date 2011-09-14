
Public Class FieldValueImportException
	Inherits kCura.Utility.ImporterExceptionBase

	Public Sub New(ByVal row As Int64, ByVal fieldName As String, ByVal additionalInfo As String)
		Me.New(Nothing, row, fieldName, additionalInfo)
	End Sub

	Public Sub New(ByVal innerException As System.Exception, ByVal row As Int64, ByVal fieldName As String, ByVal additionalInfo As String)
		MyBase.New(innerException, row, fieldName, additionalInfo)
		_fieldName = fieldName
		_rowNum = row
	End Sub

	Public ReadOnly Property RowNumber() As Long
		Get
			Return _rowNum
		End Get
	End Property

	Public Sub New()
		MyBase.New()
	End Sub

	Public ReadOnly Property FieldName As String
		Get
			Return _fieldName
		End Get
	End Property

	Private _fieldName As String
	Private _rowNum As Long
End Class
