'Imports kCura.EDDS.DynamicFields
Namespace kCura.WinEDDS
	Public Class LoadFilePreviewer
		Inherits kCura.WinEDDS.LoadFileBase

#Region "Members"
		Private _errorsOnly As Boolean
#End Region

#Region "Constructors"

		Public Sub New(ByVal args As LoadFile, ByVal timeZoneOffset As Int32, ByVal errorsOnly As Boolean)
			MyBase.New(args, timeZoneOffset)
			_errorsOnly = errorsOnly
		End Sub

#End Region

		Public Overrides Function ReadFile(ByVal path As String) As Object
			Reader = New System.IO.StreamReader(path, System.Text.Encoding.Default)
			Dim fieldArrays As New System.Collections.ArrayList
			If _firstLineContainsColumnNames Then
				_columnHeaders = GetLine
				_filePathColumnIndex = Array.IndexOf(_columnHeaders, _filePathColumn)
			End If
			Dim i As Int32 = 0
			While Not HasReachedEOF
				If fieldArrays.Count < 1000 Then
					Dim x As DocumentField() = CheckLine(getline)
					If Not x Is Nothing Then fieldArrays.Add(x)
					i += 1
				Else
					Exit While
				End If
			End While
			Return fieldArrays
		End Function
		Private Function CheckLine(ByVal values As String()) As DocumentField()
			Dim mapItem As LoadFileFieldMap.LoadFileFieldMapItem
			Dim lineContainsErrors As Boolean = False
			Dim retval As New ArrayList
			For Each mapItem In _fieldMap
				If mapItem.NativeFileColumnIndex > -1 AndAlso Not mapItem.DocumentField Is Nothing Then
					Dim docfield As New DocumentField(mapItem.DocumentField)
					lineContainsErrors = lineContainsErrors Or SetFieldValueOrErrorMessage(docfield, values(mapItem.NativeFileColumnIndex), mapItem.NativeFileColumnIndex)
					retval.Add(docfield)
				End If
			Next
			If _errorsOnly Then
				If lineContainsErrors Then
					Return DirectCast(retval.ToArray(GetType(DocumentField)), DocumentField())
				Else
					Return Nothing
				End If
			Else
				Return DirectCast(retval.ToArray(GetType(DocumentField)), DocumentField())
			End If
		End Function

		'Private Function CheckLine(ByVal values As String()) As DocumentField()
		'	Dim docFields(_docFields.Length - 1) As DocumentField
		'	Dim docField As DocumentField
		'	Dim i As Int32 = 0
		'	Dim lineContainsErrors As Boolean = False
		'	'Dim fieldIDs(_docFields.Length - 1) As Int32
		'	'Dim fieldValues(_docFields.Length - 1) As String
		'	For Each docField In _docfields
		'		docField.Value = ""
		'	Next
		'	For Each docField In _docFields
		'		lineContainsErrors = lineContainsErrors Or SetFieldValueOrErrorMessage(docField, values(i), i)
		'		i += 1
		'		If values.Length - 1 < i Then
		'			Exit For
		'		End If
		'	Next
		'	For i = 0 To _docFields.Length - 1
		'		docFields(i) = New DocumentField(_docFields(i))
		'		docFields(i).Value = _docFields(i).Value
		'	Next
		'	If _errorsOnly Then
		'		If lineContainsErrors Then
		'			Return docFields
		'		Else
		'			Return Nothing
		'		End If
		'	Else
		'		Return docFields
		'	End If
		'End Function

		Private Function SetFieldValueOrErrorMessage(ByVal field As DocumentField, ByVal value As String, ByVal column As Int32) As Boolean
			Try
				SetFieldValue(field, value, column, True)
				Return False
			Catch ex As kCura.Utility.DelimitedFileImporter.ImporterExceptionBase
				field.Value = ex.Message
				Return True
			End Try
		End Function
	End Class
End Namespace

